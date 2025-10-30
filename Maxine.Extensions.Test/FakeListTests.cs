using Maxine.Extensions.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class FakeListTests
{
    // Concrete implementation for testing
    private class SimpleFakeList : FakeList<int>
    {
        private readonly List<int> _items = new();

        public override IEnumerator<int> GetEnumerator() => _items.GetEnumerator();
        public override int Count => _items.Count;
        public override void Add(int item) => _items.Add(item);
        public override void Clear() => _items.Clear();
        public override bool Contains(int item) => _items.Contains(item);
        public override int IndexOf(int item) => _items.IndexOf(item);
        public override void Insert(int index, int item) => _items.Insert(index, item);
        public override void RemoveAt(int index) => _items.RemoveAt(index);

        public override int this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }
    }

    [TestMethod]
    public void FakeList_Add_AddsItem()
    {
        var list = new SimpleFakeList();
        
        list.Add(42);
        
        Assert.AreEqual(1, list.Count);
        Assert.IsTrue(list.Contains(42));
    }

    [TestMethod]
    public void FakeList_Clear_RemovesAllItems()
    {
        var list = new SimpleFakeList();
        list.Add(1);
        list.Add(2);
        
        list.Clear();
        
        Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    public void FakeList_Contains_FindsExistingItem()
    {
        var list = new SimpleFakeList();
        list.Add(42);
        
        Assert.IsTrue(list.Contains(42));
        Assert.IsFalse(list.Contains(99));
    }

    [TestMethod]
    public void FakeList_IndexOf_ReturnsCorrectIndex()
    {
        var list = new SimpleFakeList();
        list.Add(10);
        list.Add(20);
        list.Add(30);
        
        Assert.AreEqual(1, list.IndexOf(20));
        Assert.AreEqual(-1, list.IndexOf(99));
    }

    [TestMethod]
    public void FakeList_Insert_InsertsAtCorrectPosition()
    {
        var list = new SimpleFakeList();
        list.Add(10);
        list.Add(30);
        
        list.Insert(1, 20);
        
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(20, list[1]);
    }

    [TestMethod]
    public void FakeList_RemoveAt_RemovesItemAtIndex()
    {
        var list = new SimpleFakeList();
        list.Add(10);
        list.Add(20);
        list.Add(30);
        
        list.RemoveAt(1);
        
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual(30, list[1]);
    }

    [TestMethod]
    public void FakeList_Remove_RemovesFirstOccurrence()
    {
        var list = new SimpleFakeList();
        list.Add(10);
        list.Add(20);
        list.Add(20);
        
        var removed = list.Remove(20);
        
        Assert.IsTrue(removed);
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual(1, list.IndexOf(20));
    }

    [TestMethod]
    public void FakeList_Remove_NonExistentItem_ReturnsFalse()
    {
        var list = new SimpleFakeList();
        list.Add(10);
        
        var removed = list.Remove(99);
        
        Assert.IsFalse(removed);
        Assert.AreEqual(1, list.Count);
    }

    [TestMethod]
    public void FakeList_Indexer_GetSet_WorksCorrectly()
    {
        var list = new SimpleFakeList();
        list.Add(10);
        list.Add(20);
        
        Assert.AreEqual(10, list[0]);
        Assert.AreEqual(20, list[1]);
        
        list[0] = 99;
        Assert.AreEqual(99, list[0]);
    }

    [TestMethod]
    public void FakeList_CopyTo_CopiesItemsToArray()
    {
        var list = new SimpleFakeList();
        list.Add(10);
        list.Add(20);
        list.Add(30);
        
        var array = new int[5];
        list.CopyTo(array, 1);
        
        Assert.AreEqual(0, array[0]);
        Assert.AreEqual(10, array[1]);
        Assert.AreEqual(20, array[2]);
        Assert.AreEqual(30, array[3]);
    }

    [TestMethod]
    public void FakeList_GetEnumerator_EnumeratesAllItems()
    {
        var list = new SimpleFakeList();
        list.Add(10);
        list.Add(20);
        list.Add(30);
        
        var items = new List<int>();
        foreach (var item in list)
        {
            items.Add(item);
        }
        
        CollectionAssert.AreEqual(new[] { 10, 20, 30 }, items);
    }

    [TestMethod]
    public void FakeAppendOnlyList_Add_WorksCorrectly()
    {
        var sourceList = new List<int>();
        var appendOnlyList = FakeAppendOnlyList<string>.WrapWithConversionTo(sourceList, s => int.Parse(s));
        
        appendOnlyList.Add("42");
        
        Assert.HasCount(1, sourceList);
        Assert.AreEqual(42, sourceList[0]);
    }

    [TestMethod]
    public void FakeAppendOnlyList_GetEnumerator_ThrowsNotSupportedException()
    {
        var sourceList = new List<int>();
        var appendOnlyList = FakeAppendOnlyList<string>.WrapWithConversionTo(sourceList, s => int.Parse(s));
        
        try
        {
            appendOnlyList.GetEnumerator();
            Assert.Fail("Expected NotSupportedException");
        }
        catch (NotSupportedException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void FakeAppendOnlyList_Contains_ThrowsNotSupportedException()
    {
        var sourceList = new List<int>();
        var appendOnlyList = FakeAppendOnlyList<string>.WrapWithConversionTo(sourceList, s => int.Parse(s));
        
        try
        {
            appendOnlyList.Contains("42");
            Assert.Fail("Expected NotSupportedException");
        }
        catch (NotSupportedException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void FakeAppendOnlyList_Indexer_ThrowsNotSupportedException()
    {
        var sourceList = new List<int>();
        var appendOnlyList = FakeAppendOnlyList<string>.WrapWithConversionTo(sourceList, s => int.Parse(s));
        
        try
        {
            var _ = appendOnlyList[0];
            Assert.Fail("Expected NotSupportedException");
        }
        catch (NotSupportedException)
        {
            // Expected
        }
    }
}
