using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MathUnit;

public readonly record struct Length : IComparable<Length>
{
    public static readonly Length MinValue = new(double.MinValue);
    public static readonly Length MaxValue = new(double.MaxValue);
    public static readonly Length PositiveInfinity = new(double.PositiveInfinity);
    public static readonly Length Zero = new(0);

    public bool IsZero => Meters == 0;
    public bool IsPositiveInfinity => double.IsPositiveInfinity(Meters);
    public double Meters { get; }
    public double Centimeters => Meters * 100;
    public double Millimeters => Meters * 1000.0;
    public double Kilometers => Meters / 1000.0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Length(double meters)
    {
        Meters = meters;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length FromMeters(double meters) => new(meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length FromKilometers(double kilometers) => new(kilometers * 1000.0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length FromMillimeters(double millimeters) => new(millimeters / 1000.0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length FromCentimeters(double centimeters) => new(centimeters / 100.0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length operator *(Length length, double scalar) => new(length.Meters * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length operator *(double scalar, Length length) => new(length.Meters * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length operator /(Length length, double scalar) => new(length.Meters / scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length operator /(Length a, Length b) => new(a.Meters / b.Meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed operator /(Length len, TimeSpan time) => Speed.FromMetersPerSecond(len.Meters / time.TotalSeconds);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan operator /(Length len, Speed speed) => TimeSpan.FromSeconds(len.Meters / speed.MetersPerSecond);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length operator +(Length len1, Length len2) => new(len1.Meters + len2.Meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length operator -(Length len1, Length len2) => new(len1.Meters - len2.Meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length operator -(Length len) => new(-len.Meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length Max(Length len1, Length len2) => new(Math.Max(len1.Meters, len2.Meters));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length Min(Length len1, Length len2) => new(Math.Min(len1.Meters, len2.Meters));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Length Abs() => new(Math.Abs(Meters));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Length a, Length b) => a.Meters > b.Meters;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Length a, Length b) => a.Meters >= b.Meters;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Length a, Length b) => a.Meters < b.Meters;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Length a, Length b) => a.Meters <= b.Meters;

    public override string ToString() => $"{Meters.ToString(CultureInfo.InvariantCulture)}m";

    public string ToString(string format) => $"{Meters.ToString(format)}m";

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public bool Equals(Length obj) => Meters == obj.Meters;

    public override int GetHashCode()
    {
        return Meters.GetHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Length Min(Length other) => this < other ? this : other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Length Max(Length other) => this > other ? this : other;

    public int CompareTo(Length other) => Meters.CompareTo(other.Meters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Sign() => Math.Sign(Meters);
}