using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Maxine.Extensions.Collections;

namespace Maxine.Extensions;

file static class Accessor<TKey, TItem> where TKey : notnull
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetKeyForItem")]
    public static extern TKey GetKeyForItem(KeyedCollection<TKey, TItem> collection, TItem item);
}

public static class KeyedCollectionExtensions
{
    extension<TKey, TItem>(KeyedCollection<TKey, TItem> collection) where TKey : notnull
    {
        public TItem? GetValueOrDefault(TKey key) =>
            collection.GetValueOrDefault(key, default);

        [return: NotNullIfNotNull(nameof(defaultValue))]
        public TItem? GetValueOrDefault(TKey key, TItem? defaultValue)
        {
            ArgumentNullException.ThrowIfNull(collection);

            return collection.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public void AddRange(IEnumerable<TItem> values)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(values);

            foreach (var value in values)
            {
                collection.Add(value);
            }
        }

        public void RemoveAll(IEnumerable<TKey> keys)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(keys);

            foreach (var key in keys)
            {
                collection.Remove(key);
            }
        }

        public void RemoveAll(Predicate<TItem> keys)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(keys);
        
            List<TItem>? valuesToRemove = null;
            foreach (var value in collection)
            {
                if (keys(value))
                {
                    valuesToRemove ??= [];
                    valuesToRemove.Add(value);
                }
            }

            if (valuesToRemove != null)
            {
                foreach (var value in valuesToRemove)
                {
                    collection.Remove(value);
                }
            }
        }
        
        public TItem Swap(int index, TItem value)
        {
            ArgumentNullException.ThrowIfNull(collection);
            
            var oldValue = collection[index];
            // Remove by key first to handle any pre-existing duplicates safely.
            // RemoveAt+Insert can throw "key already exists" if the key is still
            // present in the dictionary (e.g. from a duplicate item at another index).
            var key = Accessor<TKey, TItem>.GetKeyForItem(collection, value);
            collection.Remove(key);
            collection.Insert(index, value);
            return oldValue;
        }
        
        public bool TryAdd(TItem value)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (collection.Contains(Accessor<TKey, TItem>.GetKeyForItem(collection, value)))
            {
                return false;
            }

            collection.Add(value);
            return true;
        }
    }
}