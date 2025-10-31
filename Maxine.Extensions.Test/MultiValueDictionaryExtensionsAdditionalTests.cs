using Microsoft.Collections.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class MultiValueDictionaryExtensionsAdditionalTests
{
    [TestMethod]
    public void TryGetValues_WithValues_ReturnsValuesAndSetsHasValuesToTrue()
    {
        var dict = new MultiValueDictionary<string, int>
        {
            { "key1", 1 },
            { "key1", 2 },
            { "key1", 3 }
        };

        var result = dict.TryGetValues("key1", out var hasValues);
        
        Assert.IsTrue(hasValues);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result.ToList());
    }

    [TestMethod]
    public void TryGetValues_WithoutValues_ReturnsEmptyAndSetsHasValuesToFalse()
    {
        var dict = new MultiValueDictionary<string, int>
        {
            { "key1", 1 }
        };

        var result = dict.TryGetValues("key2", out var hasValues);
        
        Assert.IsFalse(hasValues);
        CollectionAssert.AreEqual(Array.Empty<int>(), result.ToList());
    }

    [TestMethod]
    public void TryGetValues_EmptyDictionary_ReturnsEmptyAndSetsHasValuesToFalse()
    {
        var dict = new MultiValueDictionary<string, int>();

        var result = dict.TryGetValues("anyKey", out var hasValues);
        
        Assert.IsFalse(hasValues);
        CollectionAssert.AreEqual(Array.Empty<int>(), result.ToList());
    }

    [TestMethod]
    public void TryGetValues_SingleValue_ReturnsValueAndSetsHasValuesToTrue()
    {
        var dict = new MultiValueDictionary<string, int>
        {
            { "key1", 42 }
        };

        var result = dict.TryGetValues("key1", out var hasValues);
        
        Assert.IsTrue(hasValues);
        CollectionAssert.AreEqual(new[] { 42 }, result.ToList());
    }

    [TestMethod]
    public void TryGetValues_MultipleKeysWithMultipleValues_ReturnsCorrectValuesForEachKey()
    {
        var dict = new MultiValueDictionary<int, string>
        {
            { 1, "a" },
            { 1, "b" },
            { 2, "c" },
            { 2, "d" },
            { 2, "e" }
        };

        var result1 = dict.TryGetValues(1, out var hasValues1);
        var result2 = dict.TryGetValues(2, out var hasValues2);
        
        Assert.IsTrue(hasValues1);
        Assert.IsTrue(hasValues2);
        CollectionAssert.AreEqual(new[] { "a", "b" }, result1.ToList());
        CollectionAssert.AreEqual(new[] { "c", "d", "e" }, result2.ToList());
    }

    [TestMethod]
    public void TryGetValues_WithComplexType_ReturnsValuesCorrectly()
    {
        var dict = new MultiValueDictionary<string, (int Id, string Name)>
        {
            { "group1", (1, "Alice") },
            { "group1", (2, "Bob") },
            { "group2", (3, "Charlie") }
        };

        var result = dict.TryGetValues("group1", out var hasValues);
        
        Assert.IsTrue(hasValues);
        var list = result.ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual((1, "Alice"), list[0]);
        Assert.AreEqual((2, "Bob"), list[1]);
    }

    [TestMethod]
    public void TryGetValues_CanIterateResultMultipleTimes_WhenHasValues()
    {
        var dict = new MultiValueDictionary<string, int>
        {
            { "key1", 10 },
            { "key1", 20 }
        };

        var result = dict.TryGetValues("key1", out var hasValues);
        
        Assert.IsTrue(hasValues);
        
        // First iteration
        var firstIteration = result.ToList();
        Assert.AreEqual(2, firstIteration.Count);
        
        // Second iteration
        var secondIteration = result.ToList();
        Assert.AreEqual(2, secondIteration.Count);
        
        CollectionAssert.AreEqual(firstIteration, secondIteration);
    }

    [TestMethod]
    public void TryGetValues_CanIterateResultMultipleTimes_WhenNoValues()
    {
        var dict = new MultiValueDictionary<string, int>();

        var result = dict.TryGetValues("missing", out var hasValues);
        
        Assert.IsFalse(hasValues);
        
        // Multiple iterations should all be empty
        Assert.AreEqual(0, result.Count());
        Assert.AreEqual(0, result.Count());
    }
}
