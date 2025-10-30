using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Collections.Extensions;

namespace Maxine.Extensions;


public class MultiValueDictionaryConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsConstructedGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(MultiValueDictionary<,>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return (JsonConverter?)Activator.CreateInstance(typeof(MultiValueDictionaryConverter<,>).MakeGenericType(typeToConvert.GetGenericArguments()));
    }
}

public class MultiValueDictionaryConverter<TK, TV> : JsonConverter<MultiValueDictionary<TK, TV>> where TK : IEquatable<TK>
{
    public override bool HandleNull => true;

    public override MultiValueDictionary<TK, TV>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, expected {nameof(JsonTokenType.StartObject)}");
        }

        var keyConverter = (JsonConverter<TK>)options.GetConverter(typeof(TK));
        var valueConverter = (JsonConverter<TV[]>)options.GetConverter(typeof(TV[]));

        var dictionary = new MultiValueDictionary<TK, TV>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, expected {nameof(JsonTokenType.PropertyName)}");
            }

            var k = keyConverter.ReadAsPropertyName(ref reader, typeof(TK), options);

            reader.Read();
            
            var v = valueConverter.Read(ref reader, typeof(TV[]), options);

            dictionary.AddRange(k, v);
        }

        return dictionary;
    }

    public override void Write(Utf8JsonWriter writer, MultiValueDictionary<TK, TV>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }
        
        writer.WriteStartObject();
        
        var keyConverter = (JsonConverter<TK>)options.GetConverter(typeof(TK));
        var valueConverter = (JsonConverter<IReadOnlyCollection<TV>>)options.GetConverter(typeof(IReadOnlyCollection<TV>));
        foreach (var (k, v) in value)
        {
            keyConverter.WriteAsPropertyName(writer, k, options);
            valueConverter.Write(writer, v, options);
        }
        
        writer.WriteEndObject();
    }
}