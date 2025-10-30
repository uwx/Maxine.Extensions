using System.Numerics;

namespace Maxine.Extensions;

public static class LinqMathExtensions
{
    public static T Sum<T>(this IEnumerable<T> source) where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
    {
        return source.Aggregate(T.AdditiveIdentity, static (a, b) => a + b);
    }

    public static IEnumerable<T> Range<T, TStep>(T start, T end, TStep step) where T : IAdditionOperators<T, TStep, T>, IComparisonOperators<T, T, bool> where TStep : notnull
    {
        for (var i = start; i < end; i += step)
        {
            yield return i;
        }
    }

    public static IEnumerable<T> Range<T>(T start, T end, T step) where T : IAdditionOperators<T, T, T>, IComparisonOperators<T, T, bool>
    {
        for (var i = start; i < end; i += step)
        {
            yield return i;
        }
    }

    public static T Average<T>(this IEnumerable<T> source) where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IDivisionOperators<T, int, T>
    {
        var (val, count) = source.Aggregate((val: T.AdditiveIdentity, count: 0), static (acc, b) => (acc.val + b, acc.count+1));
        return val / count;
    }
}

