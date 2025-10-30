using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class EnumerableExtensionsTests
{
    [TestMethod]
    public void GroupByToDictionary_GroupsByKey_CreatesDictionary()
    {
        var items = new[] { "apple", "apricot", "banana", "blueberry", "cherry" };
        
        var result = items.GroupByToDictionary(s => s[0]);
        
        Assert.HasCount(3, result);
        Assert.IsTrue(result.ContainsKey('a'));
        Assert.IsTrue(result.ContainsKey('b'));
        Assert.IsTrue(result.ContainsKey('c'));
        Assert.HasCount(2, result['a']);
        Assert.HasCount(2, result['b']);
        Assert.HasCount(1, result['c']);
    }

    [TestMethod]
    public void GroupByToDictionary_GroupsIntegers_WorksCorrectly()
    {
        var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        
        var result = numbers.GroupByToDictionary(n => n % 3);
        
        Assert.HasCount(3, result);
        Assert.HasCount(3, result[0]); // 3, 6, 9
        Assert.HasCount(4, result[1]); // 1, 4, 7, 10
        Assert.HasCount(3, result[2]); // 2, 5, 8
    }

    [TestMethod]
    public void GroupByToDictionary_WithCustomComparer_UsesComparer()
    {
        var items = new[] { "Apple", "apple", "APPLE", "Banana" };
        
        var result = items.GroupByToDictionary(
            s => s,
            StringComparer.OrdinalIgnoreCase);
        
        Assert.HasCount(2, result);
        Assert.IsTrue(result.ContainsKey("apple"));
        Assert.IsTrue(result.ContainsKey("banana"));
        Assert.HasCount(3, result["apple"]);
    }

    [TestMethod]
    public void GroupByToDictionary_EmptyEnumerable_ReturnsEmptyDictionary()
    {
        var items = Array.Empty<string>();
        
        var result = items.GroupByToDictionary(s => s[0]);
        
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void GroupByToDictionary_WithResultSelector_TransformsValues()
    {
        var items = new[] { "apple", "apricot", "banana", "blueberry" };
        
        var result = items.GroupByToDictionary(
            s => s[0],
            s => s.Length);
        
        Assert.HasCount(2, result);
        Assert.HasCount(2, result['a']);
        CollectionAssert.AreEqual(new[] { 5, 7 }, (System.Collections.ICollection)result['a']);
        Assert.HasCount(2, result['b']);
        CollectionAssert.AreEqual(new[] { 6, 9 }, (System.Collections.ICollection)result['b']);
    }

    [TestMethod]
    public void GroupByToDictionary_WithResultSelector_AndComparer_WorksCorrectly()
    {
        var items = new[] { "Apple", "apple", "Banana", "BANANA" };
        
        var result = items.GroupByToDictionary(
            s => s,
            s => s.Length,
            StringComparer.OrdinalIgnoreCase);
        
        Assert.HasCount(2, result);
        Assert.HasCount(2, result["apple"]);
        Assert.HasCount(2, result["banana"]);
    }

    [TestMethod]
    public void GroupByToDictionary_SingleGroup_WorksCorrectly()
    {
        var items = new[] { 1, 1, 1, 1 };
        
        var result = items.GroupByToDictionary(n => "same");
        
        Assert.HasCount(1, result);
        Assert.HasCount(4, result["same"]);
    }

    [TestMethod]
    public void GroupByToDictionary_ComplexObjects_GroupsCorrectly()
    {
        var people = new[]
        {
            new { Name = "Alice", Age = 25 },
            new { Name = "Bob", Age = 30 },
            new { Name = "Charlie", Age = 25 },
            new { Name = "David", Age = 30 }
        };
        
        var result = people.GroupByToDictionary(p => p.Age);
        
        Assert.HasCount(2, result);
        Assert.HasCount(2, result[25]);
        Assert.HasCount(2, result[30]);
    }

    [TestMethod]
    public void GroupByToDictionary_ReturnsReadOnlyList()
    {
        var items = new[] { 1, 2, 3 };
        
        var result = items.GroupByToDictionary(n => "key");
        
        Assert.IsInstanceOfType(result["key"], typeof(IReadOnlyList<int>));
    }
}

