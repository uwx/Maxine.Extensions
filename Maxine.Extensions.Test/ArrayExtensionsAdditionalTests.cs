using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class ArrayExtensionsAdditionalTests
{
    [TestMethod]
    public void Replace_SingleOccurrence_ReplacesCorrectly()
    {
        var arr = new[] { 1, 2, 3, 4, 5 };
        var result = arr.Replace([2, 3], [8, 9]);
        
        CollectionAssert.AreEqual(new[] { 1, 8, 9, 4, 5 }, result);
    }

    [TestMethod]
    public void Replace_MultipleOccurrences_ReplacesAll()
    {
        var arr = new[] { 1, 2, 1, 2, 1, 2 };
        var result = arr.Replace([1, 2], [9]);
        
        CollectionAssert.AreEqual(new[] { 9, 9, 9 }, result);
    }

    [TestMethod]
    public void Replace_NoOccurrence_ReturnsOriginal()
    {
        var arr = new[] { 1, 2, 3, 4, 5 };
        var result = arr.Replace([6, 7], [8, 9]);
        
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, result);
    }

    [TestMethod]
    public void Replace_EmptyFind_ReturnsOriginal()
    {
        var arr = new[] { 1, 2, 3 };
        var result = arr.Replace([], [9]);
        
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
    }

    [TestMethod]
    public void Replace_EmptyReplace_RemovesOccurrences()
    {
        var arr = new[] { 1, 2, 3, 2, 3, 4 };
        var result = arr.Replace([2, 3], []);
        
        CollectionAssert.AreEqual(new[] { 1, 4 }, result);
    }

    [TestMethod]
    public void Replace_EmptyArray_ReturnsEmpty()
    {
        var arr = Array.Empty<int>();
        var result = arr.Replace([1], [2]);
        
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void Replace_LongerReplacement_ExpandsArray()
    {
        var arr = new[] { 1, 2, 3 };
        var result = arr.Replace([2], [7, 8, 9]);
        
        CollectionAssert.AreEqual(new[] { 1, 7, 8, 9, 3 }, result);
    }

    [TestMethod]
    public void Replace_ShorterReplacement_ShrinksArray()
    {
        var arr = new[] { 1, 2, 3, 4, 5 };
        var result = arr.Replace([2, 3, 4], [8]);
        
        CollectionAssert.AreEqual(new[] { 1, 8, 5 }, result);
    }

    [TestMethod]
    public void Replace_EntireArray_ReplacesAll()
    {
        var arr = new[] { 1, 2, 3 };
        var result = arr.Replace([1, 2, 3], [9]);
        
        CollectionAssert.AreEqual(new[] { 9 }, result);
    }

    [TestMethod]
    public void Replace_AtBeginning_ReplacesCorrectly()
    {
        var arr = new[] { 1, 2, 3, 4 };
        var result = arr.Replace([1, 2], [9]);
        
        CollectionAssert.AreEqual(new[] { 9, 3, 4 }, result);
    }

    [TestMethod]
    public void Replace_AtEnd_ReplacesCorrectly()
    {
        var arr = new[] { 1, 2, 3, 4 };
        var result = arr.Replace([3, 4], [9]);
        
        CollectionAssert.AreEqual(new[] { 1, 2, 9 }, result);
    }

    [TestMethod]
    public void Replace_StringArray_WorksCorrectly()
    {
        var arr = new[] { "hello", "world", "foo", "bar" };
        var result = arr.Replace(["world", "foo"], ["test"]);
        
        CollectionAssert.AreEqual(new[] { "hello", "test", "bar" }, result);
    }

    [TestMethod]
    public void Replace_OverlappingPattern_ReplacesNonOverlapping()
    {
        var arr = new[] { 1, 1, 1, 1 };
        var result = arr.Replace([1, 1], [2]);
        
        // Should replace first occurrence, then continue from after it
        CollectionAssert.AreEqual(new[] { 2, 2 }, result);
    }

    [TestMethod]
    public void Replace_SingleElement_WorksCorrectly()
    {
        var arr = new[] { 1, 2, 1, 3, 1 };
        var result = arr.Replace([1], [9]);
        
        CollectionAssert.AreEqual(new[] { 9, 2, 9, 3, 9 }, result);
    }

    [TestMethod]
    public void Replace_LongPattern_WorksCorrectly()
    {
        var arr = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var result = arr.Replace([3, 4, 5, 6], [99]);
        
        CollectionAssert.AreEqual(new[] { 1, 2, 99, 7, 8 }, result);
    }

    [TestMethod]
    public void Replace_ConsecutiveOccurrences_ReplacesAll()
    {
        var arr = new[] { 1, 2, 2, 3 };
        var result = arr.Replace([2], [9]);
        
        CollectionAssert.AreEqual(new[] { 1, 9, 9, 3 }, result);
    }

    [TestMethod]
    public void Replace_CharArray_WorksCorrectly()
    {
        var arr = new[] { 'a', 'b', 'c', 'd' };
        var result = arr.Replace(['b', 'c'], ['x']);
        
        CollectionAssert.AreEqual(new[] { 'a', 'x', 'd' }, result);
    }
}
