namespace Maxine.Extensions.Test;

[TestClass]
public class PooledArrayBufferWriterAdditionalTests
{
    [TestMethod]
    public void GetMemory_ReturnsMemory()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        var memory = writer.GetMemory(5);
        
        Assert.IsGreaterThanOrEqualTo(5, memory.Length);
    }

    [TestMethod]
    public void ResetWrittenCount_ResetsWithoutClearing()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        var span = writer.GetSpan(5);
        span[0] = 123;
        writer.Advance(5);
        
        writer.ResetWrittenCount();
        
        Assert.AreEqual(0, writer.WrittenCount);
        // Data is not cleared, just position reset
    }

    [TestMethod]
    public void WrittenSpan_ReturnsCorrectData()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        var span = writer.GetSpan(3);
        span[0] = 1;
        span[1] = 2;
        span[2] = 3;
        writer.Advance(3);
        
        var writtenSpan = writer.WrittenSpan;
        Assert.AreEqual(3, writtenSpan.Length);
        Assert.AreEqual(1, writtenSpan[0]);
        Assert.AreEqual(2, writtenSpan[1]);
        Assert.AreEqual(3, writtenSpan[2]);
    }

    [TestMethod]
    public void Capacity_ReturnsCorrectValue()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        Assert.IsGreaterThanOrEqualTo(16, writer.Capacity);
    }

    [TestMethod]
    public void FreeCapacity_DecreasesAfterAdvance()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        var initialFree = writer.FreeCapacity;
        
        writer.GetSpan(5);
        writer.Advance(5);
        
        Assert.AreEqual(initialFree - 5, writer.FreeCapacity);
    }

    [TestMethod]
    public void Constructor_NegativeCapacity_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new PooledArrayBufferWriter<byte>(-1));
    }

    [TestMethod]
    public void Constructor_ZeroCapacity_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new PooledArrayBufferWriter<byte>(0));
    }

    [TestMethod]
    public void Advance_NegativeCount_ThrowsException()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        Assert.Throws<ArgumentException>(() => writer.Advance(-1));
    }

    [TestMethod]
    public void Advance_TooMuch_ThrowsException()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        writer.GetSpan(5);
        Assert.Throws<InvalidOperationException>(() => writer.Advance(100)); // More than capacity
    }

    [TestMethod]
    public void GetSpan_NegativeSize_ThrowsException()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        Assert.Throws<ArgumentException>(() => writer.GetSpan(-1));
    }

    [TestMethod]
    public void GetMemory_NegativeSize_ThrowsException()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        Assert.Throws<ArgumentException>(() => writer.GetMemory(-1));
    }

    [TestMethod]
    public void GetSpan_ZeroSize_ReturnsNonEmptySpan()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        var span = writer.GetSpan(0);
        
        Assert.IsGreaterThan(0, span.Length);
    }

    [TestMethod]
    public void GetMemory_ZeroSize_ReturnsNonEmptyMemory()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        var memory = writer.GetMemory(0);
        
        Assert.IsGreaterThan(0, memory.Length);
    }

    [TestMethod]
    public void AutoGrow_WhenExceedingInitialCapacity()
    {
        using var writer = new PooledArrayBufferWriter<byte>(4);
        
        writer.GetSpan(10); // Requires more than initial capacity
        writer.Advance(10);
        
        Assert.AreEqual(10, writer.WrittenCount);
        Assert.IsGreaterThanOrEqualTo(10, writer.Capacity);
    }

    [TestMethod]
    public void MultipleWrites_WorkCorrectly()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        var span1 = writer.GetSpan(3);
        span1[0] = 1;
        span1[1] = 2;
        span1[2] = 3;
        writer.Advance(3);
        
        var span2 = writer.GetSpan(2);
        span2[0] = 4;
        span2[1] = 5;
        writer.Advance(2);
        
        var written = writer.WrittenMemory;
        Assert.AreEqual(5, written.Length);
        Assert.AreEqual(1, written.Span[0]);
        Assert.AreEqual(2, written.Span[1]);
        Assert.AreEqual(3, written.Span[2]);
        Assert.AreEqual(4, written.Span[3]);
        Assert.AreEqual(5, written.Span[4]);
    }

    [TestMethod]
    public void Clear_AllowsReuse()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        writer.GetSpan(5);
        writer.Advance(5);
        Assert.AreEqual(5, writer.WrittenCount);
        
        writer.Clear();
        Assert.AreEqual(0, writer.WrittenCount);
        
        writer.GetSpan(3);
        writer.Advance(3);
        Assert.AreEqual(3, writer.WrittenCount);
    }

    [TestMethod]
    public void ResetWrittenCount_AllowsReuse()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        
        writer.GetSpan(5);
        writer.Advance(5);
        Assert.AreEqual(5, writer.WrittenCount);
        
        writer.ResetWrittenCount();
        Assert.AreEqual(0, writer.WrittenCount);
        
        writer.GetSpan(3);
        writer.Advance(3);
        Assert.AreEqual(3, writer.WrittenCount);
    }

    [TestMethod]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var writer = new PooledArrayBufferWriter<byte>(16);
        
        writer.Dispose();
        writer.Dispose(); // Should not throw
    }
}
