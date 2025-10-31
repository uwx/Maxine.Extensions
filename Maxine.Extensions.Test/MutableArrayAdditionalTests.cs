using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions.Collections;

namespace Maxine.Extensions.Test;

[TestClass]
public class MutableArrayAdditionalTests
{
    [TestMethod]
    public void Constructor_Default_CreatesEmptyArray()
    {
        var array = new MutableArray<int>();

        Assert.IsTrue(array.IsEmpty);
        Assert.AreEqual(0, array.Length);
    }

    [TestMethod]
    public void Constructor_WithArray_WrapsArray()
    {
        var source = new[] { 1, 2, 3 };
        var array = new MutableArray<int>(source);

        Assert.AreSame(source, array.BackingArray);
        Assert.AreEqual(3, array.Length);
    }

    [TestMethod]
    public void ImplicitConversion_FromArray_CreatesWrapper()
    {
        int[] source = [1, 2, 3];
        MutableArray<int> array = source;

        Assert.AreSame(source, array.BackingArray);
    }

    [TestMethod]
    public void ImplicitConversion_ToArray_ReturnsBackingArray()
    {
        var source = new[] { 1, 2, 3 };
        var mutable = new MutableArray<int>(source);
        
        int[] array = mutable;

        Assert.AreSame(source, array);
    }

    [TestMethod]
    public void Indexer_Get_ReturnsCorrectElement()
    {
        var array = new MutableArray<int>([10, 20, 30]);

        Assert.AreEqual(10, array[0]);
        Assert.AreEqual(20, array[1]);
        Assert.AreEqual(30, array[2]);
    }

    [TestMethod]
    public void Indexer_Set_ModifiesElement()
    {
        var array = new MutableArray<int>([10, 20, 30]);
        
        array[1] = 99;

        Assert.AreEqual(99, array[1]);
    }

    [TestMethod]
    public void IsDefault_NullBackingArray_ReturnsTrue()
    {
        var array = new MutableArray<int>(null!);

        Assert.IsTrue(array.IsDefault);
    }

    [TestMethod]
    public void IsDefault_NonNullBackingArray_ReturnsFalse()
    {
        var array = new MutableArray<int>([1, 2, 3]);

        Assert.IsFalse(array.IsDefault);
    }

    [TestMethod]
    public void IsEmpty_EmptyArray_ReturnsTrue()
    {
        var array = new MutableArray<int>([]);

        Assert.IsTrue(array.IsEmpty);
    }

    [TestMethod]
    public void IsEmpty_NonEmptyArray_ReturnsFalse()
    {
        var array = new MutableArray<int>([1]);

        Assert.IsFalse(array.IsEmpty);
    }

    [TestMethod]
    public void AsSpan_ReturnsCorrectSpan()
    {
        var array = new MutableArray<int>([1, 2, 3]);
        
        var span = array.AsSpan();

        Assert.AreEqual(3, span.Length);
        Assert.AreEqual(1, span[0]);
        Assert.AreEqual(3, span[2]);
    }

    [TestMethod]
    public void AsMemory_ReturnsCorrectMemory()
    {
        var array = new MutableArray<int>([1, 2, 3]);
        
        var memory = array.AsMemory();

        Assert.AreEqual(3, memory.Length);
        Assert.AreEqual(1, memory.Span[0]);
        Assert.AreEqual(3, memory.Span[2]);
    }

    [TestMethod]
    public void IndexOf_ItemExists_ReturnsIndex()
    {
        var array = new MutableArray<string>(["apple", "banana", "cherry"]);

        var index = array.IndexOf("banana");

        Assert.AreEqual(1, index);
    }

    [TestMethod]
    public void IndexOf_ItemDoesNotExist_ReturnsNegativeOne()
    {
        var array = new MutableArray<string>(["apple", "banana", "cherry"]);

        var index = array.IndexOf("grape");

        Assert.AreEqual(-1, index);
    }

    [TestMethod]
    public void GetEnumerator_IteratesAllElements()
    {
        var array = new MutableArray<int>([1, 2, 3]);
        var list = new List<int>();

        foreach (var item in array)
        {
            list.Add(item);
        }

        Assert.AreEqual(3, list.Count);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, list);
    }

    [TestMethod]
    public void Count_ReadOnlyCollection_ReturnsLength()
    {
        var array = new MutableArray<int>([1, 2, 3]);
        var readOnly = (System.Collections.Generic.IReadOnlyCollection<int>)array;

        Assert.AreEqual(3, readOnly.Count);
    }

    [TestMethod]
    public void Count_Collection_ReturnsLength()
    {
        var array = new MutableArray<int>([1, 2, 3]);
        var collection = (System.Collections.Generic.ICollection<int>)array;

        Assert.AreEqual(3, collection.Count);
    }

    [TestMethod]
    public void IsReadOnly_ReturnsTrue()
    {
        var array = new MutableArray<int>([1, 2, 3]);
        var collection = (System.Collections.Generic.ICollection<int>)array;

        Assert.IsTrue(collection.IsReadOnly);
    }

    [TestMethod]
    public void Length_EmptyArray_ReturnsZero()
    {
        var array = new MutableArray<int>([]);

        Assert.AreEqual(0, array.Length);
    }

    [TestMethod]
    public void Length_NonEmptyArray_ReturnsCorrectLength()
    {
        var array = new MutableArray<int>([1, 2, 3, 4, 5]);

        Assert.AreEqual(5, array.Length);
    }
}
