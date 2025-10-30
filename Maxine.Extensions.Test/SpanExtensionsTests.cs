using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class SpanExtensionsTests
{
    [TestMethod]
    public void WithIndex_EnumeratesWithIndex()
    {
        var items = new[] { "a", "b", "c" };
        var result = new List<(int Index, string Element)>();
        
        foreach (var (index, element) in items.WithIndex())
        {
            result.Add((index, element));
        }
        
        Assert.HasCount(3, result);
        Assert.AreEqual((0, "a"), result[0]);
        Assert.AreEqual((1, "b"), result[1]);
        Assert.AreEqual((2, "c"), result[2]);
    }

    [TestMethod]
    public void WithIndex_WithStartIndex_StartsFromSpecifiedIndex()
    {
        var items = new[] { "x", "y", "z" };
        var result = new List<(int Index, string Element)>();
        
        foreach (var (index, element) in items.WithIndex(start: 10))
        {
            result.Add((index, element));
        }
        
        Assert.HasCount(3, result);
        Assert.AreEqual((10, "x"), result[0]);
        Assert.AreEqual((11, "y"), result[1]);
        Assert.AreEqual((12, "z"), result[2]);
    }

    [TestMethod]
    public void WithIndex_EmptyEnumerable_ReturnsEmpty()
    {
        var items = Array.Empty<string>();
        var result = new List<(int Index, string Element)>();
        
        foreach (var item in items.WithIndex())
        {
            result.Add(item);
        }
        
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void WithIndex_SingleElement_ReturnsOneItem()
    {
        var items = new[] { "only" };
        var result = new List<(int Index, string Element)>();
        
        foreach (var (index, element) in items.WithIndex())
        {
            result.Add((index, element));
        }
        
        Assert.HasCount(1, result);
        Assert.AreEqual((0, "only"), result[0]);
    }

    [TestMethod]
    public void FindIndex2_FindsFirstMatch()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        
        var index = items.FindIndex2(x => x > 3);
        
        Assert.AreEqual(3, index); // Index of 4
    }

    [TestMethod]
    public void FindIndex2_NoMatch_ReturnsMinusOne()
    {
        var items = new[] { 1, 2, 3 };
        
        var index = items.FindIndex2(x => x > 10);
        
        Assert.AreEqual(-1, index);
    }

    [TestMethod]
    public void FindIndex2_FirstElement_ReturnsZero()
    {
        var items = new[] { 5, 2, 3 };
        
        var index = items.FindIndex2(x => x == 5);
        
        Assert.AreEqual(0, index);
    }

    [TestMethod]
    public void FindIndex2_EmptyEnumerable_ReturnsMinusOne()
    {
        var items = Array.Empty<int>();
        
        var index = items.FindIndex2(x => x > 0);
        
        Assert.AreEqual(-1, index);
    }

    [TestMethod]
    public void FindIndex2_WithStrings_FindsMatch()
    {
        var items = new[] { "apple", "banana", "cherry" };
        
        var index = items.FindIndex2(x => x.StartsWith("b"));
        
        Assert.AreEqual(1, index);
    }

    [TestMethod]
    public void Truncate_ShorterString_ReturnsOriginal()
    {
        var str = "Hello";
        var result = str.Truncate(10);
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void Truncate_LongerString_TruncatesCorrectly()
    {
        var str = "Hello World";
        var result = str.Truncate(5);
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void Truncate_ExactLength_ReturnsOriginal()
    {
        var str = "Test";
        var result = str.Truncate(4);
        Assert.AreEqual("Test", result);
    }

    [TestMethod]
    public void WithIndex_IntegerSequence_WorksCorrectly()
    {
        var items = Enumerable.Range(100, 5);
        var results = items.WithIndex().ToList();
        
        Assert.HasCount(5, results);
        Assert.AreEqual((0, 100), results[0]);
        Assert.AreEqual((1, 101), results[1]);
        Assert.AreEqual((4, 104), results[4]);
    }

    [TestMethod]
    public void WithIndex_CanBeEnumeratedMultipleTimes()
    {
        var items = new[] { "a", "b" };
        var enumerable = items.WithIndex();
        
        var first = enumerable.ToList();
        var second = enumerable.ToList();
        
        CollectionAssert.AreEqual(first, second);
    }
}
