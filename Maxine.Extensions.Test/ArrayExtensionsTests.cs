using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class ArrayExtensionsTests
{
    [TestMethod]
    public void Replace_SingleOccurrence_ReplacesValue()
    {
        var array = new int[] { 1, 2, 3, 4, 5 };
        var result = array.Replace([3], [99]);
        
        CollectionAssert.AreEqual(new[] { 1, 2, 99, 4, 5 }, result);
    }

    [TestMethod]
    public void Replace_MultipleOccurrences_ReplacesAll()
    {
        var array = new int[] { 1, 2, 3, 2, 5 };
        var result = array.Replace([2], [99]);
        
        CollectionAssert.AreEqual(new[] { 1, 99, 3, 99, 5 }, result);
    }

    [TestMethod]
    public void Replace_NoOccurrence_ReturnsOriginalArray()
    {
        var array = new int[] { 1, 2, 3, 4, 5 };
        var result = array.Replace([99], [100]);
        
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, result);
    }

    [TestMethod]
    public void Replace_MultipleElementSequence_ReplacesSequence()
    {
        var array = new int[] { 1, 2, 3, 4, 5 };
        var result = array.Replace([2, 3, 4], [99, 100]);
        
        CollectionAssert.AreEqual(new[] { 1, 99, 100, 5 }, result);
    }

    [TestMethod]
    public void Replace_WithEmptyReplacement_RemovesSequence()
    {
        var array = new int[] { 1, 2, 3, 4, 5 };
        var result = array.Replace([2, 3], []);
        
        CollectionAssert.AreEqual(new[] { 1, 4, 5 }, result);
    }

    [TestMethod]
    public void Replace_WithLongerReplacement_ExpandsArray()
    {
        var array = new int[] { 1, 2, 3 };
        var result = array.Replace([2], [10, 20, 30, 40]);
        
        CollectionAssert.AreEqual(new[] { 1, 10, 20, 30, 40, 3 }, result);
    }

    [TestMethod]
    public void Replace_EntireArray_ReplacesEverything()
    {
        var array = new int[] { 1, 2, 3 };
        var result = array.Replace([1, 2, 3], [99]);
        
        CollectionAssert.AreEqual(new[] { 99 }, result);
    }

    [TestMethod]
    public void Replace_OverlappingMatches_ReplacesFirst()
    {
        var array = new int[] { 1, 1, 1, 2 };
        var result = array.Replace([1, 1], [5]);
        
        // Should replace first occurrence, then continue from after it
        CollectionAssert.AreEqual(new[] { 5, 1, 2 }, result);
    }

    [TestMethod]
    public void Replace_StringArray_ReplacesCorrectly()
    {
        var array = new string[] { "hello", "world", "test" };
        var result = array.Replace(new[] { "world" }, new[] { "universe" });
        
        CollectionAssert.AreEqual(new[] { "hello", "universe", "test" }, result);
    }

    [TestMethod]
    public void Replace_EmptyArray_ReturnsEmpty()
    {
        var array = Array.Empty<int>();
        var result = array.Replace([1], [2]);
        
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void Replace_AtBeginning_ReplacesCorrectly()
    {
        var array = new int[] { 1, 2, 3, 4, 5 };
        var result = array.Replace([1, 2], [99]);
        
        CollectionAssert.AreEqual(new[] { 99, 3, 4, 5 }, result);
    }

    [TestMethod]
    public void Replace_AtEnd_ReplacesCorrectly()
    {
        var array = new int[] { 1, 2, 3, 4, 5 };
        var result = array.Replace([4, 5], [99]);
        
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 99 }, result);
    }

    [TestMethod]
    public void Replace_ConsecutiveMatches_ReplacesAll()
    {
        var array = new int[] { 1, 2, 1, 2, 3 };
        var result = array.Replace([1, 2], [99]);
        
        CollectionAssert.AreEqual(new[] { 99, 99, 3 }, result);
    }

    [TestMethod]
    public void Replace_SingleElement_WithSingleElement_WorksCorrectly()
    {
        var array = new int[] { 5 };
        var result = array.Replace([5], [10]);
        
        CollectionAssert.AreEqual(new[] { 10 }, result);
    }

    [TestMethod]
    public void Replace_CharArray_ReplacesCorrectly()
    {
        var array = new char[] { 'a', 'b', 'c', 'd' };
        var result = array.Replace(['b', 'c'], ['x', 'y', 'z']);
        
        CollectionAssert.AreEqual(new[] { 'a', 'x', 'y', 'z', 'd' }, result);
    }
}

