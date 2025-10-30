using System.Diagnostics.CodeAnalysis;
using RocksDbSharp;

namespace Maxine.Extensions.KeyValue;

public sealed class RocksKeyValueDb : IKeyValueDb
{
    private readonly RocksDb _db;
    private readonly KeyValues<string, KvDbTable> _tables;

    public RocksKeyValueDb(string rootPath)
    {
        var options = new DbOptions()
                .SetCreateIfMissing()
                .SetCompression(Compression.Zstd)
                .SetUseFsync(1) // https://github.com/facebook/rocksdb/wiki/basic-operations#synchronous-writes
            ;

        _db = RocksDb.Open(options, rootPath);
        
        _tables = new KeyValues<string, KvDbTable>(
            _db,
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
            _db,
            guid,
            Helpers.GetSerializer<TKey>(keyProviderType),
            Helpers.GetSerializer<TValue>(valueProviderType)
        );
    }

    private sealed class KeyValues<TKey, TValue>(RocksDb db, Guid guid, IMaxineSerializer<TKey> keySerializer, IMaxineSerializer<TValue> valueSerializer) : IKeyValues<TKey, TValue>
    {
        private const int DefaultStackallocSize = 128;
        private const int MaxStackallocSize = 256;

        private int GetKeyStackallocSize(in TKey key)
        {
            var sizeHint = keySerializer.SizeHint(key) ?? (DefaultStackallocSize - GuidSerializer.GuidLength);
            return Math.Min(sizeHint + GuidSerializer.GuidLength, MaxStackallocSize);
        }

        private ReadOnlySpan<byte> GetPrefixedKeyBytes(ref ValueArrayBuilder<byte> builder, in TKey key)
        {
            GuidSerializer.Serialize(guid, ref builder);
            keySerializer.Serialize(key, ref builder);

            return builder.AsSpan();
        }

        private ReadOnlySpan<byte> GetValueBytes(ref ValueArrayBuilder<byte> builder, in TValue value)
        {
            valueSerializer.Serialize(value, ref builder);

            return builder.AsSpan();
        }

        public bool TryGet(in TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            var builder = new ValueArrayBuilder<byte>(stackalloc byte[GetKeyStackallocSize(key)]);

            try
            {
                if (db.Get(GetPrefixedKeyBytes(ref builder, key), valueSerializer) is { HasValue: true, MaybeValue: var v })
                {
                    value = v;
                    return true;
                }

            }
            finally
            {
                builder.Dispose();
            }
            
            value = default;
            return false;
        }

        public void Upsert(in TKey key, in TValue value)
        {
            var keyBuilder = new ValueArrayBuilder<byte>(stackalloc byte[GetKeyStackallocSize(key)]);
            var valueBuilder = new ValueArrayBuilder<byte>(stackalloc byte[Math.Min(keySerializer.SizeHint(key) ?? DefaultStackallocSize, MaxStackallocSize)]);

            try
            {
                db.Put(GetPrefixedKeyBytes(ref keyBuilder, key), GetValueBytes(ref valueBuilder, value));
            }
            finally
            {
                keyBuilder.Dispose();
                valueBuilder.Dispose();
            }
        }

        public bool Delete(in TKey key)
        {
            var builder = new ValueArrayBuilder<byte>(stackalloc byte[GetKeyStackallocSize(key)]);

            try
            {
                var keySpan = GetPrefixedKeyBytes(ref builder, key);
                
                // :(
                if (db.HasKey(keySpan))
                {
                    db.Remove(keySpan);
                    return true;
                }

                return false;
            }
            finally
            {
                builder.Dispose();
            }
        }
        
        public void Dispose()
        {
        }
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}