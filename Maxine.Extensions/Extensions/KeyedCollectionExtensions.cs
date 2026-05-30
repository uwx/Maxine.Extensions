using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Maxine.Extensions.Collections;

namespace Maxine.Extensions;

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
            collection.RemoveAt(index);
            collection.Insert(index, value);
            return oldValue;
        }

        public bool TryAdd(TItem value)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (collection.Contains(value))
            {
                return false;
            }

            collection.Add(value);
            return true;
        }
    }
}