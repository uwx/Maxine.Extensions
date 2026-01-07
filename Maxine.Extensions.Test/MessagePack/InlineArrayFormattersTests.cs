using System.Buffers;
using System.Runtime.CompilerServices;
using MessagePack;
using MessagePack.Resolvers;
using Maxine.Extensions.MessagePack;

namespace Maxine.Extensions.Test.MessagePack;

[TestClass]
public class InlineArrayFormattersTests
{
    private readonly MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard.WithResolver(StandardResolver.Instance);

    #region InlineArray2Formatter Tests

    [TestMethod]
    public void InlineArray2_Serialize_WritesCorrectArrayHeader()
    {
        var formatter = new InlineArray2Formatter<int>();
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        var array = new InlineArray2<int>();
        array[0] = 10;
        array[1] = 20;

        formatter.Serialize(ref writer, array, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var length = reader.ReadArrayHeader();
        Assert.AreEqual(2, length);
    }

    [TestMethod]
    public void InlineArray2_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray2Formatter<int>();
        var original = new InlineArray2<int>();
        original[0] = 42;
        original[1] = 84;

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        Assert.AreEqual(original[0], result[0]);
        Assert.AreEqual(original[1], result[1]);
    }

    [TestMethod]
    public void InlineArray2_Deserialize_Nil_ThrowsException()
    {
        var formatter = new InlineArray2Formatter<int>();
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteNil();
        writer.Flush();

        Assert.Throws<MessagePackSerializationException>(() =>
        {
            var reader = new MessagePackReader(buffer.WrittenMemory);
            return formatter.Deserialize(ref reader, _options);
        });
    }

    [TestMethod]
    public void InlineArray2_Deserialize_HandlesExtraElements()
    {
        var formatter = new InlineArray2Formatter<int>();
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteArrayHeader(4); // More elements than expected
        writer.Write(1);
        writer.Write(2);
        writer.Write(3);
        writer.Write(4);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        Assert.AreEqual(1, result[0]);
        Assert.AreEqual(2, result[1]);
    }

    [TestMethod]
    public void InlineArray2_Deserialize_HandlesMissingElements()
    {
        var formatter = new InlineArray2Formatter<int>();
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteArrayHeader(1); // Fewer elements than expected
        writer.Write(100);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        Assert.AreEqual(100, result[0]);
        Assert.AreEqual(0, result[1]); // Default value
    }

    #endregion

    #region InlineArray4Formatter Tests

    [TestMethod]
    public void InlineArray4_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray4Formatter<int>();
        var original = new InlineArray4<int>();
        original[0] = 1;
        original[1] = 2;
        original[2] = 3;
        original[3] = 4;

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        Assert.AreEqual(original[0], result[0]);
        Assert.AreEqual(original[1], result[1]);
        Assert.AreEqual(original[2], result[2]);
        Assert.AreEqual(original[3], result[3]);
    }

    [TestMethod]
    public void InlineArray4_RoundTrip_StringData()
    {
        var formatter = new InlineArray4Formatter<string>();
        var original = new InlineArray4<string>();
        original[0] = "Hello";
        original[1] = "World";
        original[2] = "Test";
        original[3] = "Data";

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        Assert.AreEqual(original[0], result[0]);
        Assert.AreEqual(original[1], result[1]);
        Assert.AreEqual(original[2], result[2]);
        Assert.AreEqual(original[3], result[3]);
    }

    #endregion

    #region InlineArray8Formatter Tests

    [TestMethod]
    public void InlineArray8_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray8Formatter<double>();
        var original = new InlineArray8<double>();
        for (int i = 0; i < 8; i++)
        {
            original[i] = i * 1.5;
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 8; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    [TestMethod]
    public void InlineArray8_Serialize_WritesCorrectArrayHeader()
    {
        var formatter = new InlineArray8Formatter<int>();
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        var array = new InlineArray8<int>();

        formatter.Serialize(ref writer, array, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var length = reader.ReadArrayHeader();
        Assert.AreEqual(8, length);
    }

    #endregion

    #region InlineArray16Formatter Tests

    [TestMethod]
    public void InlineArray16_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray16Formatter<byte>();
        var original = new InlineArray16<byte>();
        for (int i = 0; i < 16; i++)
        {
            original[i] = (byte)i;
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 16; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    [TestMethod]
    public void InlineArray16_Serialize_WritesCorrectArrayHeader()
    {
        var formatter = new InlineArray16Formatter<int>();
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        var array = new InlineArray16<int>();

        formatter.Serialize(ref writer, array, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var length = reader.ReadArrayHeader();
        Assert.AreEqual(16, length);
    }

    [TestMethod]
    public void InlineArray16_Deserialize_Nil_ThrowsException()
    {
        var formatter = new InlineArray16Formatter<int>();
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteNil();
        writer.Flush();

        Assert.Throws<MessagePackSerializationException>(() =>
        {
            var reader = new MessagePackReader(buffer.WrittenMemory);
            return formatter.Deserialize(ref reader, _options);
        });
    }

    #endregion

    #region TypeCode Tests

    [TestMethod]
    public void InlineArray2_TypeCode_DefaultValue()
    {
        var formatter = new InlineArray2Formatter<int>();
        Assert.AreEqual(-2, formatter.TypeCode);
    }

    [TestMethod]
    public void InlineArray2_TypeCode_CustomValue()
    {
        var formatter = new InlineArray2Formatter<int>(-100);
        Assert.AreEqual(-100, formatter.TypeCode);
    }

    [TestMethod]
    public void InlineArray4_TypeCode_DefaultValue()
    {
        var formatter = new InlineArray4Formatter<int>();
        Assert.AreEqual(-4, formatter.TypeCode);
    }

    [TestMethod]
    public void InlineArray8_TypeCode_DefaultValue()
    {
        var formatter = new InlineArray8Formatter<int>();
        Assert.AreEqual(-8, formatter.TypeCode);
    }

    [TestMethod]
    public void InlineArray16_TypeCode_DefaultValue()
    {
        var formatter = new InlineArray16Formatter<int>();
        Assert.AreEqual(-16, formatter.TypeCode);
    }

    #endregion

    #region Instance Tests

    [TestMethod]
    public void InlineArray2_Instance_IsSingleton()
    {
        var instance1 = InlineArray2Formatter<int>.Instance;
        var instance2 = InlineArray2Formatter<int>.Instance;
        Assert.AreSame(instance1, instance2);
    }

    [TestMethod]
    public void InlineArray4_Instance_IsSingleton()
    {
        var instance1 = InlineArray4Formatter<string>.Instance;
        var instance2 = InlineArray4Formatter<string>.Instance;
        Assert.AreSame(instance1, instance2);
    }

    #endregion

    #region Various Size Tests

    [TestMethod]
    public void InlineArray3_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray3Formatter<long>();
        var original = new InlineArray3<long>();
        original[0] = 100L;
        original[1] = 200L;
        original[2] = 300L;

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        Assert.AreEqual(original[0], result[0]);
        Assert.AreEqual(original[1], result[1]);
        Assert.AreEqual(original[2], result[2]);
    }

    [TestMethod]
    public void InlineArray5_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray5Formatter<float>();
        var original = new InlineArray5<float>();
        for (int i = 0; i < 5; i++)
        {
            original[i] = i * 0.5f;
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    [TestMethod]
    public void InlineArray6_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray6Formatter<int>();
        var original = new InlineArray6<int>();
        for (int i = 0; i < 6; i++)
        {
            original[i] = i * 10;
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 6; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    [TestMethod]
    public void InlineArray7_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray7Formatter<int>();
        var original = new InlineArray7<int>();
        for (int i = 0; i < 7; i++)
        {
            original[i] = i + 1;
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 7; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    [TestMethod]
    public void InlineArray9_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray9Formatter<int>();
        var original = new InlineArray9<int>();
        for (int i = 0; i < 9; i++)
        {
            original[i] = i * i;
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    [TestMethod]
    public void InlineArray10_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray10Formatter<int>();
        var original = new InlineArray10<int>();
        for (int i = 0; i < 10; i++)
        {
            original[i] = i * 100;
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 10; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    [TestMethod]
    public void InlineArray11_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray11Formatter<int>();
        var original = new InlineArray11<int>();
        for (int i = 0; i < 11; i++)
        {
            original[i] = i - 5;
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 11; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    [TestMethod]
    public void InlineArray12_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray12Formatter<byte>();
        var original = new InlineArray12<byte>();
        for (int i = 0; i < 12; i++)
        {
            original[i] = (byte)(i * 20);
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 12; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    [TestMethod]
    public void InlineArray13_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray13Formatter<int>();
        var original = new InlineArray13<int>();
        for (int i = 0; i < 13; i++)
        {
            original[i] = i * 7;
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 13; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    [TestMethod]
    public void InlineArray14_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray14Formatter<int>();
        var original = new InlineArray14<int>();
        for (int i = 0; i < 14; i++)
        {
            original[i] = i * 3;
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 14; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    [TestMethod]
    public void InlineArray15_RoundTrip_PreservesData()
    {
        var formatter = new InlineArray15Formatter<int>();
        var original = new InlineArray15<int>();
        for (int i = 0; i < 15; i++)
        {
            original[i] = i * 11;
        }

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        for (int i = 0; i < 15; i++)
        {
            Assert.AreEqual(original[i], result[i]);
        }
    }

    #endregion

    #region Complex Type Tests

    [TestMethod]
    public void InlineArray4_RoundTrip_ComplexType()
    {
        var formatter = new InlineArray4Formatter<(int, string)>();
        var original = new InlineArray4<(int, string)>();
        original[0] = (1, "one");
        original[1] = (2, "two");
        original[2] = (3, "three");
        original[3] = (4, "four");

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        formatter.Serialize(ref writer, original, _options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, _options);

        Assert.AreEqual(original[0], result[0]);
        Assert.AreEqual(original[1], result[1]);
        Assert.AreEqual(original[2], result[2]);
        Assert.AreEqual(original[3], result[3]);
    }

    #endregion
}