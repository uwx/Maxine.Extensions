using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MathUnit;

public readonly record struct Speed : IComparable<Speed>
{
    public static readonly Speed MinValue = new(double.MinValue);
    public static readonly Speed MaxValue = new(double.MaxValue);
    public static readonly Speed PositiveInfinity = new(double.PositiveInfinity);
    public static readonly Speed Zero = new(0);

    public bool IsZero => MetersPerSecond == 0;
    public bool IsPositiveInfinity => double.IsPositiveInfinity(MetersPerSecond);
    public double MetersPerSecond { get; }
    public double KilometersPerSecond => MetersPerSecond / 1_000;
    public double KilometersPerHour => MetersPerSecond * 3_600 / 1_000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Speed(double metersPerSecond)
    {
        MetersPerSecond = metersPerSecond;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed FromMetersPerSecond(double metersPerSecond)
    {
        return new Speed(metersPerSecond);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed FromKilometersPerSecond(double kilometersPerSecond)
    {
        return new Speed(kilometersPerSecond * 1_000);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed FromKilometersPerHour(double kilometersPerHour)
    {
        return new Speed(kilometersPerHour * 1_000 / 3_600);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed operator *(Speed speed, double scalar) => new(speed.MetersPerSecond * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed operator *(double scalar, Speed speed) => new(speed.MetersPerSecond * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed operator /(Speed speed, double scalar) => new(speed.MetersPerSecond / scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double operator /(Speed a, Speed b) => a.MetersPerSecond / b.MetersPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length operator *(Speed speed, TimeSpan time) => Length.FromMeters(speed.MetersPerSecond * time.TotalSeconds);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed operator +(Speed val1, Speed val2) => new(val1.MetersPerSecond + val2.MetersPerSecond);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed operator -(Speed val1, Speed val2) => new(val1.MetersPerSecond - val2.MetersPerSecond);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed operator -(Speed val) => new(-val.MetersPerSecond);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Speed a, Speed b) => a.MetersPerSecond > b.MetersPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Speed a, Speed b) => a.MetersPerSecond >= b.MetersPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Speed a, Speed b) => a.MetersPerSecond < b.MetersPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Speed a, Speed b) => a.MetersPerSecond <= b.MetersPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed Max(Speed val1, Speed val2) => new(Math.Max(val1.MetersPerSecond, val2.MetersPerSecond));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed Min(Speed val1, Speed val2) => new(Math.Min(val1.MetersPerSecond, val2.MetersPerSecond));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Speed Abs() => new(Math.Abs(MetersPerSecond));

    public override string ToString() => $"{MetersPerSecond.ToString(CultureInfo.InvariantCulture)} m/s";
    public string ToString(string format) => $"{MetersPerSecond.ToString(format)} m/s";

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public bool Equals(Speed obj) => MetersPerSecond == obj.MetersPerSecond;

    public override int GetHashCode() => MetersPerSecond.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Speed Min(Speed other) => this < other ? this : other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Speed Max(Speed other) => this > other ? this : other;

    public int CompareTo(Speed other)
    {
        return MetersPerSecond.CompareTo(other.MetersPerSecond);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Sign() => Math.Sign(MetersPerSecond);
}