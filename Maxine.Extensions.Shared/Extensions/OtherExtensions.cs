namespace Maxine.Extensions.Shared;

public static class OtherExtensions
{
    public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TSource, TKey, TValue>(
        this IEnumerable<TSource> enumerable,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> elementSelector
    ) where TKey : notnull
    {
        var dict = new OrderedDictionary<TKey, TValue>();

        foreach (var source in enumerable)
        {
            dict[keySelector(source)] = elementSelector(source);
        }

        return dict;
    }

    public static bool EqualsIgnoreCase(this string? a, string? b)
    {
        if (a == null) return b == null;
        return a.Equals(b, StringComparison.OrdinalIgnoreCase);
    }
}