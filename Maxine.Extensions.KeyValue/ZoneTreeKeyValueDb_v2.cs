using System.Diagnostics.CodeAnalysis;
using Tenray.ZoneTree;
using Tenray.ZoneTree.Comparers;
using Tenray.ZoneTree.Options;
using Tenray.ZoneTree.Serializers;
using InvalidOperationException = System.InvalidOperationException;

namespace Maxine.Extensions.KeyValue;

public sealed class ZoneTreeKeyValueDb2 : IKeyValueDb
{
    private sealed class MemoryComparerAscending : IRefComparer<ReadOnlyMemory<byte>>, IRefComparer<Memory<byte>>
    {
        public int Compare(in ReadOnlyMemory<byte> x, in ReadOnlyMemory<byte> y)
        {
            return x.Span.SequenceCompareTo(y.Span);
        }

        public int Compare(in Memory<byte> x, in Memory<byte> y)
        {
            return x.Span.SequenceCompareTo(y.Span);
        }
    }

    private sealed class UnsafeMemorySerializer : ISerializer<ReadOnlyMemory<byte>>, ISerializer<Memory<byte>>
    {
        ReadOnlyMemory<byte> ISerializer<ReadOnlyMemory<byte>>.Deserialize(Memory<byte> bytes)
        {
            // Need to create new byte array.
            // Otherwise, the data in memory would attach to the block caches.
            // return bytes.ToArray();
            
            // Option 2: unsafe! we know we're always returning a brand new array so this shouldn't matter
            return bytes;
        }

        Memory<byte> ISerializer<ReadOnlyMemory<byte>>.Serialize(in ReadOnlyMemory<byte> entry)
        {
            return entry.ToArray();
        }

        Memory<byte> ISerializer<Memory<byte>>.Deserialize(Memory<byte> bytes)
        {
            // Need to create new byte array.
            // Otherwise, the data in memory would attach to the block caches.
            // return bytes.ToArray();
            
            // Option 2: unsafe! we know we're always returning a brand new array so this shouldn't matter
            return bytes;
        }

        Memory<byte> ISerializer<Memory<byte>>.Serialize(in Memory<byte> entry)
        {
            return entry;
        }
    }
    
    private readonly IZoneTree<Memory<byte>, Memory<byte>> _zoneTree;

    private readonly KeyValues<string, KvDbTable> _tables;
    public ZoneTreeKeyValueDb2(string rootPath)
    {
        _zoneTree = new ZoneTreeFactory<Memory<byte>, Memory<byte>>()
            .SetComparer(new MemoryComparerAscending())
            .SetDataDirectory(rootPath)
            .SetKeySerializer(new UnsafeMemorySerializer())
            .SetValueSerializer(new UnsafeMemorySerializer())
            .SetIsDeletedDelegate((in Memory<byte> _, in Memory<byte> value) => value.Span is [1])
            .SetMarkValueDeletedDelegate((ref Memory<byte> value) => value = (byte[])[1])
            .ConfigureWriteAheadLogOptions(e =>
            {
                e.WriteAheadLogMode = WriteAheadLogMode.Sync;
            })
            .ConfigureDiskSegmentOptions(e =>
            {
                e.CompressionMethod = CompressionMethod.LZ4;
                e.CompressionLevel = CompressionLevels.LZ4Fastest;
            })
            .OpenOrCreate();

        _tables = new KeyValues<string, KvDbTable>(
            _zoneTree,
            Helpers.GetGuidFromName(Helpers.TablesConstantName),
            Helpers.GetSerializer<string>(ProviderType.Default),
            Helpers.GetSerializer<KvDbTable>(ProviderType.FlatBuffers)
        );
    }

    public IKeyValues<TKey, TValue> MakeTableInternal<TKey, TValue>(string name, ProviderType keyProviderType, ProviderType valueProviderType)
    {
        var guid = Helpers.GetGuidFromName(name);
        
        _tables.Upsert(name, new KvDbTable { TableGuid = guid, KeyTypeName = typeof(TKey).HumanizeTypeName(), ValueTypeName = typeof(TValue).HumanizeTypeName() });
        
        return new KeyValues<TKey, TValue>(
            _zoneTree,
            guid,
            Helpers.GetSerializer<TKey>(keyProviderType),
            Helpers.GetSerializer<TValue>(valueProviderType)
        );
    }

    private sealed class KeyValues<TKey, TValue>(IZoneTree<Memory<byte>, Memory<byte>> zoneTree, Guid guid, IMaxineSerializer<TKey> keySerializer, IMaxineSerializer<TValue> valueSerializer) : IKeyValues<TKey, TValue>
    {    
        internal const int DefaultStackallocSize = 128;
        private const int MaxStackallocSize = 256;

        private int GetKeyStackallocSize(in TKey key)
        {
            var sizeHint = keySerializer.SizeHint(key) ?? (DefaultStackallocSize - GuidSerializer.GuidLength);
            return Math.Min(sizeHint + GuidSerializer.GuidLength, MaxStackallocSize);
        }

        private byte[] GetPrefixedKeyBytes(in TKey key)
        {
            var builder = new ValueArrayBuilder<byte>(GetKeyStackallocSize(key));

            try
            {
                GuidSerializer.Serialize(guid, ref builder);
                keySerializer.Serialize(key, ref builder);

                return builder.AsSpan().ToArray();
            }
            finally
            {
                builder.Dispose();
            }
        }

        private byte[] GetValueBytesWithIsDeletedPrefix(in TValue value)
        {
            var sizeHint = valueSerializer.SizeHint(value) ?? (DefaultStackallocSize - 1);

            var builder = new ValueArrayBuilder<byte>(stackalloc byte[sizeHint + 1]);

            try
            {
                builder.Append(0); // IsDeleted = false
                valueSerializer.Serialize(value, ref builder);
                
                return builder.AsSpan().ToArray();
            }
            finally
            {
                builder.Dispose();
            }
        }
        
        public bool TryGet(in TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            var keyBytes = GetPrefixedKeyBytes(key);
            
            if (zoneTree.TryGet(keyBytes, out var value1) && value1.Span is var valueSpan and not [1] and not [1, ..])
            {
                if (valueSerializer.TryDeserialize(valueSpan[1..], out value, out _))
                    return true;

                throw new InvalidOperationException("Could not deserialize value from bytes");
            }

            value = default;
            return false;
        }

        public void Upsert(in TKey key, in TValue value)
        {
            zoneTree.AtomicUpsert(GetPrefixedKeyBytes(key), GetValueBytesWithIsDeletedPrefix(value));
        }

        public bool Delete(in TKey key)
        {
            return zoneTree.TryDelete(GetPrefixedKeyBytes(key), out _);
        }

        public void Dispose()
        {
        }
    }

    public void Dispose()
    {
        _zoneTree.Dispose();
    }
}