using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Maxine.Extensions;

[PublicAPI]
[StructLayout(LayoutKind.Sequential)]
public struct Nibble(byte value) :
    ISpanFormattable,
    IUtf8SpanFormattable,
    IEquatable<Nibble>,
    IEquatable<byte>,
    IComparable<Nibble>,
    IComparable<byte>,
    IComparable,
    IConvertible,
    IMinMaxValue<Nibble>,
    ISpanParsable<Nibble>,
    IAdditionOperators<Nibble, Nibble, Nibble>,
    IAdditiveIdentity<Nibble, Nibble>,
    IBitwiseOperators<Nibble, Nibble, Nibble>,
    IComparisonOperators<Nibble, Nibble, bool>,
    IDecrementOperators<Nibble>,
    IDivisionOperators<Nibble, Nibble, Nibble>,
    IIncrementOperators<Nibble>,
    IMultiplicativeIdentity<Nibble, Nibble>,
    IMultiplyOperators<Nibble, Nibble, Nibble>,
    ISubtractionOperators<Nibble, Nibble, Nibble>,
    IShiftOperators<Nibble, int, Nibble>,
    IModulusOperators<Nibble, Nibble, Nibble>,
    IUnaryPlusOperators<Nibble, Nibble>
{
    private byte _value = value;

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    public byte Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => _value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _value = value;
    }

    public bool this[int bit]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => ((_value << bit) & 0x01) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var bitMask = 1 << bit;
            if (value)
            {
                _value |= (byte)bitMask;
            }
            else
            {
                _value &= (byte)~bitMask;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Nibble(byte value) => new(value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator byte(Nibble nibble) => nibble._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Nibble other) => _value == other._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(byte other) => _value == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int CompareTo(Nibble other) => _value.CompareTo(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int CompareTo(byte other) => _value.CompareTo(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override bool Equals([NotNullWhen(true)] object? obj)
        => obj?.GetType() == typeof(Nibble) && ((Nibble)obj)._value == _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override int GetHashCode()
        => _value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override string ToString()
        => $"{nameof(_value)}: {_value}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Nibble left, Nibble right) => left._value == right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Nibble left, Nibble right) => !(left == right);

    #region IComparable
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly int IComparable.CompareTo(object? obj) => _value.CompareTo(obj);
    
    #endregion

    #region Formattable
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string? format, IFormatProvider? formatProvider) => _value.ToString(formatProvider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => _value.TryFormat(destination, out charsWritten, format, provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryFormat(Span<byte> destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => _value.TryFormat(destination, out bytesWritten, format, provider);

    #endregion

    #region IConvertible
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly TypeCode IConvertible.GetTypeCode() => _value.GetTypeCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly bool IConvertible.ToBoolean(IFormatProvider? provider) => ((IConvertible)_value).ToBoolean(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly byte IConvertible.ToByte(IFormatProvider? provider) => ((IConvertible)_value).ToByte(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly char IConvertible.ToChar(IFormatProvider? provider) => ((IConvertible)_value).ToChar(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly DateTime IConvertible.ToDateTime(IFormatProvider? provider) => ((IConvertible)_value).ToDateTime(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly decimal IConvertible.ToDecimal(IFormatProvider? provider) => ((IConvertible)_value).ToDecimal(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly double IConvertible.ToDouble(IFormatProvider? provider) => ((IConvertible)_value).ToDouble(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly short IConvertible.ToInt16(IFormatProvider? provider) => ((IConvertible)_value).ToInt16(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly int IConvertible.ToInt32(IFormatProvider? provider) => ((IConvertible)_value).ToInt32(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly long IConvertible.ToInt64(IFormatProvider? provider) => ((IConvertible)_value).ToInt64(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly sbyte IConvertible.ToSByte(IFormatProvider? provider) => ((IConvertible)_value).ToSByte(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly float IConvertible.ToSingle(IFormatProvider? provider) => ((IConvertible)_value).ToSingle(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly string IConvertible.ToString(IFormatProvider? provider) => _value.ToString(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => ((IConvertible)_value).ToType(conversionType, provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly ushort IConvertible.ToUInt16(IFormatProvider? provider) => ((IConvertible)_value).ToUInt16(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly uint IConvertible.ToUInt32(IFormatProvider? provider) => ((IConvertible)_value).ToUInt32(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly ulong IConvertible.ToUInt64(IFormatProvider? provider) => ((IConvertible)_value).ToUInt64(provider);
    
    #endregion

    #region Parsable

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble Parse(string s, IFormatProvider? provider) => byte.Parse(s, provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Nibble result)
    {
        var res = byte.TryParse(s, provider, out var value);
        result = value;
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => byte.Parse(s, provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Nibble result)
    {
        var res = byte.TryParse(s, provider, out var value);
        result = value;
        return res;
    }
    
    #endregion

    #region Number

    static Nibble IAdditiveIdentity<Nibble, Nibble>.AdditiveIdentity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 0;
    }

    static Nibble IMultiplicativeIdentity<Nibble, Nibble>.MultiplicativeIdentity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 1;
    }

    public static Nibble MaxValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => byte.MaxValue;
    }

    public static Nibble MinValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => byte.MinValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator +(Nibble value) => value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator +(Nibble left, Nibble right) => (byte)(left._value + right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator &(Nibble left, Nibble right) => (byte)(left._value & right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator |(Nibble left, Nibble right) => (byte)(left._value | right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator ^(Nibble left, Nibble right) => (byte)(left._value ^ right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator ~(Nibble value) => (byte)~value._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Nibble left, Nibble right) =>  left._value > right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Nibble left, Nibble right) => left._value >= right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Nibble left, Nibble right) =>  left._value < right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Nibble left, Nibble right) => left._value <= right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator --(Nibble value) => (byte)(value._value-1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator /(Nibble left, Nibble right) => (byte)(left._value / right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator %(Nibble left, Nibble right) => (byte)(left._value % right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator ++(Nibble value) => (byte)(value._value+1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator *(Nibble left, Nibble right) => (byte)(left._value * right._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator -(Nibble left, Nibble right) => (byte)(left._value - right._value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator <<(Nibble value, int shiftAmount) => (byte)(value._value << shiftAmount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator >>(Nibble value, int shiftAmount) => (byte)(value._value >> shiftAmount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble operator >>>(Nibble value, int shiftAmount) => (byte)(value._value >>> shiftAmount);

    #endregion
}