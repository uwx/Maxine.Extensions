using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Maxine.Extensions;

public readonly record struct Angle<T> : IComparable<Angle<T>> where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>
{
    private static T _Pi => T.CreateTruncating(Math.PI);
    private static T _180 => T.CreateTruncating(180);
    private static T _60 => T.CreateTruncating(60);
    private static T _3600 => T.CreateTruncating(3600);
    private static T TwoPi => T.CreateTruncating(2) * _Pi;

    public static Angle<T> Zero { get; } = new(T.Zero);
    public static Angle<T> PI { get; } = new(_Pi);
    public static Angle<T> FullCircle { get; } = new(T.CreateTruncating(2) * _Pi);
    public static Angle<T> NaN { get; } = new(T.NaN);

    public T Degrees
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Radians * _180 / _Pi;
    }

    public T Radians { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Angle(T radians)
    {
        Radians = radians;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> FromDegrees(T degrees)
    {
        return new Angle<T>(degrees * _Pi / _180);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> FromDegrees<TInt>(TInt degrees, TInt minutes, T seconds) where TInt : INumber<TInt>
    {
        return FromDegrees(T.CreateTruncating(degrees) + T.CreateTruncating(minutes) / _60 + seconds / _3600);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> FromRadians(T radians)
    {
        return new Angle<T>(radians);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> Min(Angle<T> @this, Angle<T> other) => @this < other ? @this : other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> Max(Angle<T> @this, Angle<T> other) => @this > other ? @this : other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(Angle<T> @this) => T.Sign(@this.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sin(Angle<T> @this) => T.Sin(@this.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cos(Angle<T> @this) => T.Cos(@this.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Tan(Angle<T> @this) => T.Tan(@this.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle<T> Normalize() => new(Math2.Mod(Radians, TwoPi));

    public static Angle<T> Distance(Angle<T> a, Angle<T> b)
    {
        var diff = (a - b).Normalize();
        return diff <= PI
            ? diff
            : T.CreateTruncating(2) * PI - diff;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle<T> Abs() => this >= Zero ? this : -this;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> operator -(Angle<T> a, Angle<T> b) => new(a.Radians - b.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> operator -(Angle<T> a) => new(-a.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> operator +(Angle<T> a, Angle<T> b) => new(a.Radians + b.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Angle<T> a, Angle<T> b) => a.Radians < b.Radians;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Angle<T> a, Angle<T> b) => a.Radians <= b.Radians;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Angle<T> a, Angle<T> b) => a.Radians > b.Radians;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Angle<T> a, Angle<T> b) => a.Radians >= b.Radians;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T operator /(Angle<T> a, Angle<T> b) => a.Radians / b.Radians;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> operator *(Angle<T> a, T scalar) => new(a.Radians * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> operator /(Angle<T> a, T scalar) => new(a.Radians / scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle<T> operator *(T scalar, Angle<T> a) => new(a.Radians * scalar);

    public override string ToString() => ToString(CultureInfo.InvariantCulture);

    public string ToString(string format) => $"{Degrees.ToString(format, null)}°";

    public string ToString(CultureInfo culture) => $"{Degrees.ToString(null, culture)}°";

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public bool Equals(Angle<T> obj) => Radians == obj.Radians;

    public override int GetHashCode() => Radians.GetHashCode();

    public int CompareTo(Angle<T> other) => Radians.CompareTo(other.Radians);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle<T> AddDegrees(T deg) => FromDegrees(Degrees + deg);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Angle<T> AddRadians(T rad) => FromRadians(Radians + rad);

    /// <inheritdoc cref="Math.Atan2"/>
    public static Angle<T> Atan2(T y, T x) => new(T.Atan2(y, x));

    /// <inheritdoc cref="Math.Asin"/>
    public static Angle<T> Asin(T a) => new(T.Asin(a));
}