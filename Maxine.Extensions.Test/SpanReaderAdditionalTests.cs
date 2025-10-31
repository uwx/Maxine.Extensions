namespace Maxine.Extensions.Test;

[TestClass]
public class SpanReaderAdditionalTests
{
    [TestMethod]
    public void Constructor_CreatesReader()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3, 4, 5 };
        var reader = new Streams.SpanReader(data);
        
        Assert.AreEqual(0, reader.Position);
        Assert.AreEqual(5, reader.Length);
        Assert.AreEqual(5, reader.Remaining);
    }

    [TestMethod]
    public void ReadByte_ReadsCorrectly()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 42, 100, 255 };
        var reader = new Streams.SpanReader(data);
        
        Assert.AreEqual((byte)42, reader.ReadByte());
        Assert.AreEqual(1, reader.Position);
        Assert.AreEqual((byte)100, reader.ReadByte());
        Assert.AreEqual((byte)255, reader.ReadByte());
        Assert.AreEqual(3, reader.Position);
    }

    [TestMethod]
    public void TryReadByte_Success_ReturnsTrue()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 42 };
        var reader = new Streams.SpanReader(data);
        
        var success = reader.TryReadByte(out var result);
        
        Assert.IsTrue(success);
        Assert.AreEqual((byte)42, result);
    }

    [TestMethod]
    public void TryReadByte_EndOfData_ReturnsFalse()
    {
        ReadOnlySpan<byte> data = ReadOnlySpan<byte>.Empty;
        var reader = new Streams.SpanReader(data);
        
        var success = reader.TryReadByte(out var result);
        
        Assert.IsFalse(success);
        Assert.AreEqual((byte)0, result);
    }

    [TestMethod]
    public void ReadBoolean_ReadsCorrectly()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 0, 1, 255 };
        var reader = new Streams.SpanReader(data);
        
        Assert.IsFalse(reader.ReadBoolean());
        Assert.IsTrue(reader.ReadBoolean());
        Assert.IsTrue(reader.ReadBoolean());
    }

    [TestMethod]
    public void ReadInt16_ReadsCorrectly()
    {
        Span<byte> buffer = stackalloc byte[2];
        BitConverter.TryWriteBytes(buffer, (short)12345);
        var reader = new Streams.SpanReader(buffer);
        
        Assert.AreEqual((short)12345, reader.ReadInt16());
    }

    [TestMethod]
    public void ReadInt32_ReadsCorrectly()
    {
        Span<byte> buffer = stackalloc byte[4];
        BitConverter.TryWriteBytes(buffer, 123456789);
        var reader = new Streams.SpanReader(buffer);
        
        Assert.AreEqual(123456789, reader.ReadInt32());
    }

    [TestMethod]
    public void ReadInt64_ReadsCorrectly()
    {
        Span<byte> buffer = stackalloc byte[8];
        BitConverter.TryWriteBytes(buffer, 123456789012345L);
        var reader = new Streams.SpanReader(buffer);
        
        Assert.AreEqual(123456789012345L, reader.ReadInt64());
    }

    [TestMethod]
    public void ReadUInt16_ReadsCorrectly()
    {
        Span<byte> buffer = stackalloc byte[2];
        BitConverter.TryWriteBytes(buffer, (ushort)54321);
        var reader = new Streams.SpanReader(buffer);
        
        Assert.AreEqual((ushort)54321, reader.ReadUInt16());
    }

    [TestMethod]
    public void ReadUInt32_ReadsCorrectly()
    {
        Span<byte> buffer = stackalloc byte[4];
        BitConverter.TryWriteBytes(buffer, 987654321u);
        var reader = new Streams.SpanReader(buffer);
        
        Assert.AreEqual(987654321u, reader.ReadUInt32());
    }

    [TestMethod]
    public void ReadUInt64_ReadsCorrectly()
    {
        Span<byte> buffer = stackalloc byte[8];
        BitConverter.TryWriteBytes(buffer, 987654321098765ul);
        var reader = new Streams.SpanReader(buffer);
        
        Assert.AreEqual(987654321098765ul, reader.ReadUInt64());
    }

    [TestMethod]
    public void ReadSByte_ReadsCorrectly()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 127, 255 };
        var reader = new Streams.SpanReader(data);
        
        Assert.AreEqual((sbyte)127, reader.ReadSByte());
        Assert.AreEqual((sbyte)-1, reader.ReadSByte());
    }

    [TestMethod]
    public void ReadSingle_ReadsCorrectly()
    {
        Span<byte> buffer = stackalloc byte[4];
        BitConverter.TryWriteBytes(buffer, 3.14159f);
        var reader = new Streams.SpanReader(buffer);
        
        Assert.AreEqual(3.14159f, reader.ReadSingle(), 0.00001f);
    }

    [TestMethod]
    public void ReadDouble_ReadsCorrectly()
    {
        Span<byte> buffer = stackalloc byte[8];
        BitConverter.TryWriteBytes(buffer, 3.141592653589793);
        var reader = new Streams.SpanReader(buffer);
        
        Assert.AreEqual(3.141592653589793, reader.ReadDouble(), 0.000000000001);
    }

    [TestMethod]
    public void ReadDecimal_ReadsCorrectly()
    {
        Span<byte> buffer = stackalloc byte[16];
        var value = 123.456m;
        decimal.GetBits(value).AsSpan().CopyTo(System.Runtime.InteropServices.MemoryMarshal.Cast<byte, int>(buffer));
        var reader = new Streams.SpanReader(buffer);
        
        Assert.AreEqual(value, reader.ReadDecimal());
    }

    [TestMethod]
    public void ReadBytes_ReadsCorrectly()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3, 4, 5 };
        var reader = new Streams.SpanReader(data);
        
        var bytes = reader.ReadBytes(3);
        
        Assert.AreEqual(3, bytes.Length);
        Assert.AreEqual((byte)1, bytes[0]);
        Assert.AreEqual((byte)2, bytes[1]);
        Assert.AreEqual((byte)3, bytes[2]);
        Assert.AreEqual(3, reader.Position);
    }

    [TestMethod]
    public void ReadSpan_ReadsCorrectly()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3, 4, 5 };
        var reader = new Streams.SpanReader(data);
        
        var span = reader.ReadSpan(3);
        
        Assert.AreEqual(3, span.Length);
        Assert.AreEqual((byte)1, span[0]);
        Assert.AreEqual((byte)2, span[1]);
        Assert.AreEqual((byte)3, span[2]);
    }

    [TestMethod]
    public void Position_SetGet_WorksCorrectly()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3, 4, 5 };
        var reader = new Streams.SpanReader(data);
        
        reader.Position = 3;
        Assert.AreEqual(3, reader.Position);
        Assert.AreEqual(2, reader.Remaining);
    }

    [TestMethod]
    public void Position_SetNegative_ThrowsException()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3 };
        var reader = new Streams.SpanReader(data);
        
        try
        {
            reader.Position = -1;
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Position_SetBeyondLength_ThrowsException()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3 };
        var reader = new Streams.SpanReader(data);
        
        try
        {
            reader.Position = 10;
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Advance_MovesPosition()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3, 4, 5 };
        var reader = new Streams.SpanReader(data);
        
        reader.Advance(3);
        
        Assert.AreEqual(3, reader.Position);
        Assert.AreEqual(2, reader.Remaining);
    }

    [TestMethod]
    public void Advance_ToEnd_WorksCorrectly()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3 };
        var reader = new Streams.SpanReader(data);
        
        reader.Advance(3);
        
        Assert.AreEqual(3, reader.Position);
        Assert.AreEqual(0, reader.Remaining);
    }

    [TestMethod]
    public void GetRemainingBuffer_ReturnsCorrectSpan()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3, 4, 5 };
        var reader = new Streams.SpanReader(data);
        
        reader.ReadByte();
        reader.ReadByte();
        
        var remaining = reader.GetRemainingBuffer();
        
        Assert.AreEqual(3, remaining.Length);
        Assert.AreEqual((byte)3, remaining[0]);
        Assert.AreEqual((byte)4, remaining[1]);
        Assert.AreEqual((byte)5, remaining[2]);
    }

    [TestMethod]
    public void Read_IntoArray_WorksCorrectly()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3, 4, 5 };
        var reader = new Streams.SpanReader(data);
        
        var buffer = new byte[3];
        var count = reader.Read(buffer, 0, 3);
        
        Assert.AreEqual(3, count);
        Assert.AreEqual((byte)1, buffer[0]);
        Assert.AreEqual((byte)2, buffer[1]);
        Assert.AreEqual((byte)3, buffer[2]);
    }

    [TestMethod]
    public void Read_ZeroCount_ReturnsZero()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 2, 3 };
        var reader = new Streams.SpanReader(data);
        
        var buffer = new byte[10];
        var count = reader.Read(buffer, 0, 0);
        
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void TryRead_Methods_ReturnFalseAtEnd()
    {
        ReadOnlySpan<byte> data = stackalloc byte[] { 1 };
        var reader = new Streams.SpanReader(data);
        
        reader.ReadByte(); // Consume the data
        
        Assert.IsFalse(reader.TryReadByte(out _));
        Assert.IsFalse(reader.TryReadBoolean(out _));
        Assert.IsFalse(reader.TryReadInt16(out _));
        Assert.IsFalse(reader.TryReadInt32(out _));
        Assert.IsFalse(reader.TryReadInt64(out _));
        Assert.IsFalse(reader.TryReadUInt16(out _));
        Assert.IsFalse(reader.TryReadUInt32(out _));
        Assert.IsFalse(reader.TryReadUInt64(out _));
        Assert.IsFalse(reader.TryReadSByte(out _));
        Assert.IsFalse(reader.TryReadSingle(out _));
        Assert.IsFalse(reader.TryReadDouble(out _));
        Assert.IsFalse(reader.TryReadDecimal(out _));
    }

    [TestMethod]
    public void TryRead_Methods_ReturnTrueWithData()
    {
        Span<byte> buffer = stackalloc byte[100];
        BitConverter.TryWriteBytes(buffer[0..], (short)100);
        BitConverter.TryWriteBytes(buffer[2..], 200);
        BitConverter.TryWriteBytes(buffer[6..], 300L);
        BitConverter.TryWriteBytes(buffer[14..], 1.5f);
        BitConverter.TryWriteBytes(buffer[18..], 2.5);
        
        var reader = new Streams.SpanReader(buffer);
        
        Assert.IsTrue(reader.TryReadInt16(out var i16));
        Assert.AreEqual((short)100, i16);
        
        Assert.IsTrue(reader.TryReadInt32(out var i32));
        Assert.AreEqual(200, i32);
        
        Assert.IsTrue(reader.TryReadInt64(out var i64));
        Assert.AreEqual(300L, i64);
        
        Assert.IsTrue(reader.TryReadSingle(out var f32));
        Assert.AreEqual(1.5f, f32, 0.001f);
        
        Assert.IsTrue(reader.TryReadDouble(out var f64));
        Assert.AreEqual(2.5, f64, 0.001);
    }
}
