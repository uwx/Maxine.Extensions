using Maxine.Extensions.Streams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Buffers.Binary;

namespace Maxine.Extensions.Test;

[TestClass]
public class SpanReaderTests
{
    [TestMethod]
    public void ReadBoolean_TrueValue_ReturnsTrue()
    {
        Span<byte> buffer = [1];
        var reader = new SpanReader(buffer);
        
        var result = reader.ReadBoolean();
        
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ReadBoolean_FalseValue_ReturnsFalse()
    {
        Span<byte> buffer = [0];
        var reader = new SpanReader(buffer);
        
        var result = reader.ReadBoolean();
        
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ReadByte_ReturnsCorrectByte()
    {
        Span<byte> buffer = [42];
        var reader = new SpanReader(buffer);
        
        var result = reader.ReadByte();
        
        Assert.AreEqual((byte)42, result);
    }

    [TestMethod]
    public void ReadInt16_ReturnsCorrectValue()
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteInt16LittleEndian(buffer, 12345);
        var reader = new SpanReader(buffer);
        
        var result = reader.ReadInt16();
        
        Assert.AreEqual((short)12345, result);
    }

    [TestMethod]
    public void ReadInt32_ReturnsCorrectValue()
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(buffer, 123456789);
        var reader = new SpanReader(buffer);
        
        var result = reader.ReadInt32();
        
        Assert.AreEqual(123456789, result);
    }

    [TestMethod]
    public void ReadInt64_ReturnsCorrectValue()
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteInt64LittleEndian(buffer, 1234567890123456789L);
        var reader = new SpanReader(buffer);
        
        var result = reader.ReadInt64();
        
        Assert.AreEqual(1234567890123456789L, result);
    }

    [TestMethod]
    public void ReadSingle_ReturnsCorrectValue()
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteSingleLittleEndian(buffer, 3.14f);
        var reader = new SpanReader(buffer);
        
        var result = reader.ReadSingle();
        
        Assert.AreEqual(3.14f, result, 0.0001f);
    }

    [TestMethod]
    public void ReadDouble_ReturnsCorrectValue()
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteDoubleLittleEndian(buffer, 3.14159265);
        var reader = new SpanReader(buffer);
        
        var result = reader.ReadDouble();
        
        Assert.AreEqual(3.14159265, result, 0.000001);
    }

    [TestMethod]
    public void ReadDecimal_ReturnsCorrectValue()
    {
        Span<byte> buffer = stackalloc byte[16];
        var originalValue = 123.456m;
        var decimalBits = decimal.GetBits(originalValue);
        
        BinaryPrimitives.WriteInt32LittleEndian(buffer, decimalBits[0]);
        BinaryPrimitives.WriteInt32LittleEndian(buffer[4..], decimalBits[1]);
        BinaryPrimitives.WriteInt32LittleEndian(buffer[8..], decimalBits[2]);
        BinaryPrimitives.WriteInt32LittleEndian(buffer[12..], decimalBits[3]);
        
        var reader = new SpanReader(buffer);
        var result = reader.ReadDecimal();
        
        Assert.AreEqual(originalValue, result);
    }

    [TestMethod]
    public void ReadMultipleValues_ReadsSequentially()
    {
        Span<byte> buffer = stackalloc byte[9];
        buffer[0] = 1; // boolean
        BinaryPrimitives.WriteInt32LittleEndian(buffer[1..], 42);
        BinaryPrimitives.WriteInt32LittleEndian(buffer[5..], 100);
        
        var reader = new SpanReader(buffer);
        
        var bool1 = reader.ReadBoolean();
        var int1 = reader.ReadInt32();
        var int2 = reader.ReadInt32();
        
        Assert.IsTrue(bool1);
        Assert.AreEqual(42, int1);
        Assert.AreEqual(100, int2);
    }

    [TestMethod]
    public void Position_TracksReadPosition()
    {
        Span<byte> buffer = stackalloc byte[10];
        var reader = new SpanReader(buffer);
        
        Assert.AreEqual(0, reader.Position);
        
        reader.ReadByte();
        Assert.AreEqual(1, reader.Position);
        
        reader.ReadInt32();
        Assert.AreEqual(5, reader.Position);
    }
}

