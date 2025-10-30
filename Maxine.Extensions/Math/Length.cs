using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MathUnit;

public readonly record struct Length<T> : IComparable<Length<T>> where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>
{
    public static readonly Length<T> MinValue = new(T.MinValue);
    public static readonly Length<T> MaxValue = new(T.MaxValue);
    public static readonly Length<T> PositiveInfinity = new(T.PositiveInfinity);
    public static readonly Length<T> Zero = new(T.Zero);

    public bool IsZero => Meters == T.Zero;
    public bool IsPositiveInfinity => T.IsPositiveInfinity(Meters);
    public T Meters { get; }
    public T Centimeters => Meters * T.CreateTruncating(100);
    public T Millimeters => Meters * T.CreateTruncating(1000);
    public T Kilometers => Meters / T.CreateTruncating(1000);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Length(T meters)
    {
        Meters = meters;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> FromMeters(T meters) => new(meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> FromKilometers(T kilometers) => new(kilometers * T.CreateTruncating(1000));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> FromMillimeters(T millimeters) => new(millimeters / T.CreateTruncating(1000));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> FromCentimeters(T centimeters) => new(centimeters / T.CreateTruncating(100));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> operator *(Length<T> length, T scalar) => new(length.Meters * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> operator *(T scalar, Length<T> length) => new(length.Meters * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> operator /(Length<T> length, T scalar) => new(length.Meters / scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> operator /(Length<T> a, Length<T> b) => new(a.Meters / b.Meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> operator /(Length<T> len, TimeSpan time) => Speed<T>.FromMetersPerSecond(len.Meters / T.CreateTruncating(time.TotalSeconds));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan operator /(Length<T> len, Speed<T> speed) => TimeSpan.FromSeconds(double.CreateTruncating(len.Meters / speed.MetersPerSecond));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> operator +(Length<T> len1, Length<T> len2) => new(len1.Meters + len2.Meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> operator -(Length<T> len1, Length<T> len2) => new(len1.Meters - len2.Meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> operator -(Length<T> len) => new(-len.Meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> Max(Length<T> len1, Length<T> len2) => new(T.Max(len1.Meters, len2.Meters));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> Min(Length<T> len1, Length<T> len2) => new(T.Min(len1.Meters, len2.Meters));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Length<T> Abs() => new(T.Abs(Meters));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Length<T> a, Length<T> b) => a.Meters > b.Meters;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Length<T> a, Length<T> b) => a.Meters >= b.Meters;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Length<T> a, Length<T> b) => a.Meters < b.Meters;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Length<T> a, Length<T> b) => a.Meters <= b.Meters;

    public override string ToString() => $"{Meters.ToString(null, CultureInfo.InvariantCulture)}m";

    public string ToString(string format) => $"{Meters.ToString(format, null)}m";

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public bool Equals(Length<T> obj) => Meters == obj.Meters;

    public override int GetHashCode()
    {
        return Meters.GetHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Length<T> Min(Length<T> other) => this < other ? this : other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Length<T> Max(Length<T> other) => this > other ? this : other;

    public int CompareTo(Length<T> other) => Meters.CompareTo(other.Meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Sign() => T.Sign(Meters);
}