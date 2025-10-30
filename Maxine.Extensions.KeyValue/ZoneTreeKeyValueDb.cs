// using System.Buffers;
// using System.Diagnostics.CodeAnalysis;
// using System.Globalization;
// using System.IO.Hashing;
// using System.Runtime.CompilerServices;
// using System.Runtime.InteropServices;
// using System.Text;
// using MessagePack;
// using Tenray.ZoneTree;
// using Tenray.ZoneTree.Comparers;
// using Tenray.ZoneTree.Options;
// using Tenray.ZoneTree.PresetTypes;
// using Tenray.ZoneTree.Serializers;
//
// namespace Maxine.Extensions.KeyValue;
//
// public class ZoneTreeKeyValueDb(string rootPath) : IKeyValueDb
// {
//     private class MemoryComparerAscending : IRefComparer<ReadOnlyMemory<byte>>
//     {
//         public int Compare(in ReadOnlyMemory<byte> x, in ReadOnlyMemory<byte> y)
//         {
//             return x.Span.SequenceCompareTo(y.Span);
//         }
//     }
//
//     private interface IBufferSerializer<T> : ISerializer<T>
//     {
//         void Serialize(in IBufferWriter<byte> span, in T value);
//
//         Memory<byte> ISerializer<T>.Serialize(in T entry)
//         {
//             var writer = new ArrayBufferWriter<byte>(16);
//             Serialize(writer, entry);
//             return writer.WrittenSpan.ToArray();
//         }
//     }
//
//     private interface ISerializerWithLength<T> : IBufferSerializer<T>
//     {
//         int Length(in T value);
//
//         bool TrySerialize(in Span<byte> span, in T value);
//
//         void IBufferSerializer<T>.Serialize(in IBufferWriter<byte> span, in T value)
//         {
//             var length = Length(value);
//             if (!TrySerialize(span.GetSpan(length)[..length], value))
//             {
//                 throw new InvalidOperationException();
//             }
//             span.Advance(length);
//         }
//
//         Memory<byte> ISerializer<T>.Serialize(in T entry)
//         {
//             var array = new byte[Length(entry)];
//             
//             if (!TrySerialize(array, entry))
//             {
//                 throw new InvalidOperationException();
//             }
//
//             return array;
//         }
//     }
//     
//     private class MemorySerializer : ISerializer<ReadOnlyMemory<byte>>, ISerializer<Memory<byte>>
//     {
//         public ReadOnlyMemory<byte> Deserialize(Memory<byte> bytes)
//         {
//             return bytes;
//         }
//
//         public Memory<byte> Serialize(in Memory<byte> entry)
//         {
//             return entry;
//         }
//
//         public Memory<byte> Serialize(in ReadOnlyMemory<byte> entry)
//         {
//             return entry.ToArray();
//         }
//
//         Memory<byte> ISerializer<Memory<byte>>.Deserialize(Memory<byte> bytes)
//         {
//             return bytes;
//         }
//     }
//
//     private readonly IZoneTree<ReadOnlyMemory<byte>, Memory<byte>> _zoneTree = new ZoneTreeFactory<ReadOnlyMemory<byte>, Memory<byte>>()
//         .SetComparer(new MemoryComparerAscending())
//         .SetDataDirectory(rootPath)
//         .SetKeySerializer(new MemorySerializer())
//         .SetValueSerializer(new MemorySerializer())
//         .SetIsDeletedDelegate((in ReadOnlyMemory<byte> _, in Memory<byte> value) => value.Span is [1] or [1, ..])
//         .SetMarkValueDeletedDelegate((ref Memory<byte> value) => value = (byte[])[1])
//         .ConfigureWriteAheadLogOptions(e =>
//         {
//             e.WriteAheadLogMode = WriteAheadLogMode.Sync;
//         })
//         .ConfigureDiskSegmentOptions(e =>
//         {
//             e.CompressionMethod = CompressionMethod.LZ4;
//             e.CompressionLevel = CompressionLevels.LZ4Fastest;
//         })
//         .OpenOrCreate();
//
//     public IKeyValues<TKey, TValue> Table<TKey, TValue>(string name)
//     {
//         var guid = Unsafe.BitCast<UInt128, Guid>(XxHash128.HashToUInt128(MemoryMarshal.AsBytes(name.AsSpan())));
//         
//         return new KeyValues<TKey, TValue>(
//             _zoneTree,
//             guid,
//             GetSerializer<TKey>(),
//             GetSerializer<TValue>()
//         );
//     }
//     
//     #region Comparers
//     
//     private static IRefComparer<T> GetComparer<T>(bool ascending, StringComparison? comparison = StringComparison.Ordinal, CultureInfo? cultureInfo = null, bool ignoreCase = false)
//     {
//         if (typeof(T) == typeof(bool)) return (IRefComparer<T>)(object)(ascending ? new ComparableComparerAscending<bool>() : new ComparableComparerDescending<bool>());
//         if (typeof(T) == typeof(byte[])) return (IRefComparer<T>)(object)(ascending ? new ByteArrayComparerAscending() : new ByteArrayComparerDescending());
//         if (typeof(T) == typeof(byte)) return (IRefComparer<T>)(object)(ascending ? new ByteComparerAscending() : new ComparableComparerDescending<byte>());
//         if (typeof(T) == typeof(char)) return (IRefComparer<T>)(object)(ascending ? new CharComparerAscending() : new ComparableComparerDescending<char>());
//         if (typeof(T) == typeof(DateTime)) return (IRefComparer<T>)(object)(ascending ? new DateTimeComparerAscending() : new DateTimeComparerDescending());
//         if (typeof(T) == typeof(decimal)) return (IRefComparer<T>)(object)(ascending ? new DecimalComparerAscending() : new DecimalComparerDescending());
//         if (typeof(T) == typeof(short)) return (IRefComparer<T>)(object)(ascending ? new Int16ComparerAscending() : new Int16ComparerDescending());
//         if (typeof(T) == typeof(int)) return (IRefComparer<T>)(object)(ascending ? new Int32ComparerAscending() : new Int32ComparerDescending());
//         if (typeof(T) == typeof(long)) return (IRefComparer<T>)(object)(ascending ? new Int64ComparerAscending() : new Int64ComparerDescending());
//         if (typeof(T) == typeof(ushort)) return (IRefComparer<T>)(object)(ascending ? new UInt16ComparerAscending() : new UInt16ComparerDescending());
//         if (typeof(T) == typeof(uint)) return (IRefComparer<T>)(object)(ascending ? new UInt32ComparerAscending() : new UInt32ComparerDescending());
//         if (typeof(T) == typeof(ulong)) return (IRefComparer<T>)(object)(ascending ? new UInt64ComparerAscending() : new UInt64ComparerDescending());
//         if (typeof(T) == typeof(double)) return (IRefComparer<T>)(object)(ascending ? new DoubleComparerAscending() : new DoubleComparerDescending());
//         if (typeof(T) == typeof(float)) return (IRefComparer<T>)(object)(ascending ? new ComparableComparerAscending<float>() : new ComparableComparerDescending<float>());
//         if (typeof(T) == typeof(Guid)) return (IRefComparer<T>)(object)(ascending ? new GuidComparerAscending() : new ComparableComparerDescending<Guid>());
//
//         if (typeof(T) == typeof(string))
//         {
//             switch (comparison)
//             {
//                 case StringComparison.CurrentCultureIgnoreCase:
//                 case StringComparison.CurrentCulture when ignoreCase:
//                     return (IRefComparer<T>)(object)(ascending ? new StringCurrentCultureIgnoreCaseComparerAscending() : new StringCurrentCultureIgnoreCaseComparerDescending());
//                 case StringComparison.InvariantCultureIgnoreCase:
//                 case StringComparison.InvariantCulture when ignoreCase:
//                     return (IRefComparer<T>)(object)(ascending ? new StringInvariantIgnoreCaseComparerAscending() : new StringInvariantIgnoreCaseComparerDescending());
//                 case StringComparison.OrdinalIgnoreCase:
//                 case StringComparison.Ordinal when ignoreCase:
//                     return (IRefComparer<T>)(object)(ascending ? new StringOrdinalIgnoreCaseComparerAscending() : new StringOrdinalIgnoreCaseComparerDescending());
//                 case StringComparison.CurrentCulture:
//                     return (IRefComparer<T>)(object)(ascending ? new StringCurrentCultureComparerAscending() : new StringCurrentCultureComparerDescending());
//                 case StringComparison.InvariantCulture:
//                     return (IRefComparer<T>)(object)(ascending ? new StringInvariantComparerAscending() : new StringInvariantComparerDescending());
//                 case StringComparison.Ordinal:
//                     return (IRefComparer<T>)(object)(ascending ? new StringOrdinalComparerAscending() : new StringOrdinalComparerDescending());
//                 default:
//                     if (cultureInfo is not null)
//                         return (IRefComparer<T>)(object)(ascending ? new StringSpecificCultureComparerAscending(cultureInfo, ignoreCase) : new StringSpecificCultureComparerDescending(cultureInfo, ignoreCase));
//                     throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null);
//             }
//         }
//
//         if (typeof(T).IsAssignableTo(typeof(IComparable<T>)))
//             return ascending
//                 ? AscendingComparableComparerHolder<T>.Comparer
//                 : DescendingComparableComparerHolder<T>.Comparer;
//         
//         if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) return ascending ? new BlittableComparerAscending<T>() : new BlittableComparerDescending<T>();
//
//         throw new InvalidOperationException($"Could not get comparer for {typeof(T)}");
//     }
//
//     private sealed class BlittableComparerAscending<T> : IRefComparer<T>
//     {
//         static BlittableComparerAscending()
//         {
//             if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
//             {
//                 throw new InvalidOperationException($"{typeof(T)} IsReferenceOrContainsReferences");
//             }
//         }
//
//         public int Compare(in T x, in T y)
//         {
//             unsafe
//             {
// #pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
//                 var xspan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in x)), sizeof(T));
//                 var yspan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in y)), sizeof(T));
// #pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
//
//                 return xspan.SequenceCompareTo(yspan);
//             }
//         }
//     }
//
//     private sealed class BlittableComparerDescending<T> : IRefComparer<T>
//     {
//         static BlittableComparerDescending()
//         {
//             if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
//             {
//                 throw new InvalidOperationException($"{typeof(T)} IsReferenceOrContainsReferences");
//             }
//         }
//
//         public int Compare(in T x, in T y)
//         {
//             unsafe
//             {
// #pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
//                 var xspan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in x)), sizeof(T));
//                 var yspan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in y)), sizeof(T));
// #pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
//
//                 return yspan.SequenceCompareTo(xspan);
//             }
//         }
//     }
//
//     private sealed class StringSpecificCultureComparerAscending(CultureInfo culture, bool ignoreCase)
//         : IRefComparer<string>
//     {
//         public StringSpecificCultureComparerAscending(string culture, bool ignoreCase) : this(CultureInfo.GetCultureInfo(culture), ignoreCase)
//         {
//         }
//
//         public int Compare(in string x, in string y)
//         {
//             return string.Compare(x, y, ignoreCase, culture);
//         }
//     }
//
//     private sealed class StringSpecificCultureComparerDescending(CultureInfo culture, bool ignoreCase)
//         : IRefComparer<string>
//     {
//         public StringSpecificCultureComparerDescending(string culture, bool ignoreCase) : this(CultureInfo.GetCultureInfo(culture), ignoreCase)
//         {
//         }
//
//         public int Compare(in string x, in string y)
//         {
//             return string.Compare(y, x, ignoreCase, culture);
//         }
//     }
//
//     private sealed class StringOrdinalIgnoreCaseComparerDescending : IRefComparer<string>
//     {
//         public int Compare(in string x, in string y)
//         {
//             return string.Compare(y, x, StringComparison.OrdinalIgnoreCase);
//         }
//     }
//
//     private sealed class StringInvariantComparerDescending : IRefComparer<string>
//     {
//         public int Compare(in string x, in string y)
//         {
//             return string.Compare(y, x, StringComparison.InvariantCulture);
//         }
//     }
//     
//     private sealed class StringInvariantIgnoreCaseComparerDescending : IRefComparer<string>
//     {
//         public int Compare(in string x, in string y)
//         {
//             return string.Compare(y, x, StringComparison.InvariantCultureIgnoreCase);
//         }
//     }
//     
//     private sealed class StringCurrentCultureIgnoreCaseComparerDescending : IRefComparer<string>
//     {
//         public int Compare(in string x, in string y)
//         {
//             return string.Compare(y, x, StringComparison.CurrentCultureIgnoreCase);
//         }
//     }
//
//     private sealed class StringCurrentCultureComparerDescending : IRefComparer<string>
//     {
//         public int Compare(in string x, in string y)
//         {
//             return string.Compare(y, x, StringComparison.CurrentCulture);
//         }
//     }
//
//     private static class AscendingComparableComparerHolder<T>
//     {
//         static AscendingComparableComparerHolder()
//         {
//             if (!typeof(T).IsAssignableTo(typeof(IComparable<T>)))
//                 throw new InvalidOperationException();
//         }
//         
//         public static IRefComparer<T> Comparer { get; } = (IRefComparer<T>)Activator.CreateInstance(typeof(ComparableComparerAscending<>).MakeGenericType(typeof(T)))!;
//     }
//
//     private static class DescendingComparableComparerHolder<T>
//     {
//         static DescendingComparableComparerHolder()
//         {
//             if (!typeof(T).IsAssignableTo(typeof(IComparable<T>)))
//                 throw new InvalidOperationException();
//         }
//         
//         public static IRefComparer<T> Comparer { get; } = (IRefComparer<T>)Activator.CreateInstance(typeof(ComparableComparerDescending<>).MakeGenericType(typeof(T)))!;
//     }
//
//     private sealed class ComparableComparerDescending<T> : IRefComparer<T> where T : IComparable<T>
//     {
//         public int Compare(in T x, in T y)
//         {
//             return y.CompareTo(x);
//         }
//     }
//     
//     private sealed class ComparableComparerAscending<T> : IRefComparer<T> where T : IComparable<T>
//     {
//         public int Compare(in T x, in T y)
//         {
//             return x.CompareTo(y);
//         }
//     }
//     #endregion
//
//     private static ISerializer<T> GetSerializer<T>()
//     {
//         if (typeof(T) == typeof(byte[])) return (ISerializer<T>)(object)new MaxineByteArraySerializer();
//         if (typeof(T) == typeof(bool)) return (ISerializer<T>)(object)new BlittableSerializer<bool>();
//         if (typeof(T) == typeof(byte)) return (ISerializer<T>)(object)new BlittableSerializer<byte>();
//         if (typeof(T) == typeof(char)) return (ISerializer<T>)(object)new BlittableSerializer<char>();
//         if (typeof(T) == typeof(DateTime)) return (ISerializer<T>)(object)new BlittableSerializer<DateTime>();
//         if (typeof(T) == typeof(decimal)) return (ISerializer<T>)(object)new BlittableSerializer<decimal>();
//         if (typeof(T) == typeof(short)) return (ISerializer<T>)(object)new BlittableSerializer<short>();
//         if (typeof(T) == typeof(int)) return (ISerializer<T>)(object)new BlittableSerializer<int>();
//         if (typeof(T) == typeof(long)) return (ISerializer<T>)(object)new BlittableSerializer<long>();
//         if (typeof(T) == typeof(ushort)) return (ISerializer<T>)(object)new BlittableSerializer<ushort>();
//         if (typeof(T) == typeof(uint)) return (ISerializer<T>)(object)new BlittableSerializer<uint>();
//         if (typeof(T) == typeof(ulong)) return (ISerializer<T>)(object)new BlittableSerializer<ulong>();
//         if (typeof(T) == typeof(double)) return (ISerializer<T>)(object)new BlittableSerializer<double>();
//         if (typeof(T) == typeof(float)) return (ISerializer<T>)(object)new BlittableSerializer<float>();
//         
//         if (typeof(T) == typeof(bool?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<bool>(new BlittableSerializer<bool>());
//         if (typeof(T) == typeof(byte?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<byte>(new BlittableSerializer<byte>());
//         if (typeof(T) == typeof(char?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<char>(new BlittableSerializer<char>());
//         if (typeof(T) == typeof(DateTime?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<DateTime>(new BlittableSerializer<DateTime>());
//         if (typeof(T) == typeof(decimal?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<decimal>(new BlittableSerializer<decimal>());
//         if (typeof(T) == typeof(short?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<short>(new BlittableSerializer<short>());
//         if (typeof(T) == typeof(int?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<int>(new BlittableSerializer<int>());
//         if (typeof(T) == typeof(long?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<long>(new BlittableSerializer<long>());
//         if (typeof(T) == typeof(ushort?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<ushort>(new BlittableSerializer<ushort>());
//         if (typeof(T) == typeof(uint?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<uint>(new BlittableSerializer<uint>());
//         if (typeof(T) == typeof(ulong?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<ulong>(new BlittableSerializer<ulong>());
//         if (typeof(T) == typeof(double?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<double>(new BlittableSerializer<double>());
//         if (typeof(T) == typeof(float?)) return (ISerializer<T>)(object)new NullableSerializerWithLength<float>(new BlittableSerializer<float>());
//
//         if (typeof(T) == typeof(string)) return (ISerializer<T>)(object)new MaxineUtf8StringSerializer();
//
//         if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) return new BlittableSerializer<T>();
//
//         return new MessagePackSerializer<T>();
//     }
//
//     private class MaxineByteArraySerializer : ISerializerWithLength<byte[]>
//     {
//         public int Length(in byte[] value)
//         {
//             return value.Length;
//         }
//
//         public byte[] Deserialize(Memory<byte> bytes)
//         {
//             return bytes.ToArray();
//         }
//
//         public bool TrySerialize(in Span<byte> span, in byte[] value)
//         {
//             return value.AsSpan().TryCopyTo(span);
//         }
//
//         public void Serialize(in IBufferWriter<byte> span, in byte[] value)
//         {
//             span.Write(value);
//         }
//
//         public Memory<byte> Serialize(in byte[] entry)
//         {
//             return entry;
//         }
//     }
//
//     private static byte[] DeletedBytes { get; } = [1];
//
//     private sealed class DeletableRefSerializer<T>(ISerializer<T> valueSerializer)
//         : ISerializer<Deletable<T>>
//     {
//         public Deletable<T> Deserialize(Memory<byte> bytes)
//         {
//             if (bytes.Span is [1])
//             {
//                 return new Deletable<T>(default!, true);
//             }
//
//             if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
//             {
//                 unsafe
//                 {
// #pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
//                     if (sizeof(T) > bytes.Length - 1)
// #pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
//                     {
//                         throw new InvalidOperationException();
//                     }
//
//                     return new Deletable<T>(Unsafe.ReadUnaligned<T>(ref Unsafe.AsRef(ref bytes.Span[1])));
//                 }
//             }
//
//             return new Deletable<T>(valueSerializer.Deserialize(bytes[1..]));
//         }
//
//         public Memory<byte> Serialize(in Deletable<T> entry)
//         {
//             if (entry.IsDeleted) return DeletedBytes;
//
//             if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
//             {
//                 unsafe
//                 {
// #pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
//                     var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref Unsafe.AsRef(in entry.Value)), sizeof(T));
// #pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
//                     return (byte[])[0, ..span];
//                 }
//             }
//
//             var b1 = valueSerializer.Serialize(entry.Value);
//             return (byte[])[0, ..b1.Span];
//         }
//     }
//
//     private sealed class MessagePackSerializer<T> : IBufferSerializer<T>
//     {
//         public void Serialize(in IBufferWriter<byte> writer, in T value)
//         {
//             MessagePackSerializer.Serialize(writer, value);
//         }
//
//         public T Deserialize(Memory<byte> bytes)
//         {
//             return MessagePackSerializer.Deserialize<T>(bytes);
//         }
//
//         public Memory<byte> Serialize(in T entry)
//         {
//             return MessagePackSerializer.Serialize(entry);
//         }
//     }
//
// #pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
//     private sealed unsafe class BlittableSerializer<T> : ISerializerWithLength<T>
//     {
//         public int Length(in T value) => sizeof(T);
//         
//         static BlittableSerializer()
//         {
//             if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
//             {
//                 throw new InvalidOperationException($"{typeof(T)} IsReferenceOrContainsReferences");
//             }
//         }
//
//         public bool TrySerialize(in Span<byte> span, in T value)
//         {
//             if (sizeof(T) > span.Length)
//             {
//                 return false;
//             }
//             
//             Unsafe.WriteUnaligned(ref span[0], value);
//             return true;
//         }
//
//         public T Deserialize(Memory<byte> bytes)
//         {
//             if (sizeof(T) > bytes.Length)
//             {
//                 throw new InvalidOperationException();
//             }
//
//             return Unsafe.ReadUnaligned<T>(ref bytes.Span[0]);
//         }
//
//         public Memory<byte> Serialize(in T entry)
//         {
//             var result = new byte[sizeof(T)];
//             Unsafe.WriteUnaligned(ref result[0], entry);
//             return result;
//         }
//     }
// #pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
//
//     private sealed class MaxineUtf8StringSerializer : ISerializerWithLength<string?>
//     {
//         // Single byte 0xC2 is not a valid UTF-8 string.
//         // We can use that to serialize the null strings.
//         private const byte NullMarker = 0xC2;
//
//         private static byte[] NullMarkerArray { get; } = [NullMarker];
//
//         public int Length(in string? value) => value == null ? 1 : Encoding.UTF8.GetByteCount(value);
//
//         public bool TrySerialize(in Span<byte> span, in string? value)
//         {
//             return Encoding.UTF8.TryGetBytes(value, span, out var bytesWritten);
//         }
//
//         public string? Deserialize(Memory<byte> bytes)
//         {
//             if (bytes.Length == 1 && bytes.Span[0] == 0xC2)
//                 return null;
//             return Encoding.UTF8.GetString(bytes.Span);
//         }
//
//         public Memory<byte> Serialize(in string? entry)
//         {
//             if (entry == null)
//             {
//                 return NullMarkerArray;
//             }
//             return Encoding.UTF8.GetBytes(entry);
//         }
//     }
//
//     private sealed class NullableSerializerWithLength<T>(ISerializerWithLength<T> internalSerializer)
//         : ISerializerWithLength<T?> where T : struct
//     {
//         public int Length(in T? value) => value.HasValue ? internalSerializer.Length(value.Value) : 0;
//
//         public bool TrySerialize(in Span<byte> span, in T? value)
//         {
//             if (value.HasValue)
//             {
//                 return internalSerializer.TrySerialize(span, value.Value);
//             }
//
//             return true;
//         }
//
//         public void Serialize(in IBufferWriter<byte> span, in T? value)
//         {
//             if (value.HasValue)
//             {
//                 internalSerializer.Serialize(span, value.Value);
//             }
//         }
//
//         public T? Deserialize(Memory<byte> bytes)
//         {
//             if (bytes.Length == 0)
//                 return null;
//             return internalSerializer.Deserialize(bytes);
//         }
//
//         public Memory<byte> Serialize(in T? entry)
//         {
//             return entry.HasValue ? internalSerializer.Serialize(entry.Value) : Memory<byte>.Empty;
//         }
//     }
//
//     private sealed class NullableSerializer<T>(ISerializer<T> internalSerializer)
//         : IBufferSerializer<T?> where T : struct
//     {
//         public void Serialize(in IBufferWriter<byte> span, in T? value)
//         {
//             if (value.HasValue)
//             {
//                 if (internalSerializer is IBufferSerializer<T> bufferSerializer)
//                 {
//                     bufferSerializer.Serialize(span, value.Value);
//                 }
//                 else
//                 {
//                     span.Write(internalSerializer.Serialize(value.Value).Span);
//                 }
//             }
//         }
//
//         public T? Deserialize(Memory<byte> bytes)
//         {
//             if (bytes.Length == 0)
//                 return null;
//             return internalSerializer.Deserialize(bytes);
//         }
//
//         public Memory<byte> Serialize(in T? entry)
//         {
//             return entry.HasValue ? internalSerializer.Serialize(entry.Value) : Memory<byte>.Empty;
//         }
//     }
//
//     private sealed class GuidSerializer : ISerializerWithLength<Guid>
//     {
//         private const int GuidLength = 16;
//
//         public int Length(in Guid value) => GuidLength;
//
//         public Guid Deserialize(Memory<byte> bytes) => new(bytes.Span[..GuidLength]);
//
//         public bool TrySerialize(in Span<byte> span, in Guid value)
//         {
//             return value.TryWriteBytes(span[..GuidLength]);
//         }
//     }
//
//     private sealed class CombinedSerializer<TPrefix, TSuffix, TSerializerPrefix>(TSerializerPrefix serializerPrefix, ISerializer<TSuffix> serializerSuffix)
//         where TSerializerPrefix : ISerializerWithLength<TPrefix>
//     {
//         private readonly TSerializerPrefix _serializerPrefix = serializerPrefix;
//
//         public ReadOnlyMemory<byte> Serialize(in TPrefix prefix, in TSuffix suffix)
//         {
//             var preLen = _serializerPrefix.Length(prefix);
//
//             if (serializerSuffix is ISerializerWithLength<TSuffix> serializerWithLength)
//             {
//                 var len = serializerWithLength.Length(suffix);
//                 var actualKey = new byte[preLen + len];
//
//                 // serialize prefix
//                 if (!_serializerPrefix.TrySerialize(actualKey.AsSpan(0, preLen), prefix))
//                 {
//                     throw new InvalidOperationException();
//                 }
//
//                 // serialize suffix
//                 if (!serializerWithLength.TrySerialize(actualKey.AsSpan(preLen), suffix))
//                 {
//                     throw new InvalidOperationException();
//                 }
//
//                 return actualKey;
//             }
//
//             if (serializerSuffix is IBufferSerializer<TSuffix> bufferSerializer)
//             {
//                 var arr = new ArrayBufferWriter<byte>(preLen + 16);
//
//                 // serialize prefix
//                 if (!_serializerPrefix.TrySerialize(arr.GetSpan(preLen)[..preLen], prefix))
//                 {
//                     throw new InvalidOperationException();
//                 }
//                 arr.Advance(preLen);
//
//                 // serialize suffix
//                 bufferSerializer.Serialize(arr, suffix);
//
//                 return arr.WrittenMemory;
//             }
//
//             {
//                 var keyArr = serializerSuffix.Serialize(suffix);
//                 var outArr = new byte[preLen + keyArr.Length];
//                 
//                 // serialize prefix
//                 if (!_serializerPrefix.TrySerialize(outArr.AsSpan(0, preLen), prefix))
//                 {
//                     throw new InvalidOperationException();
//                 }
//                 
//                 // serialize suffix
//                 keyArr.Span.CopyTo(outArr.AsSpan(preLen));
//
//                 return outArr;
//             }
//         }
//
//     }
//
//     private sealed class KeyValues<TKey, TValue>(IZoneTree<ReadOnlyMemory<byte>, Memory<byte>> zoneTree, Guid guid, ISerializer<TKey> keySerializer, ISerializer<TValue> valueSerializer) : IKeyValues<TKey, TValue>
//     {
//         private CombinedSerializer<Guid, TKey, GuidSerializer> keyCombinedSerializer = new(new GuidSerializer(), keySerializer);
//
//         public bool TryGet(in TKey key, [MaybeNullWhen(false)] out TValue value)
//         {
//             var keyBytes = keyCombinedSerializer.Serialize(guid, key);
//             
//             if (zoneTree.TryGet(keyBytes, out var value1) && value1.Span is not [1] and not [1, ..])
//             {
//                 value = valueSerializer.Deserialize(value1[1..]);
//                 return true;
//             }
//
//             value = default;
//             return false;
//         }
//
//         public void Upsert(in TKey key, in TValue value)
//         {
//             // zoneTree.AtomicUpsert(key, new Deletable<TValue>(value));
//         }
//
//         public bool Delete(in TKey key)
//         {
//             return false; // TODO
//             // return zoneTree.TryDelete(key, out _);
//         }
//
//         public void Dispose()
//         {
//             zoneTree.Dispose();
//         }
//     }
//
//     public void Dispose()
//     {
//         // TODO release managed resources here
//     }
// }