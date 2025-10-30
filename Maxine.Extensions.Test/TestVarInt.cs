using System.Numerics;

namespace Maxine.Extensions.Test;

[TestClass]
public class TestVarInt
{
    public static IEnumerable<T> Data<T>() where T : unmanaged, INumber<T>, IMinMaxValue<T>
        => Range(T.MinValue, T.MaxValue, T.MaxValue / T.CreateTruncating(100000));

    private static IEnumerable<T> Range<T>(T start, T count, T? increment = null) where T : unmanaged, INumber<T>
    {
        increment ??= T.One;
        var end = start + count;
        for (var current = start; current < end; current += increment.Value)
        {
            yield return current;
        }
    }
    
    [TestMethod]
    public void TestInt()
    {
        Span<byte> buffer = stackalloc byte[10];
        foreach (var data in Data<int>())
        {
            var tryGetSignedBytes = VarInt.TryGetSignedBytes(data, buffer, out var bytesWritten);
            Assert.IsTrue(tryGetSignedBytes);

            Assert.IsTrue(VarIntBinaryPrimitives.GetBytes(data).SequenceEqual(buffer[..bytesWritten]));
        }
    }
}

file class VarIntBinaryPrimitives
{
    
    #region Non Generic

    
    /// <summary>
    /// Returns the specified byte value as varint encoded array of bytes.   
    /// </summary>
    /// <param name="value">Byte value</param>
    /// <returns>Varint array of bytes.</returns>
    public static ReadOnlySpan<byte> GetBytes(byte value)
    {
        return GetBytes((ulong)value);
    }

    /// <summary>
    /// Returns the specified 16-bit signed value as varint encoded array of bytes.   
    /// </summary>
    /// <param name="value">16-bit signed value</param>
    /// <returns>Varint array of bytes.</returns>
    public static ReadOnlySpan<byte> GetBytes(short value)
    {
        var zigzag = EncodeZigZag(value, 16);
        return GetBytes((ulong)zigzag);
    }

    /// <summary>
    /// Returns the specified 16-bit unsigned value as varint encoded array of bytes.   
    /// </summary>
    /// <param name="value">16-bit unsigned value</param>
    /// <returns>Varint array of bytes.</returns>
    public static ReadOnlySpan<byte> GetBytes(ushort value)
    {
        return GetBytes((ulong)value);
    }

    /// <summary>
    /// Returns the specified 32-bit signed value as varint encoded array of bytes.   
    /// </summary>
    /// <param name="value">32-bit signed value</param>
    /// <returns>Varint array of bytes.</returns>
    public static ReadOnlySpan<byte> GetBytes(int value)
    {
        var zigzag = EncodeZigZag(value, 32);
        return GetBytes((ulong)zigzag);
    }

    /// <summary>
    /// Returns the specified 32-bit unsigned value as varint encoded array of bytes.   
    /// </summary>
    /// <param name="value">32-bit unsigned value</param>
    /// <returns>Varint array of bytes.</returns>
    public static ReadOnlySpan<byte> GetBytes(uint value)
    {
        return GetBytes((ulong)value);
    }

    /// <summary>
    /// Returns the specified 64-bit signed value as varint encoded array of bytes.   
    /// </summary>
    /// <param name="value">64-bit signed value</param>
    /// <returns>Varint array of bytes.</returns>
    public static ReadOnlySpan<byte> GetBytes(long value)
    {
        var zigzag = EncodeZigZag(value, 64);
        return GetBytes((ulong)zigzag);
    }

    /// <summary>
    /// Returns the specified 64-bit unsigned value as varint encoded array of bytes.   
    /// </summary>
    /// <param name="value">64-bit unsigned value</param>
    /// <returns>Varint array of bytes.</returns>
    public static ReadOnlySpan<byte> GetBytes(ulong value)
    {
        var buffer = new byte[10];
        var pos = 0;
        do
        {
            var byteVal = value & 0x7f;
            value >>= 7;

            if (value != 0)
            {
                byteVal |= 0x80;
            }

            buffer[pos++] = (byte)byteVal;

        } while (value != 0);

        var result = new byte[pos];
        Buffer.BlockCopy(buffer, 0, result, 0, pos);

        return result;
    }

    /// <summary>
    /// Returns byte value from varint encoded array of bytes.
    /// </summary>
    /// <param name="bytes">Varint encoded array of bytes.</param>
    /// <returns>Byte value</returns>
    public static byte ToByte(ReadOnlySpan<byte> bytes)
    {
        return (byte)ToTarget(bytes, 8);
    }

    /// <summary>
    /// Returns 16-bit signed value from varint encoded array of bytes.
    /// </summary>
    /// <param name="bytes">Varint encoded array of bytes.</param>
    /// <returns>16-bit signed value</returns>
    public static short ToInt16(ReadOnlySpan<byte> bytes)
    {
        var zigzag = ToTarget(bytes, 16);
        return (short)DecodeZigZag(zigzag);
    }

    /// <summary>
    /// Returns 16-bit usigned value from varint encoded array of bytes.
    /// </summary>
    /// <param name="bytes">Varint encoded array of bytes.</param>
    /// <returns>16-bit usigned value</returns>
    public static ushort ToUInt16(ReadOnlySpan<byte> bytes)
    {
        return (ushort)ToTarget(bytes, 16);
    }

    /// <summary>
    /// Returns 32-bit signed value from varint encoded array of bytes.
    /// </summary>
    /// <param name="bytes">Varint encoded array of bytes.</param>
    /// <returns>32-bit signed value</returns>
    public static int ToInt32(ReadOnlySpan<byte> bytes)
    {
        var zigzag = ToTarget(bytes, 32);
        return (int)DecodeZigZag(zigzag);
    }

    /// <summary>
    /// Returns 32-bit unsigned value from varint encoded array of bytes.
    /// </summary>
    /// <param name="bytes">Varint encoded array of bytes.</param>
    /// <returns>32-bit unsigned value</returns>
    public static uint ToUInt32(ReadOnlySpan<byte> bytes)
    {
        return (uint)ToTarget(bytes, 32);
    }

    /// <summary>
    /// Returns 64-bit signed value from varint encoded array of bytes.
    /// </summary>
    /// <param name="bytes">Varint encoded array of bytes.</param>
    /// <returns>64-bit signed value</returns>
    public static long ToInt64(ReadOnlySpan<byte> bytes)
    {
        var zigzag = ToTarget(bytes, 64);
        return DecodeZigZag(zigzag);
    }

    /// <summary>
    /// Returns 64-bit unsigned value from varint encoded array of bytes.
    /// </summary>
    /// <param name="bytes">Varint encoded array of bytes.</param>
    /// <returns>64-bit unsigned value</returns>
    public static ulong ToUInt64(ReadOnlySpan<byte> bytes)
    {
        return ToTarget(bytes, 64);
    }

    private static long EncodeZigZag(long value, int bitLength)
    {
        return (value << 1) ^ (value >> (bitLength - 1));
    }

    private static long DecodeZigZag(ulong value)
    {
        if ((value & 0x1) == 0x1)
        {
            return (-1 * ((long)(value >> 1) + 1));
        }

        return (long)(value >> 1);
    }

    private static ulong ToTarget(ReadOnlySpan<byte> bytes, int sizeBites)
    {
        int shift = 0;
        ulong result = 0;

        foreach (ulong byteValue in bytes)
        {
            ulong tmp = byteValue & 0x7f;
            result |= tmp << shift;

            if (shift > sizeBites)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "Byte array is too large.");
            }

            if ((byteValue & 0x80) != 0x80)
            {
                return result;
            }

            shift += 7;
        }

        throw new ArgumentException("Cannot decode varint from byte array.", nameof(bytes));
    }

    #endregion
    
}