using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO.Hashing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using FlatSharp;
using Maxine.Extensions.Streams;
using MessagePack;
using RocksDbSharp;

namespace Maxine.Extensions.KeyValue;

internal interface IMaxineSerializer<T> : ISpanDeserializer<Optional<T>>
{
    int? SizeHint(in T entry) => null;
    bool TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out T entry, out int bytesRead);
    void Serialize(in T entry, ref ValueArrayBuilder<byte> builder);
    
    Optional<T> ISpanDeserializer<Optional<T>>.Deserialize(ReadOnlySpan<byte> buffer)
    {
        if (!TryDeserialize(buffer, out var result, out _))
        {
            throw new InvalidOperationException($"Could not deserialize {typeof(T)}");
        }

        return new Optional<T>(true, result);
    }
}

internal record struct Optional<T>(bool HasValue, T MaybeValue);

internal interface IMaxineSerializer<T, TSerializer> : IMaxineSerializer<T>
    where TSerializer : IMaxineSerializer<T, TSerializer> 
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int? IMaxineSerializer<T>.SizeHint(in T entry) => TSerializer.SizeHint(in entry);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IMaxineSerializer<T>.TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out T entry, out int bytesRead) => TSerializer.TryDeserialize(bytes, out entry, out bytesRead);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IMaxineSerializer<T>.Serialize(in T entry, ref ValueArrayBuilder<byte> builder) => TSerializer.Serialize(in entry, ref builder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    new static virtual int? SizeHint(in T entry) => null;
    new static abstract bool TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out T entry, out int bytesRead);
    new static abstract void Serialize(in T entry, ref ValueArrayBuilder<byte> builder);
}

internal static class Helpers
{
    public const string TablesConstantName = "__tables";

    private static readonly ConcurrentDictionary<Type, object> Cache = new();

    public static IMaxineSerializer<T> GetSerializer<T>(ProviderType providerType)
    {
        return providerType switch
        {
            ProviderType.Default => (IMaxineSerializer<T>)Cache.GetOrAdd(
                typeof(T),
                static _ => Activator.CreateInstance(GetSerializerType(typeof(T)))!
            ),
            ProviderType.Msgpack => new MessagePackSerializer<T>(),
            ProviderType.FlatBuffers => (IMaxineSerializer<T>)Cache.GetOrAdd(
                typeof(Tenray.ZoneTree.Serializers.ISerializer<T>),
                static _ => Activator.CreateInstance(typeof(FlatbufferSerializer<>).MakeGenericType(typeof(T)))!
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Type GetSerializerType(Type type)
    {
        if (type == typeof(byte[])) return typeof(MaxineByteArraySerializer);
        if (type == typeof(string)) return typeof(MaxineUtf8StringSerializer);
        if (type == typeof(Guid)) return typeof(GuidSerializer);

        if (type.IsConstructedGenericType &&
            type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
            type.GetGenericArguments()[0] is var nullableValue &&
            IsBlittable(nullableValue))
        {
            return typeof(NullableSerializer<,>).MakeGenericType(nullableValue, GetSerializerType(nullableValue));
        }

        if (IsBlittable(type))
            return typeof(BlittableSerializer<>).MakeGenericType(type);

        if (type.IsArray)
        {
            var rank = type.GetArrayRank();
            if (rank == 1)
            {
                var elementType = type.GetElementType()!;
                if (IsBlittable(elementType))
                {
                    return typeof(BlittableArraySerializer<>).MakeGenericType(elementType);
                }
                else
                {
                    return typeof(NonBlittableArraySerializer<,>).MakeGenericType(elementType, GetSerializerType(elementType));
                }
            }
        }

        return typeof(MessagePackSerializer<>).MakeGenericType(type);
    }

    private static readonly MethodInfo IsReferenceOrContainsReferences = typeof(RuntimeHelpers)
        .GetMethod(nameof(RuntimeHelpers.IsReferenceOrContainsReferences))!;

    private static readonly ConcurrentDictionary<Type, bool> IsBlittableCache = new();

    private static bool IsBlittable(Type type)
    {
        return IsBlittableCache.GetOrAdd(
            type,
            static type => !(bool)IsReferenceOrContainsReferences.MakeGenericMethod(type).Invoke(null, null)!
        );
    }

    // will not work on big endian
    public static Guid GetGuidFromName(string name)
        => Unsafe.BitCast<UInt128, Guid>(XxHash128.HashToUInt128(MemoryMarshal.AsBytes(name.AsSpan())));
}

internal sealed class FlatbufferSerializer<T> : IMaxineSerializer<T, FlatbufferSerializer<T>> where T : class, IFlatBufferSerializable<T>
{
    public static int? SizeHint(in T entry) => T.LazySerializer.GetMaxSize(entry);

    public static unsafe bool TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out T entry, out int bytesRead)
    {
        // https://github.com/MessagePack-CSharp/MessagePack-CSharp/issues/81#issuecomment-906627972
        fixed (byte* pointer = &bytes.GetPinnableReference())
        {
            // need to use Greedy otherwise it will want to keep a reference to the underlying memory which we release here.
            using var memoryManager = new UnmanagedMemoryManager<byte>(pointer, bytes.Length);
            entry = T.GreedyMutableSerializer.Parse(new MemoryInputBuffer(memoryManager.Memory, true));
            bytesRead = bytes.Length;
            return true;
        }
    }

    public static void Serialize(in T entry, ref ValueArrayBuilder<byte> builder)
    {
        var maxSize = T.LazySerializer.GetMaxSize(entry);
        var span = builder.GetSpan(maxSize)[..maxSize];

        var bytesWritten = T.GreedyMutableSerializer.Write(default(SpanWriter), span, entry);
        builder.Advance(bytesWritten);
    }
}

internal sealed class BlittableArraySerializer<T> : IMaxineSerializer<T[], BlittableArraySerializer<T>> where T : unmanaged
{
    public static unsafe int? SizeHint(in T[] entry) => sizeof(int) + (sizeof(T) * entry.Length);

    public static unsafe bool TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out T[] entry, out int bytesRead)
    {
        var reader = new SpanReader(bytes);

        if (!reader.TryReadInt32(out var length))
        {
            bytesRead = 0;
            entry = default;
            return false;
        }

        if (bytes.Length < (sizeof(T) * length) + sizeof(int))
        {
            entry = default;
            bytesRead = sizeof(int);
            return false;
        }

        entry = new T[length];

        for (var i = 0; i < length; i++)
        {
            if (!BlittableSerializer<T>.TryDeserialize(reader.ReadSpan(sizeof(T)), out entry[i], out var tempRead))
            {
                bytesRead = reader.Position + tempRead;
                return false;
            }
        }

        bytesRead = reader.Position;
        return true;
    }

    public static void Serialize(in T[] entry, ref ValueArrayBuilder<byte> builder)
    {
        BinaryPrimitives.WriteInt32LittleEndian(builder.AppendSpan(sizeof(int)), entry.Length);

        foreach (var t in entry)
        {
            BlittableSerializer<T>.Serialize(t, ref builder);
        }
    }
}

internal sealed class NonBlittableArraySerializer<T, TInnerSerializer> : IMaxineSerializer<T[], NonBlittableArraySerializer<T, TInnerSerializer>>
    where TInnerSerializer : IMaxineSerializer<T, TInnerSerializer>
{
    public static int? SizeHint(in T[] entry)
    {
        var x = sizeof(int);

        foreach (var e in entry)
        {
            x += sizeof(int);
            if (TInnerSerializer.SizeHint(e) is { } size) x += size;
        }

        return x;
    }

    public static bool TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out T[] entry, out int bytesRead)
    {
        var reader = new SpanReader(bytes);

        if (!reader.TryReadInt32(out var length))
        {
            entry = default;
            bytesRead = 0;
            return false;
        }

        entry = new T[length];

        for (var i = 0; i < length; i++)
        {
            if (!reader.TryReadInt32(out var elementLength))
            {
                entry = default;
                bytesRead = reader.Position;
                return false;
            }
            
            if (!TInnerSerializer.TryDeserialize(reader.GetRemainingBuffer()[..elementLength], out entry[i], out var tempRead))
            {
                bytesRead = reader.Position + tempRead;
                return false;
            }

            reader.Advance(tempRead);
        }

        bytesRead = reader.Position;
        return true;
    }

    public static void Serialize(in T[] entry, ref ValueArrayBuilder<byte> builder)
    {
        BinaryPrimitives.WriteInt32LittleEndian(builder.AppendSpan(sizeof(int)), entry.Length);

        foreach (var t in entry)
        {
            var lengthSpan = builder.AppendSpan(sizeof(int));
            
            var posStart = builder.Length;
            TInnerSerializer.Serialize(t, ref builder);
            var posEnd = builder.Length;

            BinaryPrimitives.WriteInt32LittleEndian(lengthSpan, posEnd - posStart);
        }
    }
}

internal sealed class MaxineByteArraySerializer : IMaxineSerializer<byte[], MaxineByteArraySerializer>
{
    public static int? SizeHint(in byte[] entry) => entry.Length;

    public static bool TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out byte[] entry, out int bytesRead)
    {
        entry = bytes.ToArray();
        bytesRead = bytes.Length;
        return true;
    }

    public static void Serialize(in byte[] entry, ref ValueArrayBuilder<byte> builder)
    {
        builder.Append(entry);
    }
}

internal sealed class MessagePackSerializer<T> : IMaxineSerializer<T, MessagePackSerializer<T>>
{
    public static unsafe bool TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out T entry, out int bytesRead)
    {
        // https://github.com/MessagePack-CSharp/MessagePack-CSharp/issues/81#issuecomment-906627972
        fixed (byte* pointer = &bytes.GetPinnableReference())
        {
            using var memoryManager = new UnmanagedMemoryManager<byte>(pointer, bytes.Length);
            entry = MessagePackSerializer.Deserialize<T>(memoryManager.Memory, out bytesRead);
            return true;
        }
    }

    public static void Serialize(in T entry, ref ValueArrayBuilder<byte> builder)
    {
        // MessagePackSerializer has its own thread-local cache, but this does result in an allocation for the final
        // array
        builder.Append(MessagePackSerializer.Serialize(entry));
    }
}

internal sealed unsafe class BlittableSerializer<T> : IMaxineSerializer<T, BlittableSerializer<T>> where T : unmanaged
{
    public static int? SizeHint(in T entry) => sizeof(T);
    
    static BlittableSerializer()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            throw new InvalidOperationException($"{typeof(T)} IsReferenceOrContainsReferences");
        }
    }

    public static bool TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out T entry, out int bytesRead)
    {
        if (sizeof(T) > bytes.Length)
        {
            entry = default;
            bytesRead = 0;
            return false;
        }

        entry = MemoryMarshal.Cast<byte, T>(bytes[..sizeof(T)])[0];
        bytesRead = sizeof(T);
        return true;
    }

    public static void Serialize(in T entry, ref ValueArrayBuilder<byte> builder)
    {
        var span = builder.AppendSpan(sizeof(T));
        MemoryMarshal.Cast<byte, T>(span)[0] = entry;
    }
}

internal sealed class MaxineUtf8StringSerializer : IMaxineSerializer<string?, MaxineUtf8StringSerializer>
{
    // Single byte 0xC2 is not a valid UTF-8 string.
    // We can use that to serialize the null strings.
    private const byte NullMarker = 0xC2;

    public static int? SizeHint(in string? value) => value == null ? 1 : Encoding.UTF8.GetByteCount(value);

    public static bool TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out string? entry, out int bytesRead)
    {
        if (bytes is [NullMarker])
        {
            entry = null;
            bytesRead = 1;
            return true;
        }

        entry = Encoding.UTF8.GetString(bytes);
        bytesRead = bytes.Length;
        return true;
    }

    public static void Serialize(in string? entry, ref ValueArrayBuilder<byte> builder)
    {
        if (entry == null)
        {
            builder.Append(NullMarker);
        }
        else
        {
            builder.Append(Encoding.UTF8.GetBytes(entry));
        }
    }
}

internal sealed class NullableSerializer<T, TInternalSerializer>
    : IMaxineSerializer<T?, NullableSerializer<T, TInternalSerializer>>
    where T : struct
    where TInternalSerializer : IMaxineSerializer<T, TInternalSerializer>
{
    public static int? SizeHint(in T? value) => value.HasValue ? TInternalSerializer.SizeHint(value.Value) : 0;

    public static bool TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out T? entry, out int bytesRead)
    {
        if (bytes.Length == 0)
        {
            entry = null;
            bytesRead = 0;
            return true;
        }

        if (TInternalSerializer.TryDeserialize(bytes, out var entryValue, out bytesRead))
        {
            entry = entryValue;
            return true;
        }

        entry = default;
        return false;
    }

    public static void Serialize(in T? entry, ref ValueArrayBuilder<byte> builder)
    {
        if (entry.HasValue)
            TInternalSerializer.Serialize(entry.Value, ref builder);
    }
}

internal sealed class GuidSerializer : IMaxineSerializer<Guid, GuidSerializer>
{
    public const int GuidLength = 16;

    public static int? SizeHint(in Guid value) => GuidLength;
    
    public static bool TryDeserialize(ReadOnlySpan<byte> bytes, [MaybeNullWhen(false)] out Guid entry, out int bytesRead)
    {
        if (bytes.Length < GuidLength)
        {
            entry = default;
            bytesRead = 0;
            return false;
        }
        
        entry = new Guid(bytes[..GuidLength]);
        bytesRead = GuidLength;
        return true;
    }

    public static void Serialize(in Guid entry, ref ValueArrayBuilder<byte> builder)
    {
        var span = builder.AppendSpan(GuidLength);
        if (!entry.TryWriteBytes(span))
        {
            throw new InvalidOperationException("Should never happen!");
        }
    }
}
