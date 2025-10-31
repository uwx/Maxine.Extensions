using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class GroupingExtensionsAdditionalTests
{
    [TestMethod]
    public void Deconstruct_GroupingWithKey_ExtractsKeyAndValues()
    {
        var data = new[] { 1, 2, 3, 4, 5, 6 };
        var grouped = data.GroupBy(x => x % 2).First();

        var (key, values) = grouped;

        Assert.AreEqual(1, key);
        CollectionAssert.AreEqual(new[] { 1, 3, 5 }, values.ToArray());
    }

    [TestMethod]
    public void Deconstruct_MultipleGroupings_WorksCorrectly()
    {
        var data = new[] { "apple", "apricot", "banana", "blueberry", "cherry" };
        var groupings = data.GroupBy(x => x[0]).ToList();

        var (key1, values1) = groupings[0];
        Assert.AreEqual('a', key1);
        CollectionAssert.AreEqual(new[] { "apple", "apricot" }, values1.ToArray());

        var (key2, values2) = groupings[1];
        Assert.AreEqual('b', key2);
        CollectionAssert.AreEqual(new[] { "banana", "blueberry" }, values2.ToArray());

        var (key3, values3) = groupings[2];
        Assert.AreEqual('c', key3);
        CollectionAssert.AreEqual(new[] { "cherry" }, values3.ToArray());
    }

    [TestMethod]
    public void Deconstruct_EmptyGrouping_ReturnsKeyAndEmptyValues()
    {
        var data = Array.Empty<int>();
        var grouped = data.GroupBy(x => x % 2);

        // GroupBy won't create groups for empty collections
        Assert.AreEqual(0, grouped.Count());
    }

    [TestMethod]
    public void Deconstruct_SingleElementGrouping_Works()
    {
        var data = new[] { 42 };
        var grouped = data.GroupBy(x => x).First();

        var (key, values) = grouped;

        Assert.AreEqual(42, key);
        CollectionAssert.AreEqual(new[] { 42 }, values.ToArray());
    }

    [TestMethod]
    public void Deconstruct_UsedInForEach_WorksCorrectly()
    {
        var data = new[] { 1, 2, 3, 4, 5, 6 };
        var grouped = data.GroupBy(x => x % 3);

        var results = new Dictionary<int, int[]>();
        foreach (var (key, values) in grouped)
        {
            results[key] = values.ToArray();
        }

        Assert.AreEqual(3, results.Count);
        CollectionAssert.AreEqual(new[] { 3, 6 }, results[0]);
        CollectionAssert.AreEqual(new[] { 1, 4 }, results[1]);
        CollectionAssert.AreEqual(new[] { 2, 5 }, results[2]);
    }

    [TestMethod]
    public void Deconstruct_ComplexObjects_PreservesKeyAndValues()
    {
        var records = new[]
        {
            new { Name = "Alice", Age = 30 },
            new { Name = "Bob", Age = 25 },
            new { Name = "Charlie", Age = 30 },
            new { Name = "David", Age = 25 }
        };

        var grouped = records.GroupBy(x => x.Age).First();

        var (key, values) = grouped;

        Assert.AreEqual(30, key);
        Assert.AreEqual(2, values.Count());
        Assert.IsTrue(values.Any(x => x.Name == "Alice"));
        Assert.IsTrue(values.Any(x => x.Name == "Charlie"));
    }
}
