using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Maxine.Extensions;

[PublicAPI]
[StructLayout(LayoutKind.Sequential)]
public struct Nibble<T> :
    ISpanFormattable,
    IUtf8SpanFormattable,
    IEquatable<Nibble<T>>,
    IEquatable<T>,
    IComparable<Nibble<T>>,
    IComparable<T>,
    IComparable,
    IConvertible,
    IMinMaxValue<Nibble<T>>,
    ISpanParsable<Nibble<T>>,
    IAdditionOperators<Nibble<T>, Nibble<T>, Nibble<T>>,
    IAdditiveIdentity<Nibble<T>, Nibble<T>>,
    IBitwiseOperators<Nibble<T>, Nibble<T>, Nibble<T>>,
    IComparisonOperators<Nibble<T>, Nibble<T>, bool>,
    IDecrementOperators<Nibble<T>>,
    IDivisionOperators<Nibble<T>, Nibble<T>, Nibble<T>>,
    IIncrementOperators<Nibble<T>>,
    IMultiplicativeIdentity<Nibble<T>, Nibble<T>>,
    IMultiplyOperators<Nibble<T>, Nibble<T>, Nibble<T>>,
    ISubtractionOperators<Nibble<T>, Nibble<T>, Nibble<T>>,
    IShiftOperators<Nibble<T>, int, Nibble<T>>,
    IModulusOperators<Nibble<T>, Nibble<T>, Nibble<T>>,
    IUnaryPlusOperators<Nibble<T>, Nibble<T>>
    where T : IBinaryInteger<T>, IMinMaxValue<T>, IConvertible
{
    public static Nibble<T> AllZeros => new();

    private T _value;

    public Nibble(ReadOnlySpan<bool> parts)
    {
        _value = T.Zero;
        for (var i = 0; i < parts.Length; i++)
        {
            if (parts[i])
            {
                _value |= T.One << i;
            }
        }
    }

    public Nibble(T value)
    {
        _value = value;
    }

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    public T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => _value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _value = value;
    }

    public bool this[int bit]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => ((_value >> bit) & T.One) != T.Zero;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var bitMask = T.One << bit;
            if (value)
            {
                _value |= bitMask;
            }
            else
            {
                _value &= ~bitMask;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Nibble<T>(T value) => new(value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T(Nibble<T> nibble) => nibble._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Nibble<T> other) => _value == other._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(T? other) => _value == other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int CompareTo(Nibble<T> other) => _value.CompareTo(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int CompareTo(T? other) => _value.CompareTo(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override bool Equals([NotNullWhen(true)] object? obj)
        => obj?.GetType() == typeof(Nibble<T>) && ((Nibble<T>)obj)._value == _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override int GetHashCode()
        => _value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override string ToString()
        => $"{nameof(_value)}: {_value}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Nibble<T> left, Nibble<T> right) => left._value == right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Nibble<T> left, Nibble<T> right) => !(left == right);

    #region IComparable
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly int IComparable.CompareTo(object? obj) => _value.CompareTo(obj);
    
    #endregion

    #region Formattable
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string? format, IFormatProvider? formatProvider) => _value.ToString(format, formatProvider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => _value.TryFormat(destination, out charsWritten, format, provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryFormat(Span<byte> destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => _value.TryFormat(destination, out bytesWritten, format, provider);

    #endregion

    #region IConvertible
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly TypeCode IConvertible.GetTypeCode() => _value.GetTypeCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly bool IConvertible.ToBoolean(IFormatProvider? provider) => _value.ToBoolean(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly byte IConvertible.ToByte(IFormatProvider? provider) => _value.ToByte(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly char IConvertible.ToChar(IFormatProvider? provider) => _value.ToChar(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly DateTime IConvertible.ToDateTime(IFormatProvider? provider) => _value.ToDateTime(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly decimal IConvertible.ToDecimal(IFormatProvider? provider) => _value.ToDecimal(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly double IConvertible.ToDouble(IFormatProvider? provider) => _value.ToDouble(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly short IConvertible.ToInt16(IFormatProvider? provider) => _value.ToInt16(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly int IConvertible.ToInt32(IFormatProvider? provider) => _value.ToInt32(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly long IConvertible.ToInt64(IFormatProvider? provider) => _value.ToInt64(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly sbyte IConvertible.ToSByte(IFormatProvider? provider) => _value.ToSByte(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly float IConvertible.ToSingle(IFormatProvider? provider) => _value.ToSingle(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly string IConvertible.ToString(IFormatProvider? provider) => _value.ToString(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => _value.ToType(conversionType, provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly ushort IConvertible.ToUInt16(IFormatProvider? provider) => _value.ToUInt16(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly uint IConvertible.ToUInt32(IFormatProvider? provider) => _value.ToUInt32(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly ulong IConvertible.ToUInt64(IFormatProvider? provider) => _value.ToUInt64(provider);
    
    #endregion

    #region Parsable

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> Parse(string s, IFormatProvider? provider) => T.Parse(s, provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Nibble<T> result)
    {
        var res = T.TryParse(s, provider, out var value);
        result = value!;
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => T.Parse(s, provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Nibble<T> result)
    {
        var res = T.TryParse(s, provider, out var value);
        result = value!;
        return res;
    }
    
    #endregion

    #region Number

    static Nibble<T> IAdditiveIdentity<Nibble<T>, Nibble<T>>.AdditiveIdentity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => T.AdditiveIdentity;
    }

    static Nibble<T> IMultiplicativeIdentity<Nibble<T>, Nibble<T>>.MultiplicativeIdentity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => T.MultiplicativeIdentity;
    }

    public static Nibble<T> MaxValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => T.MaxValue;
    }

    public static Nibble<T> MinValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => T.MinValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator +(Nibble<T> value) => value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator +(Nibble<T> left, Nibble<T> right) => left._value + right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator &(Nibble<T> left, Nibble<T> right) => left._value & right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator |(Nibble<T> left, Nibble<T> right) => left._value | right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator ^(Nibble<T> left, Nibble<T> right) => left._value ^ right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator ~(Nibble<T> value) => ~value._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Nibble<T> left, Nibble<T> right) =>  left._value > right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Nibble<T> left, Nibble<T> right) => left._value >= right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Nibble<T> left, Nibble<T> right) =>  left._value < right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Nibble<T> left, Nibble<T> right) => left._value <= right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator --(Nibble<T> value) => value._value-T.One;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator /(Nibble<T> left, Nibble<T> right) => left._value / right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator %(Nibble<T> left, Nibble<T> right) => left._value % right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator ++(Nibble<T> value) => value._value+T.One;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator *(Nibble<T> left, Nibble<T> right) => left._value * right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator -(Nibble<T> left, Nibble<T> right) => left._value - right._value;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator <<(Nibble<T> value, int shiftAmount) => value._value << shiftAmount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator >>(Nibble<T> value, int shiftAmount) => value._value >> shiftAmount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Nibble<T> operator >>>(Nibble<T> value, int shiftAmount) => value._value >>> shiftAmount;

    #endregion
}