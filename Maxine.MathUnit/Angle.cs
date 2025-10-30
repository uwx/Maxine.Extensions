using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MathUnit;

public readonly record struct Angle : IComparable<Angle>
{
    public static Angle Zero { get; } = new(0);
    public static Angle PI { get; } = new(Math.PI);
    public static Angle FullCircle { get; } = new(2 * Math.PI);
    public static Angle NaN { get; } = new(double.NaN);

    public double Degrees
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Radians * 180 / Math.PI;
    }

    public double Radians { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Angle(double radians)
    {
        Radians = radians;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle FromDegrees(double degrees)
    {
        return new Angle(degrees * Math.PI / 180.0);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle FromDegrees(int degrees, int minutes, double seconds)
    {
        return FromDegrees(degrees + minutes / 60.0 + seconds / 3600.0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle FromRadians(double radians)
    {
        return new Angle(radians);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle Min(Angle @this, Angle other) => @this < other ? @this : other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle Max(Angle @this, Angle other) => @this > other ? @this : other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(Angle @this) => Math.Sign(@this.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sin(Angle @this) => Math.Sin(@this.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cos(Angle @this) => Math.Cos(@this.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Tan(Angle @this) => Math.Tan(@this.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle Normalize() => new(Math2.Mod(Radians, 2 * Math.PI));

    public static Angle Distance(Angle a, Angle b)
    {
        var diff = (a - b).Normalize();
        return diff <= PI
            ? diff
            : 2 * PI - diff;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle Abs() => this >= Zero ? this : -this;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator -(Angle a, Angle b) => new(a.Radians - b.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator -(Angle a) => new(-a.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator +(Angle a, Angle b) => new(a.Radians + b.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Angle a, Angle b) => a.Radians < b.Radians;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Angle a, Angle b) => a.Radians <= b.Radians;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Angle a, Angle b) => a.Radians > b.Radians;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Angle a, Angle b) => a.Radians >= b.Radians;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double operator /(Angle a, Angle b) => a.Radians / b.Radians;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator *(Angle a, double scalar) => new(a.Radians * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator /(Angle a, double scalar) => new(a.Radians / scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle operator *(double scalar, Angle a) => new(a.Radians * scalar);

    public override string ToString() => ToString(CultureInfo.InvariantCulture);

    public string ToString(string format) => $"{Degrees.ToString(format)}°";

    public string ToString(CultureInfo culture) => $"{Degrees.ToString(culture)}°";

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public bool Equals(Angle obj) => Radians == obj.Radians;

    public override int GetHashCode() => Radians.GetHashCode();

    public int CompareTo(Angle other) => Radians.CompareTo(other.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle AddDegrees(float deg) => FromDegrees(Degrees + deg);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle AddDegrees(double deg) => FromDegrees(Degrees + deg);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle AddRadians(float rad) => FromRadians(Radians + rad);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle AddRadians(double rad) => FromRadians(Radians + rad);

    /// <inheritdoc cref="Math.Atan2"/>
    public static Angle Atan2(double y, double x) => new(Math.Atan2(y, x));

    /// <inheritdoc cref="Math.Asin"/>
    public static Angle Asin(double a) => new(Math.Asin(a));
}