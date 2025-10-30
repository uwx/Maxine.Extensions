using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Poki.Shared;

/// <summary>
/// Convenience class for dealing with randomness.
/// </summary>
/// <remarks>
/// Partially based off https://codeblog.jonskeet.uk/2009/11/04/revisiting-randomness/
/// </remarks>
public static class ThreadLocalRandom
{
    /// <summary>
    /// Returns an instance of Random which can be used freely
    /// within the current thread.
    /// </summary>
    public static Random Instance => Random.Shared;

    /// <inheritdoc cref="Random.Next()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next()
    {
        return Random.Shared.Next();
    }

    /// <inheritdoc cref="Random.Next(int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next(int maxValue)
    {
        return Random.Shared.Next(maxValue);
    }

    /// <inheritdoc cref="Random.Next(int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next(int minValue, int maxValue)
    {
        return Random.Shared.Next(minValue, maxValue);
    }

    // Largest fraction smaller than 1, 17-digit precision
    // see https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#RFormatString
    private const double SlightlyLessThan1 = 0.99999999999999989;

    // https://stackoverflow.com/a/16111036
    /// <summary>
    /// Returns a random number in a triangle distribution.
    /// </summary>
    /// <param name="mode">The mode (most frequent value).</param>
    /// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
    public static double NextTriangle(double mode)
    {
        var rand = NextDouble();

        if (rand <= mode)
            return mode * Math.Sqrt(rand);
        else
            return SlightlyLessThan1 - ((SlightlyLessThan1 - mode) * Math.Sqrt(SlightlyLessThan1 - rand));
    }

    // mode still 0-1
    public static int NextTriangle(double min, double max, double mode)
    {
        // https://stackoverflow.com/a/1527820
        return (int)Math.Floor(NextTriangle(mode) * (max - min) + min);
    }

    /// <summary>Returns a random integer that is within a specified range.</summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">
    /// The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or
    /// equal to <paramref name="minValue"/>.
    /// </param>
    /// <returns>
    /// A 8-bit signed integer greater than or equal to <paramref name="minValue"/> and less than
    /// <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/> but not
    /// <paramref name="maxValue"/>. If minValue equals <paramref name="maxValue"/>, <paramref name="minValue"/> is
    /// returned.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte NextByte(byte minValue, byte maxValue)
    {
        return (byte)Next(minValue, maxValue);
    }

    /// <inheritdoc cref="Random.Next()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long NextInt64()
    {
        return Random.Shared.NextInt64();
    }

    /// <inheritdoc cref="Random.NextInt64(long)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long NextInt64(long maxValue)
    {
        return Random.Shared.NextInt64(maxValue);
    }

    /// <inheritdoc cref="Random.NextInt64(long, long)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long NextInt64(long minValue, long maxValue)
    {
        return Random.Shared.NextInt64(minValue, maxValue);
    }

    /// <inheritdoc cref="Random.NextSingle()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float NextSingle()
    {
        return Random.Shared.NextSingle();
    }

    /// <inheritdoc cref="Random.NextDouble()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NextDouble()
    {
        return Random.Shared.NextDouble();
    }

    /// <summary>
    /// Returns a random floating-point number within the specified range.
    /// </summary>
    /// <param name="minValue">The minimum value to be returned, inclusive</param>
    /// <param name="maxValue">The maximum value to be returned, exclusive</param>
    /// <returns>The resulting random double</returns>
    public static double NextDouble(double minValue, double maxValue)
    {
        return NextDouble() * (maxValue - minValue) + minValue;
    }

    /// <inheritdoc cref="Random.NextBytes(byte[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NextBytes(byte[] buffer)
    {
        Random.Shared.NextBytes(buffer);
    }

    /// <inheritdoc cref="Random.NextBytes(Span{byte})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NextBytes(Span<byte> buffer)
    {
        Random.Shared.NextBytes(buffer);
    }

    /// <summary>
    /// Returns a random boolean, with about a 50/50 chance of being true or false.
    /// </summary>
    /// <returns>The resulting random boolean</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NextBoolean()
    {
        return NextDouble() < 0.5;
    }

    /// <summary>
    /// Returns a random boolean, with its chance of being true defined by <paramref name="odds"/>, scaled between 0 and
    /// 1; for instance, <paramref name="odds"/> of 0.25 represents a 25% chance. 
    /// </summary>
    /// <param name="odds">The decimal chance of the resulting value being true, with 0 being 0% and 1 being 100%</param>
    /// <returns>The resulting random boolean</returns>
    /// <example>
    /// <code>ThreadLocalRandom.NextBoolean(1/4d); // 1 in 4 chance of being true, or 25%</code>
    /// <code>ThreadLocalRandom.NextBoolean(0.25); // likewise, represented as a decimal</code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NextBoolean(double odds)
    {
        return NextDouble() < odds;
    }

    public static T RandomEntry<T>(IReadOnlyList<T> list)
    {
        return list[Next(list.Count)];
    }
    
    // https://stackoverflow.com/a/11930875
    public static T RandomWeightedEntry<T>(this IReadOnlyList<T> list, [RequireStaticDelegate] Func<T, double> weightSelector)
    {
        var totalWeight = list.Sum(weightSelector);
        var itemWeightIndex = NextDouble() * totalWeight;
        var currentWeightIndex = 0.0;

        foreach(var item in list)
        {
            var weight = weightSelector(item);
            
            currentWeightIndex += weight;
            
            // If we've hit or passed the weight we are after for this item then it's the one we want....
            if(currentWeightIndex >= itemWeightIndex)
                return item;
        }

        throw new InvalidOperationException("Invalid result");
    }
}