using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.Json;

namespace Maxine.Extensions.Test;

[TestClass]
public class JsonTests
{
    [TestMethod]
    public void TestMultiValueDictionaryConverterFactory_SerializeAndDeserialize()
    {
        // Arrange
        var dictionary = new Dictionary<string, List<int>>
        {
            { "key1", new List<int> { 1, 2, 3 } },
            { "key2", new List<int> { 4, 5, 6 } }
        };

        var options = new JsonSerializerOptions();
        options.Converters.Add(new MultiValueDictionaryConverterFactory());

        // Act
        var json = JsonSerializer.Serialize(dictionary, options);
        var deserialized = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(json, options);

        // Assert
        CollectionAssert.AreEqual(dictionary["key1"], deserialized["key1"]);
        CollectionAssert.AreEqual(dictionary["key2"], deserialized["key2"]);
    }

    [TestMethod]
    public void TestOrderedDictionaryConverterFactory_SerializeAndDeserialize()
    {
        // Arrange
        var dictionary = new OrderedDictionary<string, int>
        {
            { "key1", 1 },
            { "key2", 2 }
        };

        var options = new JsonSerializerOptions();
        options.Converters.Add(new OrderedDictionaryConverterFactory());

        // Act
        var json = JsonSerializer.Serialize(dictionary, options);
        var deserialized = JsonSerializer.Deserialize<OrderedDictionary<string, int>>(json, options);

        // Assert
        Assert.AreEqual(dictionary["key1"], deserialized["key1"]);
        Assert.AreEqual(dictionary["key2"], deserialized["key2"]);
    }

    [TestMethod]
    public void TestTupleAsArrayConverter_SerializeAndDeserialize()
    {
        // Arrange
        var tuple = (1, "value", 3.14);

        var options = new JsonSerializerOptions();
        options.Converters.Add(new TupleAsArrayConverter<int, string, double>.ValueTuple());

        // Act
        var json = JsonSerializer.Serialize(tuple, options);
        var deserialized = JsonSerializer.Deserialize<(int, string, double)>(json, options);

        // Assert
        Assert.AreEqual(tuple, deserialized);
    }
}