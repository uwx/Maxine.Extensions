namespace Maxine.Extensions.Shared.Test;

[TestClass]
public class CircularBufferTests
{
    [TestMethod]
    public void CircularBuffer_GetEnumeratorConstructorCapacity_ReturnsEmptyCollection()
    {
        var buffer = new ObservableCircularBuffer<string>(5);
        CollectionAssert.AreEqual(buffer.ToArray(), Array.Empty<string>());
    }

    [TestMethod]
    public void CircularBuffer_ConstructorSizeIndexAccess_CorrectContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3]);

        Assert.AreEqual(5, buffer.Capacity);
        Assert.AreEqual(4, buffer.Count);
        for (int i = 0; i < 4; i++)
        {
            Assert.AreEqual(buffer[i], i);
        }
    }

    [TestMethod]
    public void CircularBuffer_Constructor_ExceptionWhenSourceIsLargerThanCapacity()
    {
        Assert.Throws<ArgumentException>(() => new ObservableCircularBuffer<int>(3, [0, 1, 2, 3]));
    }

    [TestMethod]
    public void CircularBuffer_GetEnumeratorConstructorDefinedArray_CorrectContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3]);

        int x = 0;
        foreach (var item in buffer)
        {
            Assert.AreEqual(item, x);
            x++;
        }
    }

    [TestMethod]
    public void CircularBuffer_PushBack_CorrectContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5);

        for (int i = 0; i < 5; i++)
        {
            buffer.PushBack(i);
        }

        Assert.AreEqual(0, buffer.Front());
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(buffer[i], i);
        }
    }

    [TestMethod]
    public void CircularBuffer_PushBackOverflowingBuffer_CorrectContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5);

        for (int i = 0; i < 10; i++)
        {
            buffer.PushBack(i);
        }

        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 5, 6, 7, 8, 9 });
    }

    [TestMethod]
    public void CircularBuffer_GetEnumeratorOverflowedArray_CorrectContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5);

        for (int i = 0; i < 10; i++)
        {
            buffer.PushBack(i);
        }

        // buffer should have [5,6,7,8,9]
        int x = 5;
        foreach (var item in buffer)
        {
            Assert.AreEqual(item, x);
            x++;
        }
    }

    [TestMethod]
    public void CircularBuffer_ToArrayConstructorDefinedArray_CorrectContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3]);

        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 0, 1, 2, 3 });
    }

    [TestMethod]
    public void CircularBuffer_ToArrayOverflowedBuffer_CorrectContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5);

        for (int i = 0; i < 10; i++)
        {
            buffer.PushBack(i);
        }

        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 5, 6, 7, 8, 9 });
    }

    [TestMethod]
    public void CircularBuffer_ToArraySegmentsConstructorDefinedArray_CorrectContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3]);

        var arraySegments = buffer.ToArraySegments();

        Assert.HasCount(2, arraySegments); // length of 2 is part of the contract.
        CollectionAssert.AreEqual(arraySegments.SelectMany(x => x).ToArray(), new[] { 0, 1, 2, 3 });
    }

    [TestMethod]
    public void CircularBuffer_ToArraySegmentsOverflowedBuffer_CorrectContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5);

        for (int i = 0; i < 10; i++)
        {
            buffer.PushBack(i);
        }

        var arraySegments = buffer.ToArraySegments();
        Assert.HasCount(2, arraySegments); // length of 2 is part of the contract.
        CollectionAssert.AreEqual(arraySegments.SelectMany(x => x).ToArray(), new[] { 5, 6, 7, 8, 9 });
    }

    [TestMethod]
    public void CircularBuffer_PushFront_CorrectContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5);

        for (int i = 0; i < 5; i++)
        {
            buffer.PushFront(i);
        }

        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 4, 3, 2, 1, 0 });
    }

    [TestMethod]
    public void CircularBuffer_PushFrontAndOverflow_CorrectContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5);

        for (int i = 0; i < 10; i++)
        {
            buffer.PushFront(i);
        }

        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 9, 8, 7, 6, 5 });
    }

    [TestMethod]
    public void CircularBuffer_Front_CorrectItem()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3, 4]);

        Assert.AreEqual(0, buffer.Front());
    }

    [TestMethod]
    public void CircularBuffer_Back_CorrectItem()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3, 4]);
        Assert.AreEqual(4, buffer.Back());
    }

    [TestMethod]
    public void CircularBuffer_BackOfBufferOverflowByOne_CorrectItem()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3, 4]);
        buffer.PushBack(42);
        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 1, 2, 3, 4, 42 });
        Assert.AreEqual(42, buffer.Back());
    }

    [TestMethod]
    public void CircularBuffer_Front_EmptyBufferThrowsException()
    {
        var buffer = new ObservableCircularBuffer<int>(5);

        StringAssert.Contains(Assert.Throws<InvalidOperationException>(() => buffer.Front()).Message, "empty buffer");
    }

    [TestMethod]
    public void CircularBuffer_Back_EmptyBufferThrowsException()
    {
        var buffer = new ObservableCircularBuffer<int>(5);
        StringAssert.Contains(Assert.Throws<InvalidOperationException>(() => buffer.Back()).Message, "empty buffer");
    }

    [TestMethod]
    public void CircularBuffer_PopBack_RemovesBackElement()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3, 4]);

        Assert.AreEqual(5, buffer.Count);

        buffer.PopBack();

        Assert.AreEqual(4, buffer.Count);
        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 0, 1, 2, 3 });
    }

    [TestMethod]
    public void CircularBuffer_PopBackInOverflowBuffer_RemovesBackElement()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3, 4]);
        buffer.PushBack(5);

        Assert.AreEqual(5, buffer.Count);
        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 1, 2, 3, 4, 5 });

        buffer.PopBack();

        Assert.AreEqual(4, buffer.Count);
        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 1, 2, 3, 4 });
    }

    [TestMethod]
    public void CircularBuffer_PopFront_RemovesBackElement()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3, 4]);

        Assert.AreEqual(5, buffer.Count);

        buffer.PopFront();

        Assert.AreEqual(4, buffer.Count);
        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 1, 2, 3, 4 });
    }

    [TestMethod]
    public void CircularBuffer_PopFrontInOverflowBuffer_RemovesBackElement()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3, 4]);
        buffer.PushFront(5);

        Assert.AreEqual(5, buffer.Count);
        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 5, 0, 1, 2, 3 });

        buffer.PopFront();

        Assert.AreEqual(4, buffer.Count);
        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 0, 1, 2, 3 });
    }

    [TestMethod]
    public void CircularBuffer_SetIndex_ReplacesElement()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3, 4])
        {
            [1] = 10,
            [3] = 30
        };

        CollectionAssert.AreEqual(buffer.ToArray(), new[] { 0, 10, 2, 30, 4 });
    }

    [TestMethod]
    public void CircularBuffer_WithDifferentSizeAndCapacity_BackReturnsLastArrayPosition()
    {
        // test to confirm this issue does not happen anymore:
        // https://github.com/joaoportela/CircularBuffer-CSharp/issues/2

        var buffer = new ObservableCircularBuffer<int>(5, [0, 1, 2, 3, 4]);

        buffer.PopFront(); // (make size and capacity different)

        Assert.AreEqual(4, buffer.Back());
    }

    [TestMethod]
    public void CircularBuffer_Clear_ClearsContent()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [4, 3, 2, 1, 0]);

        buffer.Clear();

        Assert.AreEqual(0, buffer.Count);
        Assert.AreEqual(5, buffer.Capacity);
        CollectionAssert.AreEqual(buffer.ToArray(), Array.Empty<int>());
    }

    [TestMethod]
    public void CircularBuffer_Clear_WorksNormallyAfterClear()
    {
        var buffer = new ObservableCircularBuffer<int>(5, [4, 3, 2, 1, 0]);

        buffer.Clear();
        for (int i = 0; i < 5; i++)
        {
            buffer.PushBack(i);
        }

        Assert.AreEqual(0, buffer.Front());
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(buffer[i], i);
        }
    }
}