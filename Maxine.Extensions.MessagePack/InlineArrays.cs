using System.Runtime.CompilerServices;
using MessagePack;
using MessagePack.Formatters;
using JetBrains.Annotations;

namespace Maxine.Extensions.MessagePack;

[PublicAPI]
public sealed class InlineArray2Formatter<T>(sbyte typeCode = -2) : IMessagePackFormatter<InlineArray2<T>>
{
	public static readonly InlineArray2Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray2<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(2);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
    }

    public InlineArray2<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray2<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray3Formatter<T>(sbyte typeCode = -3) : IMessagePackFormatter<InlineArray3<T>>
{
	public static readonly InlineArray3Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray3<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(3);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
    }

    public InlineArray3<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray3<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray4Formatter<T>(sbyte typeCode = -4) : IMessagePackFormatter<InlineArray4<T>>
{
	public static readonly InlineArray4Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray4<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(4);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
    }

    public InlineArray4<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray4<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray5Formatter<T>(sbyte typeCode = -5) : IMessagePackFormatter<InlineArray5<T>>
{
	public static readonly InlineArray5Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray5<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(5);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
    }

    public InlineArray5<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray5<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray6Formatter<T>(sbyte typeCode = -6) : IMessagePackFormatter<InlineArray6<T>>
{
	public static readonly InlineArray6Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray6<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(6);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
        formatter.Serialize(ref writer, value[5], options);
    }

    public InlineArray6<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray6<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				case 5:
					result[5] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray7Formatter<T>(sbyte typeCode = -7) : IMessagePackFormatter<InlineArray7<T>>
{
	public static readonly InlineArray7Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray7<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(7);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
        formatter.Serialize(ref writer, value[5], options);
        formatter.Serialize(ref writer, value[6], options);
    }

    public InlineArray7<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray7<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				case 5:
					result[5] = formatter.Deserialize(ref reader, options);
					break;
				case 6:
					result[6] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray8Formatter<T>(sbyte typeCode = -8) : IMessagePackFormatter<InlineArray8<T>>
{
	public static readonly InlineArray8Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray8<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(8);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
        formatter.Serialize(ref writer, value[5], options);
        formatter.Serialize(ref writer, value[6], options);
        formatter.Serialize(ref writer, value[7], options);
    }

    public InlineArray8<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray8<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				case 5:
					result[5] = formatter.Deserialize(ref reader, options);
					break;
				case 6:
					result[6] = formatter.Deserialize(ref reader, options);
					break;
				case 7:
					result[7] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray9Formatter<T>(sbyte typeCode = -9) : IMessagePackFormatter<InlineArray9<T>>
{
	public static readonly InlineArray9Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray9<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(9);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
        formatter.Serialize(ref writer, value[5], options);
        formatter.Serialize(ref writer, value[6], options);
        formatter.Serialize(ref writer, value[7], options);
        formatter.Serialize(ref writer, value[8], options);
    }

    public InlineArray9<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray9<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				case 5:
					result[5] = formatter.Deserialize(ref reader, options);
					break;
				case 6:
					result[6] = formatter.Deserialize(ref reader, options);
					break;
				case 7:
					result[7] = formatter.Deserialize(ref reader, options);
					break;
				case 8:
					result[8] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray10Formatter<T>(sbyte typeCode = -10) : IMessagePackFormatter<InlineArray10<T>>
{
	public static readonly InlineArray10Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray10<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(10);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
        formatter.Serialize(ref writer, value[5], options);
        formatter.Serialize(ref writer, value[6], options);
        formatter.Serialize(ref writer, value[7], options);
        formatter.Serialize(ref writer, value[8], options);
        formatter.Serialize(ref writer, value[9], options);
    }

    public InlineArray10<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray10<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				case 5:
					result[5] = formatter.Deserialize(ref reader, options);
					break;
				case 6:
					result[6] = formatter.Deserialize(ref reader, options);
					break;
				case 7:
					result[7] = formatter.Deserialize(ref reader, options);
					break;
				case 8:
					result[8] = formatter.Deserialize(ref reader, options);
					break;
				case 9:
					result[9] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray11Formatter<T>(sbyte typeCode = -11) : IMessagePackFormatter<InlineArray11<T>>
{
	public static readonly InlineArray11Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray11<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(11);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
        formatter.Serialize(ref writer, value[5], options);
        formatter.Serialize(ref writer, value[6], options);
        formatter.Serialize(ref writer, value[7], options);
        formatter.Serialize(ref writer, value[8], options);
        formatter.Serialize(ref writer, value[9], options);
        formatter.Serialize(ref writer, value[10], options);
    }

    public InlineArray11<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray11<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				case 5:
					result[5] = formatter.Deserialize(ref reader, options);
					break;
				case 6:
					result[6] = formatter.Deserialize(ref reader, options);
					break;
				case 7:
					result[7] = formatter.Deserialize(ref reader, options);
					break;
				case 8:
					result[8] = formatter.Deserialize(ref reader, options);
					break;
				case 9:
					result[9] = formatter.Deserialize(ref reader, options);
					break;
				case 10:
					result[10] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray12Formatter<T>(sbyte typeCode = -12) : IMessagePackFormatter<InlineArray12<T>>
{
	public static readonly InlineArray12Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray12<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(12);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
        formatter.Serialize(ref writer, value[5], options);
        formatter.Serialize(ref writer, value[6], options);
        formatter.Serialize(ref writer, value[7], options);
        formatter.Serialize(ref writer, value[8], options);
        formatter.Serialize(ref writer, value[9], options);
        formatter.Serialize(ref writer, value[10], options);
        formatter.Serialize(ref writer, value[11], options);
    }

    public InlineArray12<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray12<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				case 5:
					result[5] = formatter.Deserialize(ref reader, options);
					break;
				case 6:
					result[6] = formatter.Deserialize(ref reader, options);
					break;
				case 7:
					result[7] = formatter.Deserialize(ref reader, options);
					break;
				case 8:
					result[8] = formatter.Deserialize(ref reader, options);
					break;
				case 9:
					result[9] = formatter.Deserialize(ref reader, options);
					break;
				case 10:
					result[10] = formatter.Deserialize(ref reader, options);
					break;
				case 11:
					result[11] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray13Formatter<T>(sbyte typeCode = -13) : IMessagePackFormatter<InlineArray13<T>>
{
	public static readonly InlineArray13Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray13<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(13);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
        formatter.Serialize(ref writer, value[5], options);
        formatter.Serialize(ref writer, value[6], options);
        formatter.Serialize(ref writer, value[7], options);
        formatter.Serialize(ref writer, value[8], options);
        formatter.Serialize(ref writer, value[9], options);
        formatter.Serialize(ref writer, value[10], options);
        formatter.Serialize(ref writer, value[11], options);
        formatter.Serialize(ref writer, value[12], options);
    }

    public InlineArray13<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray13<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				case 5:
					result[5] = formatter.Deserialize(ref reader, options);
					break;
				case 6:
					result[6] = formatter.Deserialize(ref reader, options);
					break;
				case 7:
					result[7] = formatter.Deserialize(ref reader, options);
					break;
				case 8:
					result[8] = formatter.Deserialize(ref reader, options);
					break;
				case 9:
					result[9] = formatter.Deserialize(ref reader, options);
					break;
				case 10:
					result[10] = formatter.Deserialize(ref reader, options);
					break;
				case 11:
					result[11] = formatter.Deserialize(ref reader, options);
					break;
				case 12:
					result[12] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray14Formatter<T>(sbyte typeCode = -14) : IMessagePackFormatter<InlineArray14<T>>
{
	public static readonly InlineArray14Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray14<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(14);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
        formatter.Serialize(ref writer, value[5], options);
        formatter.Serialize(ref writer, value[6], options);
        formatter.Serialize(ref writer, value[7], options);
        formatter.Serialize(ref writer, value[8], options);
        formatter.Serialize(ref writer, value[9], options);
        formatter.Serialize(ref writer, value[10], options);
        formatter.Serialize(ref writer, value[11], options);
        formatter.Serialize(ref writer, value[12], options);
        formatter.Serialize(ref writer, value[13], options);
    }

    public InlineArray14<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray14<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				case 5:
					result[5] = formatter.Deserialize(ref reader, options);
					break;
				case 6:
					result[6] = formatter.Deserialize(ref reader, options);
					break;
				case 7:
					result[7] = formatter.Deserialize(ref reader, options);
					break;
				case 8:
					result[8] = formatter.Deserialize(ref reader, options);
					break;
				case 9:
					result[9] = formatter.Deserialize(ref reader, options);
					break;
				case 10:
					result[10] = formatter.Deserialize(ref reader, options);
					break;
				case 11:
					result[11] = formatter.Deserialize(ref reader, options);
					break;
				case 12:
					result[12] = formatter.Deserialize(ref reader, options);
					break;
				case 13:
					result[13] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray15Formatter<T>(sbyte typeCode = -15) : IMessagePackFormatter<InlineArray15<T>>
{
	public static readonly InlineArray15Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray15<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(15);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
        formatter.Serialize(ref writer, value[5], options);
        formatter.Serialize(ref writer, value[6], options);
        formatter.Serialize(ref writer, value[7], options);
        formatter.Serialize(ref writer, value[8], options);
        formatter.Serialize(ref writer, value[9], options);
        formatter.Serialize(ref writer, value[10], options);
        formatter.Serialize(ref writer, value[11], options);
        formatter.Serialize(ref writer, value[12], options);
        formatter.Serialize(ref writer, value[13], options);
        formatter.Serialize(ref writer, value[14], options);
    }

    public InlineArray15<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray15<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				case 5:
					result[5] = formatter.Deserialize(ref reader, options);
					break;
				case 6:
					result[6] = formatter.Deserialize(ref reader, options);
					break;
				case 7:
					result[7] = formatter.Deserialize(ref reader, options);
					break;
				case 8:
					result[8] = formatter.Deserialize(ref reader, options);
					break;
				case 9:
					result[9] = formatter.Deserialize(ref reader, options);
					break;
				case 10:
					result[10] = formatter.Deserialize(ref reader, options);
					break;
				case 11:
					result[11] = formatter.Deserialize(ref reader, options);
					break;
				case 12:
					result[12] = formatter.Deserialize(ref reader, options);
					break;
				case 13:
					result[13] = formatter.Deserialize(ref reader, options);
					break;
				case 14:
					result[14] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

[PublicAPI]
public sealed class InlineArray16Formatter<T>(sbyte typeCode = -16) : IMessagePackFormatter<InlineArray16<T>>
{
	public static readonly InlineArray16Formatter<T> Instance = new();

    public readonly sbyte TypeCode = typeCode;

    public void Serialize(ref MessagePackWriter writer, InlineArray16<T> value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(16);
        var resolver = options.Resolver;
		var formatter = resolver.GetFormatterWithVerify<T>();

        formatter.Serialize(ref writer, value[0], options);
        formatter.Serialize(ref writer, value[1], options);
        formatter.Serialize(ref writer, value[2], options);
        formatter.Serialize(ref writer, value[3], options);
        formatter.Serialize(ref writer, value[4], options);
        formatter.Serialize(ref writer, value[5], options);
        formatter.Serialize(ref writer, value[6], options);
        formatter.Serialize(ref writer, value[7], options);
        formatter.Serialize(ref writer, value[8], options);
        formatter.Serialize(ref writer, value[9], options);
        formatter.Serialize(ref writer, value[10], options);
        formatter.Serialize(ref writer, value[11], options);
        formatter.Serialize(ref writer, value[12], options);
        formatter.Serialize(ref writer, value[13], options);
        formatter.Serialize(ref writer, value[14], options);
        formatter.Serialize(ref writer, value[15], options);
    }

    public InlineArray16<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            throw new MessagePackSerializationException("Nil is not supported for InlineArray.");
        }


		options.Security.DepthStep(ref reader);
		var formatterResolver = options.Resolver;
		var length = reader.ReadArrayHeader();
		var result = new InlineArray16<T>();
		var formatter = formatterResolver.GetFormatterWithVerify<T>();

		for (int i = 0; i < length; i++)
		{
			switch (i)
			{
				case 0:
					result[0] = formatter.Deserialize(ref reader, options);
					break;
				case 1:
					result[1] = formatter.Deserialize(ref reader, options);
					break;
				case 2:
					result[2] = formatter.Deserialize(ref reader, options);
					break;
				case 3:
					result[3] = formatter.Deserialize(ref reader, options);
					break;
				case 4:
					result[4] = formatter.Deserialize(ref reader, options);
					break;
				case 5:
					result[5] = formatter.Deserialize(ref reader, options);
					break;
				case 6:
					result[6] = formatter.Deserialize(ref reader, options);
					break;
				case 7:
					result[7] = formatter.Deserialize(ref reader, options);
					break;
				case 8:
					result[8] = formatter.Deserialize(ref reader, options);
					break;
				case 9:
					result[9] = formatter.Deserialize(ref reader, options);
					break;
				case 10:
					result[10] = formatter.Deserialize(ref reader, options);
					break;
				case 11:
					result[11] = formatter.Deserialize(ref reader, options);
					break;
				case 12:
					result[12] = formatter.Deserialize(ref reader, options);
					break;
				case 13:
					result[13] = formatter.Deserialize(ref reader, options);
					break;
				case 14:
					result[14] = formatter.Deserialize(ref reader, options);
					break;
				case 15:
					result[15] = formatter.Deserialize(ref reader, options);
					break;
				default:
					reader.Skip();
					break;
			}
		}

		reader.Depth--;
		return result;
    }
}

