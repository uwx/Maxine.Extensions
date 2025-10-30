// This work is licensed under a Creative Commons Attribution 4.0 International License
// If you find a bug or some issue you can find me (and the orginal of this file) at https://gist.github.com/AndreSteenveld 
// https://gist.github.com/AndreSteenveld/70328da36039eb349327436526aaa82e

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WadArchiveJsonRenderer;

public class TupleAsArrayFactory : JsonConverterFactory
{
    private static readonly Type?[] SystemTuple =
    {
        null,
        typeof(Tuple<>), typeof(Tuple<,>), typeof(Tuple<,,>), typeof(Tuple<,,,>),
        typeof(Tuple<,,,,>), typeof(Tuple<,,,,,>), typeof(Tuple<,,,,,,>), typeof(Tuple<,,,,,,,>)
    };

    private static readonly Type?[] ValueTuple =
    {
        null,
        typeof(ValueTuple<>), typeof(ValueTuple<,>), typeof(ValueTuple<,,>), typeof(ValueTuple<,,,>),
        typeof(ValueTuple<,,,,>), typeof(ValueTuple<,,,,,>), typeof(ValueTuple<,,,,,,>), typeof(ValueTuple<,,,,,,,>)
    };

    private static readonly Type?[] SystemTupleConverter =
    {
        null,
        typeof(TupleAsArrayConverter<>.SystemTuple), typeof(TupleAsArrayConverter<,>.SystemTuple),
        typeof(TupleAsArrayConverter<,,>.SystemTuple), typeof(TupleAsArrayConverter<,,,>.SystemTuple),
        typeof(TupleAsArrayConverter<,,,,>.SystemTuple), typeof(TupleAsArrayConverter<,,,,,>.SystemTuple),
        typeof(TupleAsArrayConverter<,,,,,,>.SystemTuple), typeof(TupleAsArrayConverter<,,,,,,>.SystemTuple<>)
    };

    private static readonly Type?[] ValueTupleConverter =
    {
        null,
        typeof(TupleAsArrayConverter<>.ValueTuple), typeof(TupleAsArrayConverter<,>.ValueTuple),
        typeof(TupleAsArrayConverter<,,>.ValueTuple), typeof(TupleAsArrayConverter<,,,>.ValueTuple),
        typeof(TupleAsArrayConverter<,,,,>.ValueTuple), typeof(TupleAsArrayConverter<,,,,,>.ValueTuple),
        typeof(TupleAsArrayConverter<,,,,,,>.ValueTuple), typeof(TupleAsArrayConverter<,,,,,,>.ValueTuple<>)
    };

    private static readonly Type[] TupleTypes = Array
        .Empty<Type>()
        .Concat(ValueTuple[1..])
        .Concat(SystemTuple[1..])
        .Cast<Type>()
        .ToArray();

    public override bool CanConvert(Type type)
        => type.IsGenericType && TupleTypes.Contains(type.GetGenericTypeDefinition());

    public override JsonConverter? CreateConverter(Type type, JsonSerializerOptions options)
    {
        var arguments = type.GetGenericArguments();
        var generic = type.GetGenericTypeDefinition();

        var converter = SystemTuple[arguments.Length] == generic
            ? SystemTupleConverter[arguments.Length]!.MakeGenericType(arguments)
            : ValueTuple[arguments.Length] == generic
                ? ValueTupleConverter[arguments.Length]!.MakeGenericType(arguments)
                : throw new NotSupportedException();

        return (JsonConverter?)Activator.CreateInstance(converter);
    }
}

public static class TupleAsArrayConverter<T1>
{
    private static ITuple? read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        var values = new[]
        {
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T1), options) : throw new JsonException()
        };

        reader.Read();

        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException();

        return (ITuple?)Activator.CreateInstance(type, values);
    }

    private static void write(Utf8JsonWriter writer, ITuple tuple, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        (options.GetConverter(typeof(T1)) as JsonConverter<T1>)!.Write(writer, (T1)tuple[0], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1>>
    {
        public override Tuple<T1>? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
            => (Tuple<T1>)read(ref reader, type, options)!;

        public override void Write(Utf8JsonWriter writer, Tuple<T1> tuple, JsonSerializerOptions options)
            => write(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1>>
    {
        public override ValueTuple<T1> Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
            => (ValueTuple<T1>)read(ref reader, type, options)!;

        public override void Write(Utf8JsonWriter writer, ValueTuple<T1> tuple, JsonSerializerOptions options)
            => write(writer, tuple, options);
    }
}

public static class TupleAsArrayConverter<T1, T2>
{
    private static ITuple? read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        var values = new[]
        {
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T1), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T2), options) : throw new JsonException()
        };

        reader.Read();

        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException();

        return (ITuple?)Activator.CreateInstance(type, values);
    }

    private static void write(Utf8JsonWriter writer, ITuple tuple, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        (options.GetConverter(typeof(T1)) as JsonConverter<T1>)!.Write(writer, (T1)tuple[0], options);
        (options.GetConverter(typeof(T2)) as JsonConverter<T2>)!.Write(writer, (T2)tuple[1], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2>>
    {
        public override Tuple<T1, T2>? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
            => (Tuple<T1, T2>)read(ref reader, type, options)!;

        public override void Write(Utf8JsonWriter writer, Tuple<T1, T2> tuple, JsonSerializerOptions options)
            => write(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2>>
    {
        public override ValueTuple<T1, T2> Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (ValueTuple<T1, T2>)read(ref reader, type, options)!;

        public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2> tuple, JsonSerializerOptions options)
            => write(writer, tuple, options);
    }
}

public static class TupleAsArrayConverter<T1, T2, T3>
{
    private static ITuple? read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        var values = new[]
        {
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T1), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T2), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T3), options) : throw new JsonException()
        };

        reader.Read();

        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException();

        return (ITuple?)Activator.CreateInstance(type, values);
    }

    private static void write(Utf8JsonWriter writer, ITuple tuple, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        (options.GetConverter(typeof(T1)) as JsonConverter<T1>)!.Write(writer, (T1)tuple[0], options);
        (options.GetConverter(typeof(T2)) as JsonConverter<T2>)!.Write(writer, (T2)tuple[1], options);
        (options.GetConverter(typeof(T3)) as JsonConverter<T3>)!.Write(writer, (T3)tuple[2], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2, T3>>
    {
        public override Tuple<T1, T2, T3>? Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (Tuple<T1, T2, T3>)read(ref reader, type, options)!;

        public override void Write(Utf8JsonWriter writer, Tuple<T1, T2, T3> tuple, JsonSerializerOptions options)
            => write(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2, T3>>
    {
        public override ValueTuple<T1, T2, T3> Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (ValueTuple<T1, T2, T3>)read(ref reader, type, options)!;

        public override void Write(
            Utf8JsonWriter writer, ValueTuple<T1, T2, T3> tuple, JsonSerializerOptions options
        ) => write(writer, tuple, options);
    }
}

public static class TupleAsArrayConverter<T1, T2, T3, T4>
{
    private static ITuple? read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        var values = new[]
        {
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T1), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T2), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T3), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T4), options) : throw new JsonException()
        };

        reader.Read();

        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException();

        return (ITuple?)Activator.CreateInstance(type, values);
    }

    private static void write(Utf8JsonWriter writer, ITuple tuple, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        (options.GetConverter(typeof(T1)) as JsonConverter<T1>)!.Write(writer, (T1)tuple[0], options);
        (options.GetConverter(typeof(T2)) as JsonConverter<T2>)!.Write(writer, (T2)tuple[1], options);
        (options.GetConverter(typeof(T3)) as JsonConverter<T3>)!.Write(writer, (T3)tuple[2], options);
        (options.GetConverter(typeof(T4)) as JsonConverter<T4>)!.Write(writer, (T4)tuple[3], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2, T3, T4>>
    {
        public override Tuple<T1, T2, T3, T4>? Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (Tuple<T1, T2, T3, T4>)read(ref reader, type, options)!;

        public override void Write(
            Utf8JsonWriter writer, Tuple<T1, T2, T3, T4> tuple, JsonSerializerOptions options
        ) => write(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2, T3, T4>>
    {
        public override ValueTuple<T1, T2, T3, T4> Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (ValueTuple<T1, T2, T3, T4>)read(ref reader, type, options)!;

        public override void Write(
            Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4> tuple, JsonSerializerOptions options
        ) => write(writer, tuple, options);
    }
}

public static class TupleAsArrayConverter<T1, T2, T3, T4, T5>
{
    private static ITuple? read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        var values = new[]
        {
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T1), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T2), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T3), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T4), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T5), options) : throw new JsonException()
        };

        reader.Read();

        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException();

        return (ITuple?)Activator.CreateInstance(type, values);
    }

    private static void write(Utf8JsonWriter writer, ITuple tuple, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        (options.GetConverter(typeof(T1)) as JsonConverter<T1>)!.Write(writer, (T1)tuple[0], options);
        (options.GetConverter(typeof(T2)) as JsonConverter<T2>)!.Write(writer, (T2)tuple[1], options);
        (options.GetConverter(typeof(T3)) as JsonConverter<T3>)!.Write(writer, (T3)tuple[2], options);
        (options.GetConverter(typeof(T4)) as JsonConverter<T4>)!.Write(writer, (T4)tuple[3], options);
        (options.GetConverter(typeof(T5)) as JsonConverter<T5>)!.Write(writer, (T5)tuple[4], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2, T3, T4, T5>>
    {
        public override Tuple<T1, T2, T3, T4, T5>? Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (Tuple<T1, T2, T3, T4, T5>)read(ref reader, type, options)!;

        public override void Write(
            Utf8JsonWriter writer, Tuple<T1, T2, T3, T4, T5> tuple, JsonSerializerOptions options
        ) => write(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2, T3, T4, T5>>
    {
        public override ValueTuple<T1, T2, T3, T4, T5> Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (ValueTuple<T1, T2, T3, T4, T5>)read(ref reader, type, options)!;

        public override void Write(
            Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5> tuple, JsonSerializerOptions options
        ) => write(writer, tuple, options);
    }
}

public static class TupleAsArrayConverter<T1, T2, T3, T4, T5, T6>
{
    private static ITuple? read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        var values = new[]
        {
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T1), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T2), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T3), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T4), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T5), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T6), options) : throw new JsonException()
        };

        reader.Read();

        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException();

        return (ITuple?)Activator.CreateInstance(type, values);
    }

    private static void write(Utf8JsonWriter writer, ITuple tuple, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        (options.GetConverter(typeof(T1)) as JsonConverter<T1>)!.Write(writer, (T1)tuple[0], options);
        (options.GetConverter(typeof(T2)) as JsonConverter<T2>)!.Write(writer, (T2)tuple[1], options);
        (options.GetConverter(typeof(T3)) as JsonConverter<T3>)!.Write(writer, (T3)tuple[2], options);
        (options.GetConverter(typeof(T4)) as JsonConverter<T4>)!.Write(writer, (T4)tuple[3], options);
        (options.GetConverter(typeof(T5)) as JsonConverter<T5>)!.Write(writer, (T5)tuple[4], options);
        (options.GetConverter(typeof(T6)) as JsonConverter<T6>)!.Write(writer, (T6)tuple[5], options);


        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2, T3, T4, T5, T6>>
    {
        public override Tuple<T1, T2, T3, T4, T5, T6>? Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (Tuple<T1, T2, T3, T4, T5, T6>)read(ref reader, type, options)!;

        public override void Write(
            Utf8JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6> tuple, JsonSerializerOptions options
        ) => write(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        public override ValueTuple<T1, T2, T3, T4, T5, T6> Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (ValueTuple<T1, T2, T3, T4, T5, T6>)read(ref reader, type, options)!;

        public override void Write(
            Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6> tuple, JsonSerializerOptions options
        ) => write(writer, tuple, options);
    }
}

public static class TupleAsArrayConverter<T1, T2, T3, T4, T5, T6, T7>
{
    private static ITuple? read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        var values = new[]
        {
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T1), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T2), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T3), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T4), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T5), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T6), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T7), options) : throw new JsonException()
        };

        reader.Read();

        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException();

        return (ITuple?)Activator.CreateInstance(type, values);
    }

    private static void write(Utf8JsonWriter writer, ITuple tuple, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        (options.GetConverter(typeof(T1)) as JsonConverter<T1>)!.Write(writer, (T1)tuple[0], options);
        (options.GetConverter(typeof(T2)) as JsonConverter<T2>)!.Write(writer, (T2)tuple[1], options);
        (options.GetConverter(typeof(T3)) as JsonConverter<T3>)!.Write(writer, (T3)tuple[2], options);
        (options.GetConverter(typeof(T4)) as JsonConverter<T4>)!.Write(writer, (T4)tuple[3], options);
        (options.GetConverter(typeof(T5)) as JsonConverter<T5>)!.Write(writer, (T5)tuple[4], options);
        (options.GetConverter(typeof(T6)) as JsonConverter<T6>)!.Write(writer, (T6)tuple[5], options);
        (options.GetConverter(typeof(T7)) as JsonConverter<T7>)!.Write(writer, (T7)tuple[6], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public override Tuple<T1, T2, T3, T4, T5, T6, T7>? Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (Tuple<T1, T2, T3, T4, T5, T6, T7>)read(ref reader, type, options)!;

        public override void Write(
            Utf8JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7> tuple, JsonSerializerOptions options
        ) => write(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public override ValueTuple<T1, T2, T3, T4, T5, T6, T7> Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (ValueTuple<T1, T2, T3, T4, T5, T6, T7>)read(ref reader, type, options)!;

        public override void Write(
            Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> tuple, JsonSerializerOptions options
        ) => write(writer, tuple, options);
    }

    //
    // 
    //

    private static ITuple? read<TRest>(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        var values = new[]
        {
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T1), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T2), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T3), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T4), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T5), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T6), options) : throw new JsonException(),
            reader.Read() ? JsonSerializer.Deserialize(ref reader, typeof(T7), options) : throw new JsonException(),
            reader.Read() ? DeserializeRest(ref reader, typeof(TRest), options) : throw new JsonException()
        };

        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException();

        return (ITuple?)Activator.CreateInstance(type, values);

        object? DeserializeRest(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            writer.WriteStartArray();

            while (reader.TokenType != JsonTokenType.EndArray)
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                    case JsonTokenType.Number:
                    case JsonTokenType.Null:
                        writer.WriteRawValue(reader.ValueSpan, true);
                        break;

                    case JsonTokenType.String:
                        writer.WriteStringValue(reader.GetString());
                        break;

                    case JsonTokenType.StartObject:
                    case JsonTokenType.StartArray:
                        writer.WriteRawValue(reader.ValueSequence.ToArray(), true);
                        break;

                    default: throw new JsonException();
                }

                reader.Read();
            }

            writer.WriteEndArray();
            writer.Flush();

            return JsonSerializer.Deserialize(Encoding.UTF8.GetString(stream.ToArray()), type, options);
        }
    }

    private static void write<TRest>(Utf8JsonWriter writer, ITuple tuple, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        (options.GetConverter(typeof(T1)) as JsonConverter<T1>)!.Write(writer, (T1)tuple[0], options);
        (options.GetConverter(typeof(T2)) as JsonConverter<T2>)!.Write(writer, (T2)tuple[1], options);
        (options.GetConverter(typeof(T3)) as JsonConverter<T3>)!.Write(writer, (T3)tuple[2], options);
        (options.GetConverter(typeof(T4)) as JsonConverter<T4>)!.Write(writer, (T4)tuple[3], options);
        (options.GetConverter(typeof(T5)) as JsonConverter<T5>)!.Write(writer, (T5)tuple[4], options);
        (options.GetConverter(typeof(T6)) as JsonConverter<T6>)!.Write(writer, (T6)tuple[5], options);
        (options.GetConverter(typeof(T7)) as JsonConverter<T7>)!.Write(writer, (T7)tuple[6], options);

        for (int i = 7; i < tuple.Length; i++)
        {
            var type = tuple[i].GetType();

            dynamic value = Convert.ChangeType(tuple[i], type);
            dynamic converter = options.GetConverter(type);

            converter.Write(writer, value, options);
        }

        writer.WriteEndArray();
    }

    public class SystemTuple<TRest> : JsonConverter<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
    {
        public override Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>? Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>)read<TRest>(ref reader, type, options)!;

        public override void Write(
            Utf8JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple, JsonSerializerOptions options
        ) => write<TRest>(writer, tuple, options);
    }

    public class ValueTuple<TRest> : JsonConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>> where TRest : struct
    {
        public override ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        ) => (ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)read<TRest>(ref reader, type, options)!;

        public override void Write(
            Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple,
            JsonSerializerOptions options
        ) => write<TRest>(writer, tuple, options);
    }
}