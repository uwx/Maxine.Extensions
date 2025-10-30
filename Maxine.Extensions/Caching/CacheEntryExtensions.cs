using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Maxine.Extensions.Caching;

[PublicAPI]
public static class CacheExtensions
{
    public static TValue? Get<TKey, TValue>(this MemoryCache<TKey, TValue> cache, TKey key) where TKey : notnull
    {
        cache.TryGetValue(key, out var value);
        return value;
    }

    public static TValue Set<TKey, TValue>(this MemoryCache<TKey, TValue> cache, TKey key, TValue value) where TKey : notnull
    {
        var entry = cache.CreateEntry(key);
        entry.Value = value;
        entry.Dispose();

        return value;
    }

    public static TValue Set<TKey, TValue>(this MemoryCache<TKey, TValue> cache, TKey key, TValue value, DateTimeOffset absoluteExpiration) where TKey : notnull
    {
        var entry = cache.CreateEntry(key);
        entry.AbsoluteExpiration = absoluteExpiration;
        entry.Value = value;
        entry.Dispose();

        return value;
    }

    public static TValue Set<TKey, TValue>(this MemoryCache<TKey, TValue> cache, TKey key, TValue value, TimeSpan absoluteExpirationRelativeToNow) where TKey : notnull
    {
        var entry = cache.CreateEntry(key);
        entry.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        entry.Value = value;
        entry.Dispose();

        return value;
    }

    public static TValue Set<TKey, TValue>(this MemoryCache<TKey, TValue> cache, TKey key, TValue value, IChangeToken expirationToken) where TKey : notnull
    {
        var entry = cache.CreateEntry(key);
        entry.AddExpirationToken(expirationToken);
        entry.Value = value;
        entry.Dispose();

        return value;
    }
    
    public static TValue Set<TKey, TValue>(this MemoryCache<TKey, TValue> cache, TKey key, TValue value, PostEvictionDelegate postEvictionCallback) where TKey : notnull
    {
        var entry = cache.CreateEntry(key);
        entry.RegisterPostEvictionCallback(postEvictionCallback);
        entry.Value = value;
        entry.Dispose();

        return value;
    }
    
    public static TValue Set<TKey, TValue>(this MemoryCache<TKey, TValue> cache, TKey key, TValue value, MemoryCache<TKey, TValue>.CacheEntry.PostEvictionDelegate postEvictionCallback) where TKey : notnull
    {
        var entry = cache.CreateEntry(key);
        entry.RegisterPostEvictionCallback(postEvictionCallback);
        entry.Value = value;
        entry.Dispose();

        return value;
    }
    
    /// <summary>
    /// The given callback will be fired after the cache entry is evicted from the cache.
    /// </summary>
    /// <param name="entry">The <see cref="ICacheEntry"/>.</param>
    /// <param name="callback">The callback to run after the entry is evicted.</param>
    /// <param name="state">The state to pass to the post-eviction callback.</param>
    /// <returns>The <see cref="ICacheEntry"/> for chaining.</returns>
    public static MemoryCache<TKey, TValue>.CacheEntry RegisterPostEvictionCallback<TKey, TValue>(
        this MemoryCache<TKey, TValue>.CacheEntry entry,
        MemoryCache<TKey, TValue>.CacheEntry.PostEvictionDelegate callback,
        object? state = null) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(callback);

        entry.PostEvictionCallbacks.Add(new MemoryCache<TKey, TValue>.CacheEntry.PostEvictionCallbackRegistration2
        {
            EvictionCallback = callback,
            State = state
        });
        return entry;
    }

    
    public static TValue Set<TKey, TValue>(this MemoryCache<TKey, TValue> cache, TKey key, TValue value, MemoryCacheEntryOptions? options) where TKey : notnull
    {
        using (var entry = cache.CreateEntry(key))
        {
            if (options != null)
            {
                entry.SetOptions(options);
            }

            entry.Value = value;
        }

        return value;
    }
    
    /// <summary>
    /// Applies the values of an existing <see cref="MemoryCacheEntryOptions"/> to the entry.
    /// </summary>
    /// <param name="entry">The <see cref="ICacheEntry"/>.</param>
    /// <param name="options">Set the values of these options on the <paramref name="entry"/>.</param>
    /// <returns>The <see cref="ICacheEntry"/> for chaining.</returns>
    public static ICacheEntry SetOptions<TKey, TValue>(this MemoryCache<TKey, TValue>.CacheEntry entry, MemoryCacheEntryOptions<TKey, TValue> options) where TKey : notnull
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        entry.AbsoluteExpiration = options.AbsoluteExpiration;
        entry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
        entry.SlidingExpiration = options.SlidingExpiration;
        entry.Priority = options.Priority;
        entry.Size = options.Size;

        foreach (var expirationToken in options.ExpirationTokens)
        {
            entry.AddExpirationToken(expirationToken);
        }

        foreach (var postEvictionCallback in options.PostEvictionCallbacks)
        {
            entry.RegisterPostEvictionCallback(postEvictionCallback.EvictionCallback, postEvictionCallback.State);
        }

        foreach (var postEvictionCallback in ((MemoryCacheEntryOptions)options).PostEvictionCallbacks)
        {
            entry.RegisterPostEvictionCallback(postEvictionCallback.EvictionCallback, postEvictionCallback.State);
        }

        return entry;
    }

    public static TValue GetOrCreate<TKey, TValue>(this MemoryCache<TKey, TValue> cache, TKey key, Func<MemoryCache<TKey, TValue>.CacheEntry, TValue> factory) where TKey : notnull
    {
        if (!cache.TryGetValue(key, out var result))
        {
            var entry = cache.CreateEntry(key);
            result = factory(entry);
            entry.SetValue(result);
            // need to manually call dispose instead of having a using
            // in case the factory passed in throws, in which case we
            // do not want to add the entry to the cache
            entry.Dispose();
        }

        return result;
    }

    public static async ValueTask<TValue> GetOrCreateAsync<TKey, TValue>(this MemoryCache<TKey, TValue> cache, TKey key, Func<MemoryCache<TKey, TValue>.CacheEntry, ValueTask<TValue>> factory) where TKey : notnull
    {
        if (!cache.TryGetValue(key, out var result))
        {
            var entry = cache.CreateEntry(key);
            result = await factory(entry);
            entry.SetValue(result);
            // need to manually call dispose instead of having a using
            // in case the factory passed in throws, in which case we
            // do not want to add the entry to the cache
            entry.Dispose();
        }

        return result;
    }

    public static async Task<TValue> GetOrCreateAsync<TKey, TValue>(this MemoryCache<TKey, TValue> cache, TKey key, Func<ICacheEntry, Task<TValue>> factory) where TKey : notnull
    {
        if (!cache.TryGetValue(key, out var result))
        {
            var entry = cache.CreateEntry(key);
            result = await factory(entry);
            entry.SetValue(result);
            // need to manually call dispose instead of having a using
            // in case the factory passed in throws, in which case we
            // do not want to add the entry to the cache
            entry.Dispose();
        }

        return result;
    }
    
    /// <summary>
    /// Sets the value of the cache entry.
    /// </summary>
    /// <param name="entry">The <see cref="ICacheEntry"/>.</param>
    /// <param name="value">The value to set on the <paramref name="entry"/>.</param>
    /// <returns>The <see cref="ICacheEntry"/> for chaining.</returns>
    public static MemoryCache<TKey, TValue>.CacheEntry SetValue<TKey, TValue>(
        this MemoryCache<TKey, TValue>.CacheEntry entry,
        TValue value) where TKey : notnull
    {
        entry.Value = value;
        return entry;
    }

}