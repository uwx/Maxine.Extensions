using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using SoftCircuits.Collections;

namespace Maxine.TU.UploadHelpers;

// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-8-0#sample-factory-pattern-converter
public class OrderedDictionaryConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsConstructedGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(OrderedDictionary<,>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var genericArguments = typeToConvert.GetGenericArguments();
        var keyType = genericArguments[0];
        var valueType = genericArguments[1];

        var converter = (JsonConverter)Activator.CreateInstance(
            typeof(OrderedDictionaryConverter<,>).MakeGenericType(keyType, valueType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[] { options },
            culture: null
        )!;

        return converter;
    }

    private class OrderedDictionaryConverter<TKey, TValue>(JsonSerializerOptions options)
        : JsonConverter<OrderedDictionary<TKey, TValue>>
        where TKey : notnull
    {
        private readonly JsonConverter<TKey> _keyConverter = (JsonConverter<TKey>)options.GetConverter(typeof(TKey));
        private readonly JsonConverter<TValue> _valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));
        private readonly Type _keyType = typeof(TKey);
        private readonly Type _valueType = typeof(TValue);

        // Cache the key and value types.

        public override OrderedDictionary<TKey, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var dictionary = new OrderedDictionary<TKey, TValue>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dictionary;
                }

                // Get the key.
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var key = _keyConverter.ReadAsPropertyName(ref reader, _keyType, options);

                // Get the value.
                if (!reader.Read())
                {
                    throw new JsonException();
                }
                var value = _valueConverter.Read(ref reader, _valueType, options)!;

                // Add to dictionary.
                dictionary.Add(key, value);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, OrderedDictionary<TKey, TValue> dictionary, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var (key, value) in dictionary)
            {
                _keyConverter.WriteAsPropertyName(writer, key, options);
                _valueConverter.Write(writer, value, options);
            }

            writer.WriteEndObject();
        }
    }
}