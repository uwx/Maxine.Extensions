namespace Maxine.Extensions;

public static class GroupingExtensions
{
    public static void Deconstruct<TKey, TValue>(
        this IGrouping<TKey, TValue> self, out TKey key, out IEnumerable<TValue> values
    )
    {
        key = self.Key;
        values = self;
    }
}