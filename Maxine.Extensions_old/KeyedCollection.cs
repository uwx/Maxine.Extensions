using System.Collections.ObjectModel;

namespace Maxine.TU.Sims4Uploader;

public class KeyedCollection
{
    private class LambdaKeyedCollection<TKey, TItem>(Func<TItem, TKey> getKey) : KeyedCollection<TKey, TItem> where TKey : notnull
    {
        protected override TKey GetKeyForItem(TItem item) => getKey(item);
    }
    
    public static KeyedCollection<TKey, TItem> From<TKey, TItem>(Func<TItem, TKey> getKey) where TKey : notnull
    {
        return new LambdaKeyedCollection<TKey, TItem>(getKey);
    }
}