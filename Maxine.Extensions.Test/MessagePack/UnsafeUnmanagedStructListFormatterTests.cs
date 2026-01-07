using System.Buffers;
using System.Runtime.InteropServices;
using MessagePack;
using MessagePack.Resolvers;
using Maxine.Extensions.MessagePack;

namespace Maxine.Extensions.Test.MessagePack;

[TestClass]
public class UnsafeUnmanagedStructListFormatterTests
{
    private readonly MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard.WithResolver(StandardResolver.Instance);

    [TestMethod]
    public void Serialize_NullList_WritesNil()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<int>(-1);
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        
        formatter.Serialize(ref writer, null, _options);
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        Assert.IsTrue(reader.TryReadNil());
    }

    [TestMethod]
    public void Serialize_EmptyList_WritesEmptyExtension()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<int>(-1);
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        var emptyList = new List<int>();
        
        formatter.Serialize(ref writer, emptyList, _options);
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        var header = reader.ReadExtensionFormatHeader();
        Assert.AreEqual(-1, header.TypeCode);
        Assert.AreEqual(0U, header.Length);
    }

    [TestMethod]
    public void Serialize_IntList_WritesCorrectData()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<int>(-1);
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        var list = new List<int> { 1, 2, 3, 4, 5 };
        
        formatter.Serialize(ref writer, list, _options);
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        var header = reader.ReadExtensionFormatHeader();
        Assert.AreEqual(-1, header.TypeCode);
        Assert.AreEqual(sizeof(int) * 5U, header.Length);
    }

    [TestMethod]
    public void Deserialize_Nil_ReturnsNull()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<int>(-1);
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteNil();
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);
        
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Deserialize_EmptyExtension_ReturnsEmptyList()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<int>(-1);
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteExtensionFormatHeader(new ExtensionHeader(-1, 0));
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);
        
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(new List<int>(), result);
    }

    [TestMethod]
    public void RoundTrip_IntList_PreservesData()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<int>(-1);
        var original = new List<int> { 1, 2, 3, 4, 5 };
        
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);
        
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(original, result);
    }

    [TestMethod]
    public void RoundTrip_LongList_PreservesData()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<long>(-2);
        var original = new List<long> { 100L, 200L, 300L, 400L };
        
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);
        
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(original, result);
    }

    [TestMethod]
    public void RoundTrip_FloatList_PreservesData()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<float>(-3);
        var original = new List<float> { 1.5f, 2.5f, 3.5f };
        
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);
        
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(original, result);
    }

    [TestMethod]
    public void RoundTrip_DoubleList_PreservesData()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<double>(-4);
        var original = new List<double> { 1.234, 5.678, 9.012 };
        
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);
        
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(original, result);
    }

    [TestMethod]
    public void RoundTrip_ByteList_PreservesData()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<byte>(-5);
        var original = new List<byte> { 1, 2, 3, 255, 0 };
        
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);
        
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(original, result);
    }

    [TestMethod]
    public void RoundTrip_LargeList_PreservesData()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<int>(-1);
        var original = new List<int>();
        for (int i = 0; i < 10000; i++)
        {
            original.Add(i);
        }
        
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);
        
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(original, result);
    }

    [TestMethod]
    public void Deserialize_InvalidTypeCode_ThrowsException()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<int>(-1);
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteExtensionFormatHeader(new ExtensionHeader(-2, sizeof(int) * 5)); // Wrong type code
        writer.WriteRaw(new byte[sizeof(int) * 5]);
        writer.Flush();
        
        Assert.Throws<MessagePackSerializationException>(() =>
        {
            var reader = new MessagePackReader(buffer.WrittenMemory);
            return formatter.Deserialize(ref reader, _options);
        });
    }

    [TestMethod]
    public void Deserialize_InvalidLength_ThrowsException()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<int>(-1);
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        // Invalid length - not a multiple of sizeof(int)
        writer.WriteExtensionFormatHeader(new ExtensionHeader(-1, sizeof(int) * 5 + 1));
        writer.WriteRaw(new byte[sizeof(int) * 5 + 1]);
        writer.Flush();
        
        Assert.Throws<MessagePackSerializationException>(() =>
        {
            var reader = new MessagePackReader(buffer.WrittenMemory);
            return formatter.Deserialize(ref reader, _options);
        });
    }

    [TestMethod]
    public void TypeCode_ReturnsCorrectValue()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<int>(-42);
        Assert.AreEqual(-42, formatter.TypeCode);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct CustomStruct
    {
        public int X;
        public float Y;
        public long Z;
    }

    [TestMethod]
    public void RoundTrip_CustomStruct_PreservesData()
    {
        var formatter = new UnsafeUnmanagedStructListFormatter<CustomStruct>(-6);
        var original = new List<CustomStruct>
        {
            new CustomStruct { X = 1, Y = 1.5f, Z = 100L },
            new CustomStruct { X = 2, Y = 2.5f, Z = 200L },
            new CustomStruct { X = 3, Y = 3.5f, Z = 300L }
        };
        
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();
        
        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);
        
        Assert.IsNotNull(result);
        Assert.HasCount(original.Count, result);
        for (int i = 0; i < original.Count; i++)
        {
            Assert.AreEqual(original[i].X, result[i].X);
            Assert.AreEqual(original[i].Y, result[i].Y);
            Assert.AreEqual(original[i].Z, result[i].Z);
        }
    }
}

