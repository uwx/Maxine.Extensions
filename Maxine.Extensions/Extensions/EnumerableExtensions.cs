using System.Runtime.InteropServices;

namespace Maxine.Extensions;

public static class EnumerableExtensions
{
    public static Dictionary<TKey, IReadOnlyList<TValue>> GroupByToDictionary<TKey, TValue>(
        this IEnumerable<TValue> enumerable,
        Func<TValue, TKey> selector,
        IEqualityComparer<TKey>? comparer = null
    ) where TKey : notnull
    {
        var dict = new Dictionary<TKey, IReadOnlyList<TValue>>(comparer);
        
        foreach (var value in enumerable)
        {
            var key = selector(value);
            ref var v = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);
            if (exists)
            {
                ((List<TValue>)v!).Add(value);
            }
            else
            {
                v = new List<TValue> { value };
            }
        }

        foreach (var (_, value) in dict)
        {
            ((List<TValue>)value).TrimExcess();
        }

        return dict;
    }
    public static Dictionary<TKey, IReadOnlyList<TResult>> GroupByToDictionary<TKey, TValue, TResult>(
        this IEnumerable<TValue> enumerable,
        Func<TValue, TKey> selector,
        Func<TValue, TResult> resultSelector,
        IEqualityComparer<TKey>? comparer = null
    ) where TKey : notnull
    {
        var dict = new Dictionary<TKey, IReadOnlyList<TResult>>(comparer);
        
        foreach (var value in enumerable)
        {
            var key = selector(value);
            ref var v = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);
            if (exists)
            {
                ((List<TResult>)v!).Add(resultSelector(value));
            }
            else
            {
                v = new List<TResult> { resultSelector(value) };
            }
        }

        foreach (var (_, value) in dict)
        {
            ((List<TResult>)value).TrimExcess();
        }

        return dict;
    }

}