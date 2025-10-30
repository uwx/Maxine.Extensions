namespace Maxine.Extensions.Test;

[TestClass]
public class PooledArrayBufferWriterTests
{
    [TestMethod]
    public void Constructor_WithCapacity_CreatesWriter()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        Assert.IsNotNull(writer);
        Assert.AreEqual(0, writer.WrittenCount);
    }

    [TestMethod]
    public void Constructor_Default_CreatesWriter()
    {
        using var writer = new PooledArrayBufferWriter<byte>();
        Assert.IsNotNull(writer);
        Assert.AreEqual(0, writer.WrittenCount);
    }

    [TestMethod]
    public void GetSpan_ReturnsSpan()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        var span = writer.GetSpan(5);
        Assert.IsGreaterThanOrEqualTo(5, span.Length);
    }

    [TestMethod]
    public void Advance_IncreasesWrittenCount()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        var span = writer.GetSpan(5);
        writer.Advance(5);
        Assert.AreEqual(5, writer.WrittenCount);
    }

    [TestMethod]
    public void Clear_ResetsWrittenCount()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        writer.GetSpan(5);
        writer.Advance(5);
        Assert.AreEqual(5, writer.WrittenCount);
        
        writer.Clear();
        Assert.AreEqual(0, writer.WrittenCount);
    }

    [TestMethod]
    public void WrittenMemory_ReturnsCorrectData()
    {
        using var writer = new PooledArrayBufferWriter<byte>(16);
        var span = writer.GetSpan(3);
        span[0] = 1;
        span[1] = 2;
        span[2] = 3;
        writer.Advance(3);
        
        var memory = writer.WrittenMemory;
        Assert.AreEqual(3, memory.Length);
        Assert.AreEqual(1, memory.Span[0]);
        Assert.AreEqual(2, memory.Span[1]);
        Assert.AreEqual(3, memory.Span[2]);
    }
}

