using System.Runtime.InteropServices;
using MessagePack;
using MessagePack.Formatters;

namespace Maxine.Extensions.MessagePack;

public static class DedupingResolver
{
    public static DedupingResolver<T> Create<T>(T inner) where T : IFormatterResolver
    {
        return new DedupingResolver<T>(inner);
    }
}

public class DedupingResolver<TResolver> : IFormatterResolver where TResolver : IFormatterResolver
{
	private const sbyte ReferenceExtensionTypeCode = 1;

	private readonly TResolver _inner;
	private readonly Dictionary<object, int> _serializedObjects = [];
	private readonly HashSet<object> _serializingObjects = [];
	private readonly List<object?> _deserializedObjects = [];
	private readonly Dictionary<Type, IMessagePackFormatter> _dedupingFormatters = [];
	private int _serializingObjectCounter;

	internal DedupingResolver(TResolver inner)
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
			formatter = new DedupingFormatter<T>(this);
		}

		return (IMessagePackFormatter<T>)formatter!;
	}

	private class DedupingFormatter<T> : IMessagePackFormatter<T>
	{
		private readonly DedupingResolver<TResolver> _owner;

		internal DedupingFormatter(DedupingResolver<TResolver> owner)
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

			// Reserve our position in the array.
			var reservation = _owner._deserializedObjects.Count;
			_owner._deserializedObjects.Add(null);
			var value = _owner._inner.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
			_owner._deserializedObjects[reservation] = value;
			return value;
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
				// Check for cyclical reference
				if (!_owner._serializingObjects.Add(value))
				{
					throw new MessagePackSerializationException($"Cyclical reference detected while serializing object of type {typeof(T).Name}");
				}

				try
				{
					_owner._serializedObjects.Add(value, _owner._serializingObjectCounter++);
					_owner._inner.GetFormatterWithVerify<T>().Serialize(ref writer, value, options);
				}
				finally
				{
					_owner._serializingObjects.Remove(value);
				}
			}
		}
	}
}