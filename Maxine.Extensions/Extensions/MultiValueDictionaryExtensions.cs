using Microsoft.Collections.Extensions;

namespace Maxine.Extensions;

public static class MultiValueDictionaryExtensions
{
    public static IEnumerable<TValue> TryGetValues<TKey, TValue>(this MultiValueDictionary<TKey, TValue> dict, TKey key, out bool hasValues)
    {
        // ReSharper disable once AssignmentInConditionalExpression
        if (hasValues = dict.TryGetValue(key, out var values))
        {
            return values!;
        }

        return [];
    }
}