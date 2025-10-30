using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Maxine.Extensions;

using M = MethodImplAttribute;

/// <summary>
/// Represents an instant in UTC time with no timezone information.
/// </summary>
/// <param name="Ticks">
/// The tick count for this <see cref="Instant"/>, also known as the number of 100-nanosecond intervals that have
/// elapsed since 1/1/0001 12:00am.
/// </param>
[PublicAPI]
public readonly record struct Instant(long Ticks) :
    IEquatable<DateTime>,
    IEquatable<DateTimeOffset>,
    IComparable<Instant>,
    IComparable<DateTime>,
    IComparable<DateTimeOffset>,
    IComparisonOperators<Instant, Instant, bool>,
    IComparisonOperators<Instant, DateTime, bool>,
    IComparisonOperators<Instant, DateTimeOffset, bool>,
    IAdditionOperators<Instant, TimeSpan, Instant>,
    ISubtractionOperators<Instant, Instant, TimeSpan>,
    ISubtractionOperators<Instant, TimeSpan, Instant>
{
    // Conversion rate from Stopwatch ticks to DateTime ticks
    private static readonly double TickFrequency = (double)TimeSpan.TicksPerSecond / Stopwatch.Frequency;

    private const MethodImplOptions A = MethodImplOptions.AggressiveInlining;

    private readonly long _ticks = Ticks;

    /// <summary>
    /// Returns the tick count for this DateTime. The returned value is
    /// the number of 100-nanosecond intervals that have elapsed since 1/1/0001
    /// 12:00am.
    /// </summary>
    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    public long Ticks
    {
        [M(A)] get => _ticks;
    }

    public double TotalDays => (double)_ticks / TimeSpan.TicksPerDay;

    public double TotalHours => (double)_ticks / TimeSpan.TicksPerHour;

    [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
    public double TotalMilliseconds
    {
        [M(A)]
        get
        {
            var temp = (double)_ticks / TimeSpan.TicksPerMillisecond;
            
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
    public double TotalMicroseconds
    {
        [M(A)] get => (double)_ticks / TimeSpan.TicksPerMicrosecond;
    }

    /// <summary>
    /// Gets the value of the current <see cref="TimeSpan"/> structure expressed in whole and fractional nanoseconds.
    /// </summary>
    public double TotalNanoseconds
    {
        [M(A)] get => (double)_ticks * TimeSpan.NanosecondsPerTick;
    }

    public double TotalMinutes
    {
        [M(A)] get => (double)_ticks / TimeSpan.TicksPerMinute;
    }

    public double TotalSeconds
    {
        [M(A)] get => (double)_ticks / TimeSpan.TicksPerSecond;
    }

    public TimeSpan ElapsedSince
    {
        [M(A)] get => Now - this;
    }

    // Converts timestamp in HPT ticks to DateTime ticks
    public static Instant Now
    {
        [M(A)] get => new(unchecked((long)(Stopwatch.GetTimestamp() * TickFrequency)));
    }

    [M(A)]
    public Instant(DateTimeOffset dateTimeOffset) : this(dateTimeOffset.UtcTicks)
    {
    }

    [M(A)]
    public Instant(DateTime dateTime) : this(dateTime.Ticks)
    {
    }

    [M(A)] public static implicit operator DateTimeOffset(Instant instant) => new(instant._ticks, TimeSpan.Zero);
    [M(A)] public static implicit operator DateTime(Instant instant) => new(instant._ticks);
    [M(A)] public static implicit operator Instant(DateTimeOffset dateTimeOffset) => new(dateTimeOffset);
    [M(A)] public static implicit operator Instant(DateTime dateTime) => new(dateTime);

    [M(A)] public bool Equals(Instant other) => _ticks == other._ticks;
    [M(A)] public bool Equals(DateTime other) => _ticks == other.Ticks;
    [M(A)] public bool Equals(DateTimeOffset other) => _ticks == other.UtcTicks;

    [M(A)] public override int GetHashCode() => _ticks.GetHashCode();

    [M(A)] public int CompareTo(Instant other) => _ticks.CompareTo(other._ticks);
    [M(A)] public int CompareTo(DateTime other) => _ticks.CompareTo(other.Ticks);
    [M(A)] public int CompareTo(DateTimeOffset other) => _ticks.CompareTo(other.UtcTicks);

    [M(A)] public static bool operator >=(Instant left, Instant right) => left._ticks >= right._ticks;
    [M(A)] public static bool operator <=(Instant left, Instant right) => left._ticks <= right._ticks;
    [M(A)] public static bool operator <(Instant left, Instant right) => left._ticks < right._ticks;
    [M(A)] public static bool operator >(Instant left, Instant right) => left._ticks > right._ticks;
    
    [M(A)] public static bool operator >=(Instant left, DateTimeOffset right) => left._ticks >= right.UtcTicks;
    [M(A)] public static bool operator <=(Instant left, DateTimeOffset right) => left._ticks <= right.UtcTicks;
    [M(A)] public static bool operator <(Instant left, DateTimeOffset right) => left._ticks < right.UtcTicks;
    [M(A)] public static bool operator >(Instant left, DateTimeOffset right) => left._ticks > right.UtcTicks;
    [M(A)] public static bool operator ==(Instant left, DateTimeOffset right) => left._ticks == right.UtcTicks;
    [M(A)] public static bool operator !=(Instant left, DateTimeOffset right) => left._ticks != right.UtcTicks;

    [M(A)] public static bool operator >=(DateTimeOffset left, Instant right) => left.UtcTicks >= right._ticks;
    [M(A)] public static bool operator <=(DateTimeOffset left, Instant right) => left.UtcTicks <= right._ticks;
    [M(A)] public static bool operator <(DateTimeOffset left, Instant right) => left.UtcTicks < right._ticks;
    [M(A)] public static bool operator >(DateTimeOffset left, Instant right) => left.UtcTicks > right._ticks;
    [M(A)] public static bool operator ==(DateTimeOffset left, Instant right) => left.UtcTicks == right._ticks;
    [M(A)] public static bool operator !=(DateTimeOffset left, Instant right) => left.UtcTicks != right._ticks; 
    
    [M(A)] public static bool operator >=(Instant left, DateTime right) => left._ticks >= right.Ticks;
    [M(A)] public static bool operator <=(Instant left, DateTime right) => left._ticks <= right.Ticks;
    [M(A)] public static bool operator <(Instant left, DateTime right) => left._ticks < right.Ticks;
    [M(A)] public static bool operator >(Instant left, DateTime right) => left._ticks > right.Ticks;
    [M(A)] public static bool operator ==(Instant left, DateTime right) => left._ticks == right.Ticks;
    [M(A)] public static bool operator !=(Instant left, DateTime right) => left._ticks != right.Ticks;
    
    [M(A)] public static bool operator >=(DateTime left, Instant right) => left.Ticks >= right._ticks;
    [M(A)] public static bool operator <=(DateTime left, Instant right) => left.Ticks <= right._ticks;
    [M(A)] public static bool operator <(DateTime left, Instant right) => left.Ticks < right._ticks;
    [M(A)] public static bool operator >(DateTime left, Instant right) => left.Ticks > right._ticks;
    [M(A)] public static bool operator ==(DateTime left, Instant right) => left.Ticks == right._ticks;
    [M(A)] public static bool operator !=(DateTime left, Instant right) => left.Ticks != right._ticks;
    
    [M(A)] public TimeSpan ElapsedUntil(Instant end) => end - this;

    [M(A)] public static Instant FromTicks(long ticks) => new(ticks);

    // Not stopwatch.ElapsedTicks, as those are HPT ticks not DateTime ticks
    [M(A)] public static Instant FromStopwatch(Stopwatch stopwatch) => new(stopwatch.Elapsed.Ticks);

    [M(A)] public static TimeSpan operator -(Instant a, Instant b) => TimeSpan.FromTicks(a._ticks - b._ticks);
    
    // fix overload resolution collisions
    [M(A)] public static TimeSpan operator -(Instant a, DateTimeOffset b) => TimeSpan.FromTicks(a._ticks - b.UtcTicks);
    [M(A)] public static TimeSpan operator -(DateTimeOffset a, Instant b) => TimeSpan.FromTicks(a.UtcTicks - b._ticks);
    [M(A)] public static TimeSpan operator -(Instant a, DateTime b) => TimeSpan.FromTicks(a._ticks - b.Ticks);
    [M(A)] public static TimeSpan operator -(DateTime a, Instant b) => TimeSpan.FromTicks(a.Ticks - b._ticks);

    [M(A)] public static Instant operator +(Instant a, TimeSpan b) => new(a._ticks + b.Ticks);
    [M(A)] public static Instant operator -(Instant a, TimeSpan b) => new(a._ticks - b.Ticks);

    [M(A)] public static Instant Older(Instant a, Instant b) => new(Math.Min(a._ticks, b._ticks));
    [M(A)] public static Instant Newer(Instant a, Instant b) => new(Math.Max(a._ticks, b._ticks));
}