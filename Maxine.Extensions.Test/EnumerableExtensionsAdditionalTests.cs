using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class EnumerableExtensionsAdditionalTests
{
    [TestMethod]
    public void GroupByToDictionary_SimpleGrouping_GroupsCorrectly()
    {
        var items = new[] { 1, 2, 3, 4, 5, 6 };
        var result = items.GroupByToDictionary(x => x % 2);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(3, result[0].Count); // Even numbers
        Assert.AreEqual(3, result[1].Count); // Odd numbers
    }

    [TestMethod]
    public void GroupByToDictionary_StringGrouping_GroupsCorrectly()
    {
        var items = new[] { "apple", "apricot", "banana", "blueberry", "cherry" };
        var result = items.GroupByToDictionary(x => x[0]);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(2, result['a'].Count);
        Assert.AreEqual(2, result['b'].Count);
        Assert.AreEqual(1, result['c'].Count);
    }

    [TestMethod]
    public void GroupByToDictionary_EmptyEnumerable_ReturnsEmptyDict()
    {
        var items = Array.Empty<int>();
        var result = items.GroupByToDictionary(x => x);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GroupByToDictionary_SingleGroup_CreatesOneEntry()
    {
        var items = new[] { 1, 1, 1, 1 };
        var result = items.GroupByToDictionary(x => x);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(4, result[1].Count);
    }

    [TestMethod]
    public void GroupByToDictionary_WithComparer_UsesCaseInsensitive()
    {
        var items = new[] { "Apple", "apple", "APPLE", "Banana" };
        var result = items.GroupByToDictionary(
            x => x,
            StringComparer.OrdinalIgnoreCase
        );

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.ContainsKey("apple"));
        Assert.AreEqual(3, result["apple"].Count);
    }

    [TestMethod]
    public void GroupByToDictionary_PreservesOrder_WithinGroups()
    {
        var items = new[] { 1, 3, 2, 4, 1, 3, 2 };
        var result = items.GroupByToDictionary(x => x % 2);

        CollectionAssert.AreEqual(new[] { 2, 4, 2 }, (List<int>)result[0]);
        CollectionAssert.AreEqual(new[] { 1, 3, 1, 3 }, (List<int>)result[1]);
    }

    [TestMethod]
    public void GroupByToDictionary_WithResultSelector_TransformsValues()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        var result = items.GroupByToDictionary(
            x => x % 2,
            x => x * 10
        );

        Assert.AreEqual(2, result.Count);
        CollectionAssert.AreEqual(new[] { 20, 40 }, (List<int>)result[0]);
        CollectionAssert.AreEqual(new[] { 10, 30, 50 }, (List<int>)result[1]);
    }

    [TestMethod]
    public void GroupByToDictionary_WithResultSelector_StringTransform()
    {
        var items = new[] { "a", "bb", "ccc", "dd", "e" };
        var result = items.GroupByToDictionary(
            x => x.Length,
            x => x.ToUpper()
        );

        Assert.AreEqual(3, result.Count);
        CollectionAssert.AreEqual(new[] { "A", "E" }, (List<string>)result[1]);
        CollectionAssert.AreEqual(new[] { "BB", "DD" }, (List<string>)result[2]);
        CollectionAssert.AreEqual(new[] { "CCC" }, (List<string>)result[3]);
    }

    [TestMethod]
    public void GroupByToDictionary_WithResultSelector_EmptyEnumerable()
    {
        var items = Array.Empty<int>();
        var result = items.GroupByToDictionary(
            x => x,
            x => x.ToString()
        );

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GroupByToDictionary_WithResultSelector_AndComparer()
    {
        var items = new[] { "Apple", "apple", "Banana", "BANANA" };
        var result = items.GroupByToDictionary(
            x => x,
            x => x.Length,
            StringComparer.OrdinalIgnoreCase
        );

        Assert.AreEqual(2, result.Count);
        CollectionAssert.AreEqual(new[] { 5, 5 }, (List<int>)result["apple"]);
        CollectionAssert.AreEqual(new[] { 6, 6 }, (List<int>)result["banana"]);
    }

    [TestMethod]
    public void GroupByToDictionary_ComplexObjects_GroupsByProperty()
    {
        var items = new[]
        {
            (Name: "Alice", Age: 25),
            (Name: "Bob", Age: 30),
            (Name: "Charlie", Age: 25),
            (Name: "David", Age: 30)
        };

        var result = items.GroupByToDictionary(x => x.Age);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2, result[25].Count);
        Assert.AreEqual(2, result[30].Count);
    }

    [TestMethod]
    public void GroupByToDictionary_ManyGroups_HandlesCorrectly()
    {
        var items = Enumerable.Range(0, 100);
        var result = items.GroupByToDictionary(x => x % 10);

        Assert.AreEqual(10, result.Count);
        foreach (var group in result.Values)
        {
            Assert.AreEqual(10, group.Count);
        }
    }

    [TestMethod]
    public void GroupByToDictionary_ReturnsReadOnlyList()
    {
        var items = new[] { 1, 2, 3 };
        var result = items.GroupByToDictionary(x => x % 2);

        foreach (var list in result.Values)
        {
            Assert.IsInstanceOfType<IReadOnlyList<int>>(list);
        }
    }
}
