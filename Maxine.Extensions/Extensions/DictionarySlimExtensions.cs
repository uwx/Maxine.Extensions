using Microsoft.Collections.Extensions;

namespace Maxine.Extensions;

public static class DictionarySlimExtensions
{
    extension<TKey, TElement>(DictionarySlim<TKey, TElement> dictionary)
        where TKey : IEquatable<TKey>
    {
        public TElement? GetValueOrDefault(TKey key)
        {
            return dictionary.TryGetValue(key, out var value) ? value : default!;
        }
    }
}
