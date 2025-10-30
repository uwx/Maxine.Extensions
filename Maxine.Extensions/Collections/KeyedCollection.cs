using System.Collections.ObjectModel;

namespace Maxine.Extensions.Collections;

public class KeyedCollection
{
    public sealed class LambdaKeyedCollection<TKey, TItem>(Func<TItem, TKey> getKey) : KeyedCollection<TKey, TItem> where TKey : notnull
    {
        protected override TKey GetKeyForItem(TItem item) => getKey(item);
    }
    
    public static LambdaKeyedCollection<TKey, TItem> From<TKey, TItem>(Func<TItem, TKey> getKey) where TKey : notnull
    {
        return new LambdaKeyedCollection<TKey, TItem>(getKey);
    }
}