using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Maxine.Extensions;


public static class TupleAsArrayConverter<T1>
{
    private static void WriteInternal<TTuple>(Utf8JsonWriter writer, TTuple tuple, JsonSerializerOptions options)
        where TTuple : ITuple
    {
        writer.WriteStartArray();

        Helpers.Write<T1>(writer, tuple[0], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1>>
    {
        public override Tuple<T1>? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            
            Helpers.StartRead(ref reader);

            var result = new Tuple<T1>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Tuple<T1> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1>>
    {
        public override ValueTuple<T1> Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return default;
            
            Helpers.StartRead(ref reader);

            var result = new ValueTuple<T1>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, ValueTuple<T1> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }
}


public static class TupleAsArrayConverter<T1, T2>
{
    private static void WriteInternal<TTuple>(Utf8JsonWriter writer, TTuple tuple, JsonSerializerOptions options)
        where TTuple : ITuple
    {
        writer.WriteStartArray();

        Helpers.Write<T1>(writer, tuple[0], options);
        Helpers.Write<T2>(writer, tuple[1], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2>>
    {
        public override Tuple<T1, T2>? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            
            Helpers.StartRead(ref reader);

            var result = new Tuple<T1, T2>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Tuple<T1, T2> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2>>
    {
        public override ValueTuple<T1, T2> Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return default;
            
            Helpers.StartRead(ref reader);

            var result = new ValueTuple<T1, T2>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }
}


public static class TupleAsArrayConverter<T1, T2, T3>
{
    private static void WriteInternal<TTuple>(Utf8JsonWriter writer, TTuple tuple, JsonSerializerOptions options)
        where TTuple : ITuple
    {
        writer.WriteStartArray();

        Helpers.Write<T1>(writer, tuple[0], options);
        Helpers.Write<T2>(writer, tuple[1], options);
        Helpers.Write<T3>(writer, tuple[2], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2, T3>>
    {
        public override Tuple<T1, T2, T3>? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            
            Helpers.StartRead(ref reader);

            var result = new Tuple<T1, T2, T3>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Tuple<T1, T2, T3> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2, T3>>
    {
        public override ValueTuple<T1, T2, T3> Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return default;
            
            Helpers.StartRead(ref reader);

            var result = new ValueTuple<T1, T2, T3>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2, T3> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }
}


public static class TupleAsArrayConverter<T1, T2, T3, T4>
{
    private static void WriteInternal<TTuple>(Utf8JsonWriter writer, TTuple tuple, JsonSerializerOptions options)
        where TTuple : ITuple
    {
        writer.WriteStartArray();

        Helpers.Write<T1>(writer, tuple[0], options);
        Helpers.Write<T2>(writer, tuple[1], options);
        Helpers.Write<T3>(writer, tuple[2], options);
        Helpers.Write<T4>(writer, tuple[3], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2, T3, T4>>
    {
        public override Tuple<T1, T2, T3, T4>? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            
            Helpers.StartRead(ref reader);

            var result = new Tuple<T1, T2, T3, T4>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!,
                Helpers.ReadOrThrow<T4>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Tuple<T1, T2, T3, T4> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2, T3, T4>>
    {
        public override ValueTuple<T1, T2, T3, T4> Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return default;
            
            Helpers.StartRead(ref reader);

            var result = new ValueTuple<T1, T2, T3, T4>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!,
                Helpers.ReadOrThrow<T4>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }
}


public static class TupleAsArrayConverter<T1, T2, T3, T4, T5>
{
    private static void WriteInternal<TTuple>(Utf8JsonWriter writer, TTuple tuple, JsonSerializerOptions options)
        where TTuple : ITuple
    {
        writer.WriteStartArray();

        Helpers.Write<T1>(writer, tuple[0], options);
        Helpers.Write<T2>(writer, tuple[1], options);
        Helpers.Write<T3>(writer, tuple[2], options);
        Helpers.Write<T4>(writer, tuple[3], options);
        Helpers.Write<T5>(writer, tuple[4], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2, T3, T4, T5>>
    {
        public override Tuple<T1, T2, T3, T4, T5>? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            
            Helpers.StartRead(ref reader);

            var result = new Tuple<T1, T2, T3, T4, T5>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!,
                Helpers.ReadOrThrow<T4>(ref reader, options)!,
                Helpers.ReadOrThrow<T5>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Tuple<T1, T2, T3, T4, T5> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2, T3, T4, T5>>
    {
        public override ValueTuple<T1, T2, T3, T4, T5> Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return default;
            
            Helpers.StartRead(ref reader);

            var result = new ValueTuple<T1, T2, T3, T4, T5>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!,
                Helpers.ReadOrThrow<T4>(ref reader, options)!,
                Helpers.ReadOrThrow<T5>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }
}


public static class TupleAsArrayConverter<T1, T2, T3, T4, T5, T6>
{
    private static void WriteInternal<TTuple>(Utf8JsonWriter writer, TTuple tuple, JsonSerializerOptions options)
        where TTuple : ITuple
    {
        writer.WriteStartArray();

        Helpers.Write<T1>(writer, tuple[0], options);
        Helpers.Write<T2>(writer, tuple[1], options);
        Helpers.Write<T3>(writer, tuple[2], options);
        Helpers.Write<T4>(writer, tuple[3], options);
        Helpers.Write<T5>(writer, tuple[4], options);
        Helpers.Write<T6>(writer, tuple[5], options);

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2, T3, T4, T5, T6>>
    {
        public override Tuple<T1, T2, T3, T4, T5, T6>? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            
            Helpers.StartRead(ref reader);

            var result = new Tuple<T1, T2, T3, T4, T5, T6>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!,
                Helpers.ReadOrThrow<T4>(ref reader, options)!,
                Helpers.ReadOrThrow<T5>(ref reader, options)!,
                Helpers.ReadOrThrow<T6>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        public override ValueTuple<T1, T2, T3, T4, T5, T6> Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return default;
            
            Helpers.StartRead(ref reader);

            var result = new ValueTuple<T1, T2, T3, T4, T5, T6>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!,
                Helpers.ReadOrThrow<T4>(ref reader, options)!,
                Helpers.ReadOrThrow<T5>(ref reader, options)!,
                Helpers.ReadOrThrow<T6>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }
}


public static class TupleAsArrayConverter<T1, T2, T3, T4, T5, T6, T7>
{
    private static void WriteInternal<TTuple>(Utf8JsonWriter writer, TTuple tuple, JsonSerializerOptions options)
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

        writer.WriteEndArray();
    }

    public class SystemTuple : JsonConverter<Tuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public override Tuple<T1, T2, T3, T4, T5, T6, T7>? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            
            Helpers.StartRead(ref reader);

            var result = new Tuple<T1, T2, T3, T4, T5, T6, T7>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!,
                Helpers.ReadOrThrow<T4>(ref reader, options)!,
                Helpers.ReadOrThrow<T5>(ref reader, options)!,
                Helpers.ReadOrThrow<T6>(ref reader, options)!,
                Helpers.ReadOrThrow<T7>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }

    public class ValueTuple : JsonConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public override ValueTuple<T1, T2, T3, T4, T5, T6, T7> Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return default;
            
            Helpers.StartRead(ref reader);

            var result = new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(
                Helpers.ReadOrThrow<T1>(ref reader, options)!,
                Helpers.ReadOrThrow<T2>(ref reader, options)!,
                Helpers.ReadOrThrow<T3>(ref reader, options)!,
                Helpers.ReadOrThrow<T4>(ref reader, options)!,
                Helpers.ReadOrThrow<T5>(ref reader, options)!,
                Helpers.ReadOrThrow<T6>(ref reader, options)!,
                Helpers.ReadOrThrow<T7>(ref reader, options)!
            );

            Helpers.EndRead(ref reader);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> tuple, JsonSerializerOptions options)
            => WriteInternal(writer, tuple, options);
    }
}

