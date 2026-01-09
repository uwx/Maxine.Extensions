using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MessagePack;
using MessagePack.Formatters;

namespace Maxine.Extensions.MessagePack;

// https://gist.github.com/AArnott/099d5b4d559cbcca2c1c2b0bd61aa951
public static class CycleResolver
{
	public static CycleResolver<T> Create<T>(T inner) where T : IFormatterResolver
	{
		return new CycleResolver<T>(inner);
	}
}

public class CycleResolver<TResolver> : IFormatterResolver where TResolver : IFormatterResolver
{
	private const sbyte ReferenceExtensionTypeCode = 1;

	private readonly TResolver _inner;
	private readonly Dictionary<object, int> _serializedObjects = [];
	private readonly List<object?> _deserializedObjects = [];
	private readonly Dictionary<Type, IMessagePackFormatter> _dedupingFormatters = [];
	private int _serializingObjectCounter;

	internal CycleResolver(TResolver inner)
	{
		_inner = inner;
	}

	public IMessagePackFormatter<T>? GetFormatter<T>()
	{
		if (!typeof(T).IsValueType)
		{
			return GetDedupingFormatter<T>();
		}

		return _inner.GetFormatter<T>();
	}

	private IMessagePackFormatter<T>? GetDedupingFormatter<T>()
	{
		ref var formatter = ref CollectionsMarshal.GetValueRefOrAddDefault(_dedupingFormatters, typeof(T), out var exists);
		if (!exists)
		{
			formatter = new CycleFormatter<T>(this);
		}

		return (IMessagePackFormatter<T>)formatter!;
	}

	private class CycleFormatter<T> : IMessagePackFormatter<T>
	{
		private readonly CycleResolver<TResolver> _owner;

		internal CycleFormatter(CycleResolver<TResolver> owner)
		{
			_owner = owner;
		}

		public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		{
			if (!typeof(T).IsValueType && reader.TryReadNil())
			{
				return default!;
			}

			if (reader.NextMessagePackType == MessagePackType.Extension)
			{
				var provisionaryReader = reader.CreatePeekReader();
				var extensionHeader = provisionaryReader.ReadExtensionFormatHeader();
				if (extensionHeader.TypeCode == ReferenceExtensionTypeCode)
				{
					var id = provisionaryReader.ReadInt32();
					reader = provisionaryReader;
					return (T)(_owner._deserializedObjects[id] ?? throw new MessagePackSerializationException("Unexpected null element in shared object array. Dependency cycle?"));
				}
			}

			if (typeof(T) == typeof(string))
			{
				// Strings have no nested objects, so no need to clone.
				var value = _owner._inner.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
				_owner._deserializedObjects.Add(value!);
				return value!;
			}

			if (typeof(T).IsArray)
			{
				// Array cannot be created with GetUninitializedObject. Take a peek at the length first, then create
				// an array of the right size.
				
				if (typeof(T).GetArrayRank() != 1)
				{
					// Multidimensional arrays are overly complicated to handle here, so we don't allow cyclic
					// references inside them.
					
					// Reserve our position in the array.
					var placement = _owner._deserializedObjects.Count;
					_owner._deserializedObjects.Add(null);
					var value = _owner._inner.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
					_owner._deserializedObjects[placement] = value;
					return value;
				}

				{
					var provisionaryReader = reader.CreatePeekReader();
					// handle nil
					if (provisionaryReader.TryReadNil())
					{
						reader = provisionaryReader;
						return default!;
					}

					var length = provisionaryReader.ReadArrayHeader();
					var elementType = typeof(T).GetElementType()!;
					var array = Array.CreateInstance(elementType, length);
					_owner._deserializedObjects.Add(array);
					var value = _owner._inner.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
					Array.Copy((Array)(object)value!, array, length);
					return (T)(object)array;
				}
			}

			{
				var newObj = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
				_owner._deserializedObjects.Add(newObj);
				var value = _owner._inner.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
				CloneInPlace(value!, newObj!);
				return newObj;
			}
		}

		[UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "GetRawData")]
		private static extern ref byte GetRawData(
			[UnsafeAccessorType("System.Runtime.CompilerServices.RuntimeHelpers")]
			object? self,
			object obj
		);

		[UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "GetRawObjectDataSize")]
		private static extern nuint GetRawObjectDataSize(
			[UnsafeAccessorType("System.Runtime.CompilerServices.RuntimeHelpers")]
			object? self,
			object obj
		);

		[UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "BulkMoveWithWriteBarrier")]
		private static extern void BulkMoveWithWriteBarrier(
			[UnsafeAccessorType("System.Buffer")] object? self,
			ref byte destination, ref byte source, nuint byteCount
		);

		private static void CloneInPlace(object srcObj, object dstObj)
		{
			nuint byteCount = GetRawObjectDataSize(null, dstObj);
			ref byte src = ref GetRawData(null, srcObj);
			ref byte dst = ref GetRawData(null, dstObj);

			// if (RuntimeHelpers.GetMethodTable(clone)->ContainsGCPointers)
			BulkMoveWithWriteBarrier(null, ref dst, ref src, byteCount);
			// else
			// 	SpanHelpers.Memmove(ref dst, ref src, byteCount);
		}

		public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
		{
			if (value is null)
			{
				writer.WriteNil();
				return;
			}

			if (_owner._serializedObjects.TryGetValue(value, out var referenceId))
			{
				// This object has already been written. Skip it this time.
				var packLength = MessagePackWriter.GetEncodedLength(referenceId);
				writer.WriteExtensionFormatHeader(new ExtensionHeader(ReferenceExtensionTypeCode, packLength));
				writer.Write(referenceId);
			}
			else
			{
				_owner._serializedObjects.Add(value, _owner._serializingObjectCounter++);
				_owner._inner.GetFormatterWithVerify<T>().Serialize(ref writer, value, options);
			}
		}
	}
}
