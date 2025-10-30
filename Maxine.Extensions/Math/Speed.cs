using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Maxine.Extensions;

public readonly record struct Speed<T> : IComparable<Speed<T>> where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>
{
    public static readonly Speed<T> MinValue = new(T.MinValue);
    public static readonly Speed<T> MaxValue = new(T.MaxValue);
    public static readonly Speed<T> PositiveInfinity = new(T.PositiveInfinity);
    public static readonly Speed<T> Zero = new(T.Zero);

    public bool IsZero => T.IsZero(MetersPerSecond);
    public bool IsPositiveInfinity => T.IsPositiveInfinity(MetersPerSecond);
    public T MetersPerSecond { get; }
    public T KilometersPerSecond => MetersPerSecond / T.CreateTruncating(1_000);
    public T KilometersPerHour => MetersPerSecond * T.CreateTruncating(3_600) / T.CreateTruncating(1_000);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Speed(T metersPerSecond)
    {
        MetersPerSecond = metersPerSecond;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> FromMetersPerSecond(T metersPerSecond)
    {
        return new Speed<T>(metersPerSecond);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> FromKilometersPerSecond(T kilometersPerSecond)
    {
        return new Speed<T>(kilometersPerSecond * T.CreateTruncating(1_000));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> FromKilometersPerHour(T kilometersPerHour)
    {
        return new Speed<T>(kilometersPerHour * T.CreateTruncating(1_000) / T.CreateTruncating(3_600));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> operator *(Speed<T> speed, T scalar) => new(speed.MetersPerSecond * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> operator *(T scalar, Speed<T> speed) => new(speed.MetersPerSecond * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> operator /(Speed<T> speed, T scalar) => new(speed.MetersPerSecond / scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T operator /(Speed<T> a, Speed<T> b) => a.MetersPerSecond / b.MetersPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> operator *(Speed<T> speed, TimeSpan time) => Length<T>.FromMeters(speed.MetersPerSecond * T.CreateTruncating(time.TotalSeconds));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> operator +(Speed<T> val1, Speed<T> val2) => new(val1.MetersPerSecond + val2.MetersPerSecond);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> operator -(Speed<T> val1, Speed<T> val2) => new(val1.MetersPerSecond - val2.MetersPerSecond);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> operator -(Speed<T> val) => new(-val.MetersPerSecond);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Speed<T> a, Speed<T> b) => a.MetersPerSecond > b.MetersPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Speed<T> a, Speed<T> b) => a.MetersPerSecond >= b.MetersPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Speed<T> a, Speed<T> b) => a.MetersPerSecond < b.MetersPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Speed<T> a, Speed<T> b) => a.MetersPerSecond <= b.MetersPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> Max(Speed<T> val1, Speed<T> val2) => new(T.Max(val1.MetersPerSecond, val2.MetersPerSecond));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Speed<T> Min(Speed<T> val1, Speed<T> val2) => new(T.Min(val1.MetersPerSecond, val2.MetersPerSecond));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Speed<T> Abs() => new(T.Abs(MetersPerSecond));

    public override string ToString() => $"{MetersPerSecond.ToString(null, CultureInfo.InvariantCulture)} m/s";
    public string ToString(string format) => $"{MetersPerSecond.ToString(format, null)} m/s";

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public bool Equals(Speed<T> obj) => MetersPerSecond == obj.MetersPerSecond;

    public override int GetHashCode() => MetersPerSecond.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Speed<T> Min(Speed<T> other) => this < other ? this : other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Speed<T> Max(Speed<T> other) => this > other ? this : other;

    public int CompareTo(Speed<T> other)
    {
        return MetersPerSecond.CompareTo(other.MetersPerSecond);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Sign() => T.Sign(MetersPerSecond);
}