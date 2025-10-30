// This work is licensed under a Creative Commons Attribution 4.0 International License
// If you find a bug or some issue you can find me (and the orginal of this file) at https://gist.github.com/AndreSteenveld 
// https://gist.github.com/AndreSteenveld/70328da36039eb349327436526aaa82e

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Maxine.Extensions;

public class TupleAsArrayFactory : JsonConverterFactory
{
    private static readonly Type[] SystemTuple =
    [
        typeof(Tuple<>),
        typeof(Tuple<,>),
        typeof(Tuple<,,>),
        typeof(Tuple<,,,>),
        typeof(Tuple<,,,,>),
        typeof(Tuple<,,,,,>),
        typeof(Tuple<,,,,,,>),
        typeof(Tuple<,,,,,,,>)
    ];

    internal static readonly Type[] ValueTuple =
    [
        typeof(ValueTuple<>),
        typeof(ValueTuple<,>),
        typeof(ValueTuple<,,>),
        typeof(ValueTuple<,,,>),
        typeof(ValueTuple<,,,,>),
        typeof(ValueTuple<,,,,,>),
        typeof(ValueTuple<,,,,,,>),
        typeof(ValueTuple<,,,,,,,>)
    ];

    private static readonly Type[] SystemTupleConverter =
    [
        typeof(TupleAsArrayConverter<>.SystemTuple),
        typeof(TupleAsArrayConverter<,>.SystemTuple),
        typeof(TupleAsArrayConverter<,,>.SystemTuple),
        typeof(TupleAsArrayConverter<,,,>.SystemTuple),
        typeof(TupleAsArrayConverter<,,,,>.SystemTuple),
        typeof(TupleAsArrayConverter<,,,,,>.SystemTuple),
        typeof(TupleAsArrayConverter<,,,,,,>.SystemTuple),
        typeof(TupleAsArrayConverter_Rest<,,,,,,>.SystemTuple<>)
    ];

    private static readonly Type[] ValueTupleConverter =
    [
        typeof(TupleAsArrayConverter<>.ValueTuple),
        typeof(TupleAsArrayConverter<,>.ValueTuple),
        typeof(TupleAsArrayConverter<,,>.ValueTuple),
        typeof(TupleAsArrayConverter<,,,>.ValueTuple),
        typeof(TupleAsArrayConverter<,,,,>.ValueTuple),
        typeof(TupleAsArrayConverter<,,,,,>.ValueTuple),
        typeof(TupleAsArrayConverter<,,,,,,>.ValueTuple),
        typeof(TupleAsArrayConverter_Rest<,,,,,,>.ValueTuple<>)
    ];

    private static readonly Type[] TupleTypes = [typeof(ITuple), ..ValueTuple, ..SystemTuple];

    public override bool CanConvert(Type type)
        => type.IsGenericType && TupleTypes.Contains(type.GetGenericTypeDefinition());

    public override JsonConverter? CreateConverter(Type type, JsonSerializerOptions options)
    {
        if (type == typeof(ITuple))
            return new TupleConverter();
        
        var arguments = type.GetGenericArguments();
        var generic = type.GetGenericTypeDefinition();

        var converter
            = SystemTuple[arguments.Length-1] == generic ? SystemTupleConverter[arguments.Length-1]
            : ValueTuple[arguments.Length-1] == generic ? ValueTupleConverter[arguments.Length-1]
            : throw new NotSupportedException();

        return (JsonConverter?)Activator.CreateInstance(converter.MakeGenericType(arguments));
    }
}

public class TupleConverter : JsonConverter<ITuple>
{
    private class DeserializedTuple(object?[] elements) : ITuple
    {
        public object? this[int index] => elements[index];

        public int Length => elements.Length;
    }
    
    public override ITuple? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var conv = (JsonConverter<object?[]>)options.GetConverter(typeof(object?[]));
        var arr = conv.Read(ref reader, typeToConvert, options);
        return arr != null ? new DeserializedTuple(arr) : null;
    }

    public override void Write(Utf8JsonWriter writer, ITuple value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        for (var i = 0; i < value.Length; i++)
        {
            var o = value[i];
            if (o != null)
            {
                var jsonConverter = options.GetConverter(o.GetType());
                Reflection.WriteAsObject(jsonConverter, writer, o, options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
        
        writer.WriteEndArray();
        
        // Reflection.WriteAsObject(options.GetConverter(value.GetType()), writer, value, options);
    }
}

internal static class Helpers
{
    public static void StartRead(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"{reader.TokenType} is not an array.");
    }
    
    public static T? ReadOrThrow<T>(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (!reader.Read())
        {
            ThrowJsonException();
            return default;
        }
        
        var jsonConverter = (options.GetConverter(typeof(T)) as JsonConverter<T>)!;
        if (reader.TokenType == JsonTokenType.Null && !jsonConverter.HandleNull)
        {
            return default;
        }
        return jsonConverter.Read(ref reader, typeof(T), options);
        
        // return JsonSerializer.Deserialize<T>(ref reader, options);
    }

    public static void EndRead(ref Utf8JsonReader reader)
    {
        reader.Read();

        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException();
    }

    [DoesNotReturn]
    private static void ThrowJsonException()
    {
        throw new JsonException();
    }

    public static void Write<T>(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        var jsonConverter = (options.GetConverter(typeof(T)) as JsonConverter<T>)!;

        if (value != null || jsonConverter.HandleNull)
        {
            jsonConverter.Write(writer, (T)value!, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}


public static class TupleAsArrayConverter_Rest<T1, T2, T3, T4, T5, T6, T7>
{
    //
    // read/write TRest flattened
    //

    private static void WriteInternal<TTuple, TRest>(Utf8JsonWriter writer, TTuple tuple, JsonSerializerOptions options)
        where TTuple : ITuple
    {
        writer.WriteStartArray();

        Helpers.Write<T1>(writer, tuple[0], options);
        Helpers.Write<T2>(writer, tuple[1], options);
        Helpers.Write<T3>(writer, tuple[2], options);
        Helpers.Write<T4>(writer, tuple[3], options);
        Helpers.Write<T5>(writer, tuple[4], options);
        Helpers.Write<T6>(writer, tuple[5], options);
        Helpers.Write<T7>(writer, tuple[6], options);

        for (int i = 7; i < tuple.Length; i++)
        {
            if (tuple[i] is { } value)
            {
                var type = value.GetType();

                var converter = options.GetConverter(type);

                Reflection.WriteAsObject(converter, writer, tuple[i], options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }

        writer.WriteEndArray();
    }

    private static object? DeserializeRest(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (!type.IsAssignableTo(typeof(ITuple)))
        {
            throw new JsonException($"{type} is not {nameof(ITuple)}");
        }

        if (!type.IsConstructedGenericType)
        {
            throw new JsonException($"{type} is not a generic tuple");
        }

        var args = type.GetGenericArguments();
        var values = new object?[args.Length];
        var i = 0;

        // Console.WriteLine(reader.TokenType);
        while (reader.TokenType != JsonTokenType.EndArray)
        {
            // Console.WriteLine(i + ": " + reader.TokenType);

            if (i == 7) // TRest again
            {
                values[i] = DeserializeRest(ref reader, args[i], options);
                break;
            }

            var jsonConverter = options.GetConverter(args[i]);
            if (jsonConverter is JsonConverterFactory)
            {
                throw new JsonException($"{jsonConverter} is {nameof(JsonConverterFactory)}, this shouldn't be possible at all");
            }

            if (reader.TokenType == JsonTokenType.Null && !Reflection.get_HandleNullOnWrite(jsonConverter))
            {
                values[i] = default;
            }
            else
            {
                values[i] = Reflection.ReadAsObject(jsonConverter, ref reader, args[i], options);
            }

            i++;

            if (!reader.Read())
            {
                throw new JsonException();
            }
        }

        if (type != typeof(ITuple))
        {
            return Activator.CreateInstance(type, values);
        }
        else
        {
            return Activator.CreateInstance(TupleAsArrayFactory.ValueTuple[i]!, values);
        }
    }

    public class SystemTuple<TRest> : JsonConverter<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>> where TRest : notnull
    {
        public override Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>? Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        )
        {
            Helpers.StartRead(ref reader);

            var result = new Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>
            (
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!,
                Helpers.ReadOrThrow<T4>(ref reader, options)!,
                Helpers.ReadOrThrow<T5>(ref reader, options)!,
                Helpers.ReadOrThrow<T6>(ref reader, options)!,
                Helpers.ReadOrThrow<T7>(ref reader, options)!,
                (TRest)(reader.Read() ? DeserializeRest(ref reader, typeof(TRest), options) : throw new JsonException())!
            );

            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException();

            return result;
        }

        public override void Write(
            Utf8JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple, JsonSerializerOptions options
        ) => WriteInternal<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>, TRest>(writer, tuple, options);
    }

    public class ValueTuple<TRest> : JsonConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>> where TRest : struct
    {
        public override ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> Read(
            ref Utf8JsonReader reader, Type type, JsonSerializerOptions options
        )
        {
            Helpers.StartRead(ref reader);

            var result = new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>
            (
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!,
                Helpers.ReadOrThrow<T4>(ref reader, options)!,
                Helpers.ReadOrThrow<T5>(ref reader, options)!,
                Helpers.ReadOrThrow<T6>(ref reader, options)!,
                Helpers.ReadOrThrow<T7>(ref reader, options)!,
                (TRest)(reader.Read() ? DeserializeRest(ref reader, typeof(TRest), options) : throw new JsonException())!
            );

            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException();

            return result;
        }

        public override void Write(
            Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple,
            JsonSerializerOptions options
        ) => WriteInternal<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>, TRest>(writer, tuple, options);
    }
}

file class Reflection
{
    [UnsafeAccessor(UnsafeAccessorKind.Method)]
    public static extern JsonConverter<TTarget> CreateCastingConverter<TTarget>(JsonConverter converter);

    [UnsafeAccessor(UnsafeAccessorKind.Method)]
    public static extern bool get_HandleNullOnWrite(JsonConverter converter);

    [UnsafeAccessor(UnsafeAccessorKind.Method)]
    public static extern object ReadAsObject(JsonConverter converter, ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);

    [UnsafeAccessor(UnsafeAccessorKind.Method)]
    public static extern void WriteAsObject(JsonConverter converter, Utf8JsonWriter writer, object? value, JsonSerializerOptions options);
}