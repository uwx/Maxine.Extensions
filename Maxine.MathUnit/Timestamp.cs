using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MathUnit;

/// <summary>
/// A timestamp represented in ticks, with the precision of <see cref="Stopwatch.GetTimestamp"/>
/// </summary>
public readonly record struct Timestamp : IComparable<Timestamp>
{
    // Conversion rate from Stopwatch ticks to DateTime ticks
    private static readonly double TickFrequency = (double)TimeSpan.TicksPerSecond / Stopwatch.Frequency;

    public long Ticks { get; }

    public double TotalDays => (double)Ticks / TimeSpan.TicksPerDay;

    public double TotalHours => (double)Ticks / TimeSpan.TicksPerHour;

    [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
    public double TotalMilliseconds
    {
        get
        {
            var temp = (double)Ticks / TimeSpan.TicksPerMillisecond;
            
            const long maxMilliseconds = long.MaxValue / TimeSpan.TicksPerMillisecond;
            if (temp > maxMilliseconds)
                return maxMilliseconds;

            const long minMilliseconds = long.MinValue / TimeSpan.TicksPerMillisecond;
            if (temp < minMilliseconds)
                return minMilliseconds;

            return temp;
        }
    }

    /// <summary>
    /// Gets the value of the current <see cref="TimeSpan"/> structure expressed in whole and fractional microseconds.
    /// </summary>
    public double TotalMicroseconds => (double)Ticks / TimeSpan.TicksPerMicrosecond;

    /// <summary>
    /// Gets the value of the current <see cref="TimeSpan"/> structure expressed in whole and fractional nanoseconds.
    /// </summary>
    public double TotalNanoseconds => (double)Ticks * TimeSpan.NanosecondsPerTick;

    public double TotalMinutes => (double)Ticks / TimeSpan.TicksPerMinute;

    public double TotalSeconds => (double)Ticks / TimeSpan.TicksPerSecond;
    
    public TimeSpan ElapsedSince
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Now - this;
    }

    // Converts timestamp in HPT ticks to DateTime ticks
    public static Timestamp Now
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(unchecked((long)(Stopwatch.GetTimestamp() * TickFrequency)));
    }

    // Expects DateTime ticks, not Stopwatch ticks.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Timestamp(long ticks)
    {
        Ticks = ticks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeSpan ElapsedUntil(Timestamp end) => end - this;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Timestamp FromTicks(long ticks) => new(ticks);

    // Not stopwatch.ElapsedTicks, as those are HPT ticks not DateTime ticks
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Timestamp FromStopwatch(Stopwatch stopwatch) => new(stopwatch.Elapsed.Ticks);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan operator -(Timestamp a, Timestamp b) => TimeSpan.FromTicks(a.Ticks - b.Ticks);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Timestamp operator +(Timestamp a, TimeSpan b) => new(a.Ticks + b.Ticks);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Timestamp operator -(Timestamp a, TimeSpan b) => new(a.Ticks - b.Ticks);

    public int CompareTo(Timestamp other) => Ticks.CompareTo(other.Ticks);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Timestamp left, Timestamp right) => left.CompareTo(right) < 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Timestamp left, Timestamp right) => left.CompareTo(right) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Timestamp left, Timestamp right) => left.CompareTo(right) <= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Timestamp left, Timestamp right) => left.CompareTo(right) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Timestamp Older(Timestamp a, Timestamp b) => new(Math.Min(a.Ticks, b.Ticks));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Timestamp Newer(Timestamp a, Timestamp b) => new(Math.Max(a.Ticks, b.Ticks));
}