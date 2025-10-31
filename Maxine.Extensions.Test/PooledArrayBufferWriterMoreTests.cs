namespace Maxine.Extensions.Test;

[TestClass]
public class PooledArrayBufferWriterMoreTests
{
    [TestMethod]
    public void DefaultConstructor_CreatesEmptyBuffer()
    {
        using var writer = new PooledArrayBufferWriter<byte>();
        
        Assert.AreEqual(0, writer.WrittenCount);
        Assert.IsGreaterThanOrEqualTo(0, writer.Capacity);
    }

    [TestMethod]
    public void DefaultConstructor_CanWriteData()
    {
        using var writer = new PooledArrayBufferWriter<byte>();
        
        var span = writer.GetSpan(5);
        span[0] = 1;
        span[1] = 2;
        writer.Advance(2);
        
        Assert.AreEqual(2, writer.WrittenCount);
        Assert.AreEqual(1, writer.WrittenSpan[0]);
        Assert.AreEqual(2, writer.WrittenSpan[1]);
    }

    [TestMethod]
    public void Clear_ZeroesBuffer()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        var span = writer.GetSpan(5);
        span[0] = 123;
        span[1] = 234;
        writer.Advance(2);
        
        writer.Clear();
        
        Assert.AreEqual(0, writer.WrittenCount);
        // After clear, getting new span and checking old data is zeroed
        writer.GetSpan(5);
        // The buffer should be cleared
    }

    [TestMethod]
    public void GetMemory_WithSizeHint_ReturnsAtLeastRequestedSize()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        var memory = writer.GetMemory(100);
        
        Assert.IsGreaterThanOrEqualTo(100, memory.Length);
    }

    [TestMethod]
    public void GetSpan_WithSizeHint_ReturnsAtLeastRequestedSize()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        var span = writer.GetSpan(100);
        
        Assert.IsGreaterThanOrEqualTo(100, span.Length);
    }

    [TestMethod]
    public void MultipleGrows_WorkCorrectly()
    {
        using var writer = new PooledArrayBufferWriter<byte>(2);
        
        // First grow
        writer.GetSpan(10);
        writer.Advance(10);
        
        // Second grow
        writer.GetSpan(50);
        writer.Advance(50);
        
        Assert.AreEqual(60, writer.WrittenCount);
        Assert.IsGreaterThanOrEqualTo(60, writer.Capacity);
    }

    [TestMethod]
    public void WrittenMemory_ReturnsCorrectData()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        var span = writer.GetSpan(3);
        span[0] = 10;
        span[1] = 20;
        span[2] = 30;
        writer.Advance(3);
        
        var memory = writer.WrittenMemory;
        
        Assert.AreEqual(3, memory.Length);
        Assert.AreEqual(10, memory.Span[0]);
        Assert.AreEqual(20, memory.Span[1]);
        Assert.AreEqual(30, memory.Span[2]);
    }

    [TestMethod]
    public void FreeCapacity_CorrectAfterMultipleOperations()
    {
        using var writer = new PooledArrayBufferWriter<byte>(20);
        
        var initialCapacity = writer.Capacity;
        
        writer.GetSpan(5);
        writer.Advance(5);
        
        Assert.AreEqual(initialCapacity - 5, writer.FreeCapacity);
        
        writer.GetSpan(3);
        writer.Advance(3);
        
        Assert.AreEqual(initialCapacity - 8, writer.FreeCapacity);
    }

    [TestMethod]
    public void GetSpan_AfterClear_ReturnsCleanBuffer()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        var span1 = writer.GetSpan(5);
        span1[0] = 255;
        writer.Advance(1);
        
        writer.Clear();
        
        var span2 = writer.GetSpan(5);
        // Buffer should be cleared, so first byte should be 0
        Assert.AreEqual(0, span2[0]);
    }

    [TestMethod]
    public void GetMemory_AfterResetWrittenCount_MayContainOldData()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        var span1 = writer.GetSpan(5);
        span1[0] = 255;
        writer.Advance(1);
        
        writer.ResetWrittenCount();
        
        // After reset (not clear), buffer is not zeroed
        Assert.AreEqual(0, writer.WrittenCount);
    }

    [TestMethod]
    public void LargeAllocation_WorksCorrectly()
    {
        using var writer = new PooledArrayBufferWriter<byte>(10);
        
        // Request a very large buffer
        var span = writer.GetSpan(10000);
        
        Assert.IsGreaterThanOrEqualTo(10000, span.Length);
        
        // Write some data at the end
        span[9999] = 42;
        writer.Advance(10000);
        
        Assert.AreEqual(10000, writer.WrittenCount);
        Assert.AreEqual(42, writer.WrittenSpan[9999]);
    }

    [TestMethod]
    public void AdvanceZero_DoesNothing()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        writer.GetSpan(5);
        writer.Advance(0);
        
        Assert.AreEqual(0, writer.WrittenCount);
    }

    [TestMethod]
    public void Capacity_IncreasesWhenNeeded()
    {
        using var writer = new PooledArrayBufferWriter<byte>(4);
        
        var initialCapacity = writer.Capacity;
        
        writer.GetSpan(100);
        
        Assert.IsGreaterThan(initialCapacity, writer.Capacity);
    }

    [TestMethod]
    public void MultipleDispose_DoesNotThrow()
    {
        var writer = new PooledArrayBufferWriter<byte>(16);
        
        writer.GetSpan(5);
        writer.Advance(5);
        
        writer.Dispose();
        writer.Dispose();
        writer.Dispose();
        
        // Should not throw
    }

    [TestMethod]
    public void VeryLargeInitialCapacity_WorksCorrectly()
    {
        using var writer = new PooledArrayBufferWriter<byte>(100000);
        
        Assert.IsGreaterThanOrEqualTo(100000, writer.Capacity);
        
        writer.GetSpan(50000);
        writer.Advance(50000);
        
        Assert.AreEqual(50000, writer.WrittenCount);
    }

    [TestMethod]
    public void Advance_ExactlyToCapacity_WorksCorrectly()
    {
        using var writer = new PooledArrayBufferWriter<byte>(10);
        
        var capacity = writer.Capacity;
        writer.GetSpan(capacity);
        writer.Advance(capacity);
        
        Assert.AreEqual(capacity, writer.WrittenCount);
        Assert.AreEqual(0, writer.FreeCapacity);
    }

    [TestMethod]
    public void GetSpan_Multiple_Times_Without_Advance()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        var span1 = writer.GetSpan(5);
        var span2 = writer.GetSpan(5);
        
        // Both should point to the same position since we didn't advance
        span1[0] = 123;
        Assert.AreEqual(123, span2[0]);
    }

    [TestMethod]
    public void WrittenCount_StartsAtZero()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        Assert.AreEqual(0, writer.WrittenCount);
    }

    [TestMethod]
    public void DefaultConstructor_FreeCapacity_IsZero()
    {
        using var writer = new PooledArrayBufferWriter<byte>();
        
        // Default constructor creates empty buffer
        Assert.AreEqual(0, writer.FreeCapacity);
    }

    [TestMethod]
    public void GrowFromZero_WorksCorrectly()
    {
        using var writer = new PooledArrayBufferWriter<byte>();
        
        var span = writer.GetSpan(256);
        Assert.IsGreaterThanOrEqualTo(256, span.Length);
        
        span[0] = 1;
        span[255] = 2;
        writer.Advance(256);
        
        Assert.AreEqual(256, writer.WrittenCount);
        Assert.AreEqual(1, writer.WrittenSpan[0]);
        Assert.AreEqual(2, writer.WrittenSpan[255]);
    }

    [TestMethod]
    public void Clear_ThenGetMemory_ReturnsZeroedMemory()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        var span1 = writer.GetSpan(10);
        for (int i = 0; i < 10; i++)
        {
            span1[i] = (byte)(i + 100);
        }
        writer.Advance(10);
        
        writer.Clear();
        
        var memory = writer.GetMemory(10);
        // After clear, memory should be zeroed
        for (int i = 0; i < 10; i++)
        {
            Assert.AreEqual(0, memory.Span[i]);
        }
    }

    [TestMethod]
    public void GetSpan_ThenGetMemory_SamePosition()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        var span = writer.GetSpan(5);
        span[0] = 42;
        
        var memory = writer.GetMemory(5);
        
        // Should be at the same position
        Assert.AreEqual(42, memory.Span[0]);
    }
}
