using Microsoft.Extensions.Caching.Memory;

namespace Maxine.Extensions.Caching;

public class MemoryCacheEntryOptions<TKey, TValue> : MemoryCacheEntryOptions where TKey : notnull
{
    /// <summary>
    /// Gets or sets the callbacks will be fired after the cache entry is evicted from the cache.
    /// </summary>
    public new IList<MemoryCache<TKey, TValue>.CacheEntry.PostEvictionCallbackRegistration2> PostEvictionCallbacks { get; }
        = new List<MemoryCache<TKey, TValue>.CacheEntry.PostEvictionCallbackRegistration2>();

    /// <summary>
    /// The given callback will be fired after the cache entry is evicted from the cache.
    /// </summary>
    /// <param name="callback">The callback to register for calling after an entry is evicted.</param>
    /// <param name="state">The state to pass to the callback.</param>
    /// <returns>The <see cref="MemoryCacheEntryOptions"/> so that additional calls can be chained.</returns>
    public MemoryCacheEntryOptions<TKey, TValue> RegisterPostEvictionCallback(
        MemoryCache<TKey, TValue>.CacheEntry.PostEvictionDelegate callback,
        object? state = null)
    {
        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        PostEvictionCallbacks.Add(new MemoryCache<TKey, TValue>.CacheEntry.PostEvictionCallbackRegistration2()
        {
            EvictionCallback = callback,
            State = state
        });

        return this;
    }
}