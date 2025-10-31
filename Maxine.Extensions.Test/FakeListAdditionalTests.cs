namespace Maxine.Extensions.Test;

[TestClass]
public class FakeListAdditionalTests
{
    private class TestFakeList : Collections.FakeList<int>
    {
        private readonly List<int> _inner = new();

        public override int Count => _inner.Count;

        public override void Add(int item) => _inner.Add(item);

        public override void Clear() => _inner.Clear();

        public override bool Contains(int item) => _inner.Contains(item);

        public override IEnumerator<int> GetEnumerator() => _inner.GetEnumerator();

        public override int IndexOf(int item) => _inner.IndexOf(item);

        public override void Insert(int index, int item) => _inner.Insert(index, item);

        public override void RemoveAt(int index) => _inner.RemoveAt(index);

        public override int this[int index]
        {
            get => _inner[index];
            set => _inner[index] = value;
        }
    }

    [TestMethod]
    public void CopyTo_CopiesElements()
    {
        var list = new TestFakeList();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        var array = new int[5];
        list.CopyTo(array, 1);

        Assert.AreEqual(0, array[0]);
        Assert.AreEqual(1, array[1]);
        Assert.AreEqual(2, array[2]);
        Assert.AreEqual(3, array[3]);
        Assert.AreEqual(0, array[4]);
    }

    [TestMethod]
    public void CopyTo_NullArray_ThrowsException()
    {
        var list = new TestFakeList();
        Assert.Throws<ArgumentNullException>(() => list.CopyTo(null!, 0));
    }

    [TestMethod]
    public void CopyTo_NegativeIndex_ThrowsException()
    {
        var list = new TestFakeList();
        var array = new int[5];
        Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(array, -1));
    }

    [TestMethod]
    public void CopyTo_InsufficientSpace_ThrowsException()
    {
        var list = new TestFakeList();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        var array = new int[3];
        Assert.Throws<ArgumentException>(() => list.CopyTo(array, 1));
    }

    [TestMethod]
    public void Remove_ExistingItem_ReturnsTrue()
    {
        var list = new TestFakeList();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        var result = list.Remove(2);

        Assert.IsTrue(result);
        Assert.AreEqual(2, list.Count);
        Assert.IsFalse(list.Contains(2));
    }

    [TestMethod]
    public void Remove_NonExistingItem_ReturnsFalse()
    {
        var list = new TestFakeList();
        list.Add(1);
        list.Add(2);

        var result = list.Remove(99);

        Assert.IsFalse(result);
        Assert.AreEqual(2, list.Count);
    }

    [TestMethod]
    public void Remove_FirstOccurrence_RemovesOnlyFirst()
    {
        var list = new TestFakeList();
        list.Add(1);
        list.Add(2);
        list.Add(2);
        list.Add(3);

        list.Remove(2);

        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(1, list[0]);
        Assert.AreEqual(2, list[1]);
        Assert.AreEqual(3, list[2]);
    }

    [TestMethod]
    public void IsReadOnly_ReturnsFalse()
    {
        var list = new TestFakeList();
        Assert.IsFalse(list.IsReadOnly);
    }

    [TestMethod]
    public void FakeAppendOnlyList_WrapWithConversionTo_WorksCorrectly()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        wrapper.Add(42);
        wrapper.Add(100);

        Assert.AreEqual(2, innerList.Count);
        Assert.AreEqual("42", innerList[0]);
        Assert.AreEqual("100", innerList[1]);
    }

    [TestMethod]
    public void FakeAppendOnlyList_GetEnumerator_ThrowsException()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        Assert.Throws<NotSupportedException>(() => wrapper.GetEnumerator());
    }

    [TestMethod]
    public void FakeAppendOnlyList_Contains_ThrowsException()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        Assert.Throws<NotSupportedException>(() => wrapper.Contains(42));
    }

    [TestMethod]
    public void FakeAppendOnlyList_CopyTo_ThrowsException()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        var array = new int[5];
        Assert.Throws<NotSupportedException>(() => wrapper.CopyTo(array, 0));
    }

    [TestMethod]
    public void FakeAppendOnlyList_Remove_ThrowsException()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        Assert.Throws<NotSupportedException>(() => wrapper.Remove(42));
    }

    [TestMethod]
    public void FakeAppendOnlyList_IndexOf_ThrowsException()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        Assert.Throws<NotSupportedException>(() => wrapper.IndexOf(42));
    }

    [TestMethod]
    public void FakeAppendOnlyList_Indexer_Get_ThrowsException()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        Assert.Throws<NotSupportedException>(() => _ = wrapper[0]);
    }

    [TestMethod]
    public void FakeAppendOnlyList_Indexer_Set_ThrowsException()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        Assert.Throws<NotSupportedException>(() => wrapper[0] = 42);
    }

    [TestMethod]
    public void FakeAppendOnlyList_Clear_ThrowsException()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        Assert.Throws<NotSupportedException>(() => wrapper.Clear());
    }

    [TestMethod]
    public void FakeAppendOnlyList_Insert_ThrowsException()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        Assert.Throws<NotSupportedException>(() => wrapper.Insert(0, 42));
    }

    [TestMethod]
    public void FakeAppendOnlyList_RemoveAt_ThrowsException()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        Assert.Throws<NotSupportedException>(() => wrapper.RemoveAt(0));
    }

    [TestMethod]
    public void FakeAppendOnlyList_Count_ReturnsCorrectValue()
    {
        var innerList = new List<string>();
        var wrapper = Collections.FakeAppendOnlyList<int>.WrapWithConversionTo(innerList, i => i.ToString());

        wrapper.Add(1);
        wrapper.Add(2);
        wrapper.Add(3);

        Assert.AreEqual(3, wrapper.Count);
    }
}
