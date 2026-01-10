using Microsoft.Collections.Extensions;

namespace Maxine.Extensions;

public static class DictionarySlimExtensions
{
    public static TElement? GetValueOrDefault<TKey, TElement>(this DictionarySlim<TKey, TElement> dictionary, TKey key)
        where TKey : IEquatable<TKey>
    {
        return dictionary.TryGetValue(key, out var value) ? value : default!;
    }
}