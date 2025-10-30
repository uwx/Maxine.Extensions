using System.Numerics;
using System.Runtime.CompilerServices;

namespace Maxine.Extensions;

public struct VarInt
{
    #region Generic
    public static bool TryGetSignedBytes<T>(T value, Span<byte> buffer, out int bytesWritten)
        where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>
        => TryGetSignedBytes<T, ulong>(value, buffer, out bytesWritten);

    public static bool TryGetSignedBytes<T, TUnsigned>(T value, Span<byte> buffer, out int bytesWritten)
        where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>
        where TUnsigned : unmanaged, IUnsignedNumber<TUnsigned>, IBinaryInteger<TUnsigned>
    {
        var zigzag = EncodeZigZag(value);
        return TryGetBytes(TUnsigned.CreateTruncating(zigzag), buffer, out bytesWritten);
    }

    public static bool TryGetBytes<T>(T value, Span<byte> buffer, out int bytesWritten) where T : IUnsignedNumber<T>, IBinaryInteger<T>
    {
        bytesWritten = 0;
        do
        {
            var byteVal = value & T.CreateTruncating(0x7f);
            value >>= 7;
            
            if (!T.IsZero(value))
            {
                byteVal |= T.CreateTruncating(0x80);
            }

            if (buffer.Length <= bytesWritten)
            {
                return false;
            }

            buffer[bytesWritten++] = byte.CreateTruncating(byteVal);

        } while (!T.IsZero(value));

        return true;
    }

    private static unsafe T EncodeZigZag<T>(T value) where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>
    {
        var bitLength = sizeof(T) * 8;

        return (value << 1) ^ (value >> (bitLength - 1));
    }
    
    private static TSigned DecodeZigZag<TSigned, TUnsigned>(TUnsigned value)
        where TUnsigned : IUnsignedNumber<TUnsigned>, IBinaryInteger<TUnsigned>
        where TSigned : ISignedNumber<TSigned>, IBinaryInteger<TSigned>
    {
        if ((value & TUnsigned.One) == TUnsigned.One)
        {
            return (TSigned.NegativeOne * (TSigned.CreateTruncating(value >> 1) + TSigned.One));
        }

        return TSigned.CreateTruncating(value >> 1);
    }

    public static T ToSigned<T>(ReadOnlySpan<byte> bytes)
        where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>
        => ToSigned<T, ulong>(bytes);

    public static T ToSigned<T, TUnsigned>(ReadOnlySpan<byte> bytes)
        where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>
        where TUnsigned : unmanaged, IUnsignedNumber<TUnsigned>, IBinaryInteger<TUnsigned>
    {
        var zigzag = ToUnsigned<TUnsigned>(bytes);
        return DecodeZigZag<T, TUnsigned>(zigzag);
    }

    private static unsafe T ToUnsigned<T>(ReadOnlySpan<byte> bytes) where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>
    {
        var bitLength = sizeof(T) * 8;

        var shift = 0;
        var result = T.Zero;

        foreach (var byteValue in bytes)
        {
            var tmp = byteValue & 0x7f;
            result |= T.CreateTruncating(tmp) << shift;

            if (shift > bitLength)
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