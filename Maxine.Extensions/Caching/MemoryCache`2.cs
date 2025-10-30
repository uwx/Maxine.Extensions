using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Maxine.Extensions.Collections;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Maxine.Extensions.Caching;

[PublicAPI]
public class MemoryCache<TKey, TValue> : IMemoryCache where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, CacheEntry> _entries;
    private long _cacheSize;
    private bool _disposed;
    private ILogger _logger;

    // We store the delegates locally to prevent allocations
    // every time a new CacheEntry is created.
    private readonly Action<CacheEntry> _setEntry;
    private readonly Action<CacheEntry> _entryExpirationNotification;

    private readonly MemoryCacheOptions _options;
    private DateTimeOffset _lastExpirationScan;

    /// <summary>
    /// Creates a new <see cref="MemoryCache"/> instance.
    /// </summary>
    /// <param name="optionsAccessor">The options of the cache.</param>
    public MemoryCache(IOptions<MemoryCacheOptions> optionsAccessor)
        : this(optionsAccessor, NullLoggerFactory.Instance)
    {
    }

    /// <summary>
    /// Creates a new <see cref="MemoryCache"/> instance.
    /// </summary>
    /// <param name="options">The options of the cache.</param>
    public MemoryCache(MemoryCacheOptions options)
        : this(new OptionsWrapper<MemoryCacheOptions>(options), NullLoggerFactory.Instance)
    {
    }

    /// <summary>
    /// Creates a new <see cref="MemoryCache"/> instance.
    /// </summary>
    /// <param name="optionsAccessor">The options of the cache.</param>
    /// <param name="loggerFactory">The factory used to create loggers.</param>
    public MemoryCache(IOptions<MemoryCacheOptions> optionsAccessor, ILoggerFactory loggerFactory)
    {
        if (optionsAccessor == null)
        {
            throw new ArgumentNullException(nameof(optionsAccessor));
        }

        if (loggerFactory == null)
        {
            throw new ArgumentNullException(nameof(loggerFactory));
        }

        _options = optionsAccessor.Value;
        _logger = loggerFactory.CreateLogger<MemoryCache>();

        _entries = new ConcurrentDictionary<TKey, CacheEntry>();
        _setEntry = SetEntry;
        _entryExpirationNotification = EntryExpired;

        if (_options.Clock == null)
        {
            _options.Clock = new SystemClock();
        }

        _lastExpirationScan = _options.Clock.UtcNow;
    }

    /// <summary>
    /// Cleans up the background collection events.
    /// </summary>
    ~MemoryCache()
    {
        Dispose(false);
    }

    /// <summary>
    /// Gets the count of the current entries for diagnostic purposes.
    /// </summary>
    public int Count => _entries.Count;

    // internal for testing
    internal long Size => Interlocked.Read(ref _cacheSize);

    private ICollection<KeyValuePair<TKey, CacheEntry>> EntriesCollection => _entries;

    /// <inheritdoc cref="IMemoryCache.CreateEntry" />
    public CacheEntry CreateEntry(TKey key)
    {
        CheckDisposed();
        
        ArgumentNullException.ThrowIfNull(key);

        return new CacheEntry(
            key,
            this
        );
    }
    
    bool IMemoryCache.TryGetValue(object key, [UnscopedRef] out object value)
    {
        var result = TryGetValue((TKey)key, out var value1);
        value = value1;
        return result;
    }

    ICacheEntry IMemoryCache.CreateEntry(object key) => CreateEntry((TKey)key);
    void IMemoryCache.Remove(object key) => Remove((TKey)key);

    private void SetEntry(CacheEntry entry)
    {
        if (_disposed)
        {
            // No-op instead of throwing since this is called during CacheEntry.Dispose
            return;
        }

        if (_options.SizeLimit.HasValue && !entry.Size.HasValue)
        {
            throw new InvalidOperationException(
                $"Cache entry must specify a value for {nameof(entry.Size)} when {nameof(_options.SizeLimit)} is set.");
        }

        var utcNow = _options.Clock.UtcNow;

        DateTimeOffset? absoluteExpiration = null;
        if (entry.AbsoluteExpirationRelativeToNow.HasValue)
        {
            absoluteExpiration = utcNow + entry.AbsoluteExpirationRelativeToNow;
        }
        else if (entry.AbsoluteExpiration.HasValue)
        {
            absoluteExpiration = entry.AbsoluteExpiration;
        }

        // Applying the option's absolute expiration only if it's not already smaller.
        // This can be the case if a dependent cache entry has a smaller value, and
        // it was set by cascading it to its parent.
        if (absoluteExpiration.HasValue)
        {
            if (!entry.AbsoluteExpiration.HasValue || absoluteExpiration.Value < entry.AbsoluteExpiration.Value)
            {
                entry.AbsoluteExpiration = absoluteExpiration;
            }
        }

        // Initialize the last access timestamp at the time the entry is added
        entry.LastAccessed = utcNow;

        if (_entries.TryGetValue(entry.Key, out var priorEntry))
        {
            priorEntry.SetExpired(EvictionReason.Replaced);
        }

        var exceedsCapacity = UpdateCacheSizeExceedsCapacity(entry);

        if (!entry.CheckExpired(utcNow) && !exceedsCapacity)
        {
            bool entryAdded;

            if (priorEntry == null)
            {
                // Try to add the new entry if no previous entries exist.
                entryAdded = _entries.TryAdd(entry.Key, entry);
            }
            else
            {
                // Try to update with the new entry if a previous entries exist.
                entryAdded = _entries.TryUpdate(entry.Key, entry, priorEntry);

                if (entryAdded)
                {
                    if (_options.SizeLimit.HasValue)
                    {
                        // The prior entry was removed, decrease the by the prior entry's size
                        Interlocked.Add(ref _cacheSize, -priorEntry.Size!.Value);
                    }
                }
                else
                {
                    // The update will fail if the previous entry was removed after retrival.
                    // Adding the new entry will succeed only if no entry has been added since.
                    // This guarantees removing an old entry does not prevent adding a new entry.
                    entryAdded = _entries.TryAdd(entry.Key, entry);
                }
            }

            if (entryAdded)
            {
                entry.AttachTokens();
            }
            else
            {
                if (_options.SizeLimit.HasValue)
                {
                    // Entry could not be added, reset cache size
                    Interlocked.Add(ref _cacheSize, -entry.Size!.Value);
                }

                entry.SetExpired(EvictionReason.Replaced);
                entry.InvokeEvictionCallbacks();
            }

            priorEntry?.InvokeEvictionCallbacks();
        }
        else
        {
            if (exceedsCapacity)
            {
                // The entry was not added due to overcapacity
                entry.SetExpired(EvictionReason.Capacity);

                TriggerOvercapacityCompaction();
            }

            entry.InvokeEvictionCallbacks();
            if (priorEntry != null)
            {
                RemoveEntry(priorEntry);
            }
        }

        StartScanForExpiredItems();
    }

    /// <inheritdoc cref="IMemoryCache.TryGetValue" />
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue result)
    {
        ArgumentNullException.ThrowIfNull(key);

        CheckDisposed();

        result = default;
        var utcNow = _options.Clock.UtcNow;
        var found = false;

        if (_entries.TryGetValue(key, out var entry))
        {
            // Check if expired due to expiration tokens, timers, etc. and if so, remove it.
            // Allow a stale Replaced value to be returned due to concurrent calls to SetExpired during SetEntry.
            if (entry.CheckExpired(utcNow) && entry.EvictionReason != EvictionReason.Replaced)
            {
                // TODO: For efficiency queue this up for batch removal
                RemoveEntry(entry);
            }
            else
            {
                found = true;
                entry.LastAccessed = utcNow;
                result = entry.Value;

                // When this entry is retrieved in the scope of creating another entry,
                // that entry needs a copy of these expiration tokens.
                entry.PropagateOptions(CacheEntryHelper<TKey, TValue>.Current);
            }
        }

        StartScanForExpiredItems();

        return found;
    }

    /// <inheritdoc cref="IMemoryCache.Remove" />
    public void Remove(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        CheckDisposed();
        if (_entries.TryRemove(key, out var entry))
        {
            if (_options.SizeLimit.HasValue)
            {
                Interlocked.Add(ref _cacheSize, -entry.Size!.Value);
            }

            entry.SetExpired(EvictionReason.Removed);
            entry.InvokeEvictionCallbacks();
        }

        StartScanForExpiredItems();
    }

    public void Clear()
    {
        CheckDisposed();
        
        long sizeDecrement = 0;

        foreach (var (key, entry) in _entries)
        {
            if (_entries.TryRemove(KeyValuePair.Create(key, entry)))
            {
                sizeDecrement += entry.Size ?? 0;

                entry.SetExpired(EvictionReason.Removed);
                entry.InvokeEvictionCallbacks();
            }
        }
            
        if (_options.SizeLimit.HasValue)
        {
            Interlocked.Add(ref _cacheSize, -sizeDecrement);
        }
    }

    private void RemoveEntry(CacheEntry entry)
    {
        if (_entries.TryRemove(KeyValuePair.Create(entry.Key, entry)))
        {
            if (_options.SizeLimit.HasValue)
            {
                Interlocked.Add(ref _cacheSize, -entry.Size!.Value);
            }

            entry.InvokeEvictionCallbacks();
        }
    }

    private void EntryExpired(CacheEntry entry)
    {
        // TODO: For efficiency consider processing these expirations in batches.
        RemoveEntry(entry);
        StartScanForExpiredItems();
    }

    // Called by multiple actions to see how long it's been since we last checked for expired items.
    // If sufficient time has elapsed then a scan is initiated on a background task.
    private void StartScanForExpiredItems()
    {
        var now = _options.Clock.UtcNow;
        if (_options.ExpirationScanFrequency < now - _lastExpirationScan)
        {
            _lastExpirationScan = now;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Task.Factory.StartNew(
                static state => ScanForExpiredItems((MemoryCache<TKey, TValue>)state),
                this,
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default
            );
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8604 // Possible null reference argument.
        }
    }

    private static void ScanForExpiredItems(MemoryCache<TKey, TValue> cache)
    {
        var now = cache._options.Clock.UtcNow;
        foreach (var (_, entry) in cache._entries)
        {
            if (entry.CheckExpired(now))
            {
                cache.RemoveEntry(entry);
            }
        }
    }

    private bool UpdateCacheSizeExceedsCapacity(CacheEntry entry)
    {
        if (!_options.SizeLimit.HasValue)
        {
            return false;
        }

        for (var i = 0; i < 100; i++)
        {
            var sizeRead = Interlocked.Read(ref _cacheSize);
            var newSize = sizeRead + entry.Size!.Value;

            if (newSize < 0 || newSize > _options.SizeLimit)
            {
                // Overflow occured, return true without updating the cache size
                return true;
            }

            if (sizeRead == Interlocked.CompareExchange(ref _cacheSize, newSize, sizeRead))
            {
                return false;
            }
        }

        return true;
    }

    private void TriggerOvercapacityCompaction()
    {
        _logger.LogDebug("Overcapacity compaction triggered");

        // Spawn background thread for compaction
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        ThreadPool.QueueUserWorkItem(static s => OvercapacityCompaction((MemoryCache<TKey, TValue>)s), this);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8604 // Possible null reference argument.
    }

    private static void OvercapacityCompaction(MemoryCache<TKey, TValue> cache)
    {
        var currentSize = Interlocked.Read(ref cache._cacheSize);

        cache._logger.LogDebug("Overcapacity compaction executing. Current size {currentSize}", currentSize);

        var lowWatermark = cache._options.SizeLimit * (1 - cache._options.CompactionPercentage);
        if (currentSize > lowWatermark)
        {
            cache.Compact(currentSize - (long)lowWatermark, static entry => entry.Size!.Value);
        }

        cache._logger.LogDebug("Overcapacity compaction executed. New size {cacheSize}", Interlocked.Read(ref cache._cacheSize));
    }

    /// Remove at least the given percentage (0.10 for 10%) of the total entries (or estimated memory?), according to the following policy:
    /// 1. Remove all expired items.
    /// 2. Bucket by CacheItemPriority.
    /// 3. Least recently used objects.
    /// ?. Items with the soonest absolute expiration.
    /// ?. Items with the soonest sliding expiration.
    /// ?. Larger objects - estimated by object graph size, inaccurate.
    public void Compact(double percentage)
    {
        var removalCountTarget = (int)(_entries.Count * percentage);
        Compact(removalCountTarget, _ => 1);
    }

    private void Compact(long removalSizeTarget, Func<CacheEntry, long> computeEntrySize)
    {
        var entriesToRemove = new List<CacheEntry>();
        var lowPriEntries = new List<CacheEntry>();
        var normalPriEntries = new List<CacheEntry>();
        var highPriEntries = new List<CacheEntry>();
        long removedSize = 0;

        // Sort items by expired & priority status
        var now = _options.Clock.UtcNow;
        foreach (var (_, entry) in _entries)
        {
            if (entry.CheckExpired(now))
            {
                entriesToRemove.Add(entry);
                removedSize += computeEntrySize(entry);
            }
            else
            {
                switch (entry.Priority)
                {
                    case CacheItemPriority.Low:
                        lowPriEntries.Add(entry);
                        break;
                    case CacheItemPriority.Normal:
                        normalPriEntries.Add(entry);
                        break;
                    case CacheItemPriority.High:
                        highPriEntries.Add(entry);
                        break;
                    case CacheItemPriority.NeverRemove:
                        break;
                    default:
                        throw new NotSupportedException("Not implemented: " + entry.Priority);
                }
            }
        }

        ExpirePriorityBucket(ref removedSize, removalSizeTarget, computeEntrySize, entriesToRemove, lowPriEntries);
        ExpirePriorityBucket(ref removedSize, removalSizeTarget, computeEntrySize, entriesToRemove, normalPriEntries);
        ExpirePriorityBucket(ref removedSize, removalSizeTarget, computeEntrySize, entriesToRemove, highPriEntries);

        foreach (var entry in entriesToRemove)
        {
            RemoveEntry(entry);
        }
    }

    /// Policy:
    /// 1. Least recently used objects.
    /// ?. Items with the soonest absolute expiration.
    /// ?. Items with the soonest sliding expiration.
    /// ?. Larger objects - estimated by object graph size, inaccurate.
    private void ExpirePriorityBucket(ref long removedSize, long removalSizeTarget,
        Func<CacheEntry, long> computeEntrySize, List<CacheEntry> entriesToRemove, List<CacheEntry> priorityEntries)
    {
        // Do we meet our quota by just removing expired entries?
        if (removalSizeTarget <= removedSize)
        {
            // No-op, we've met quota
            return;
        }

        // Expire enough entries to reach our goal
        // TODO: Refine policy

        // LRU
        foreach (var entry in priorityEntries.OrderBy(entry => entry.LastAccessed))
        {
            entry.SetExpired(EvictionReason.Capacity);
            entriesToRemove.Add(entry);
            removedSize += computeEntrySize(entry);

            if (removalSizeTarget <= removedSize)
            {
                break;
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            _disposed = true;
        }
    }

    private void CheckDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(typeof(MemoryCache).FullName);
        }
    }
    
    public sealed class CacheEntry : ICacheEntry
    {
        /// <summary>
        /// Signature of the callback which gets called when a cache entry expires.
        /// </summary>
        /// <param name="key">The key of the entry being evicted.</param>
        /// <param name="value">The value of the entry being evicted.</param>
        /// <param name="reason">The <see cref="T:Microsoft.Extensions.Caching.Memory.EvictionReason" />.</param>
        /// <param name="state">The information that was passed when registering the callback.</param>
        public delegate void PostEvictionDelegate(
            TKey key,
            TValue value,
            EvictionReason reason,
            object? state
        );

        public readonly record struct PostEvictionCallbackRegistration2(
            PostEvictionDelegate EvictionCallback,
            object? State
        );
        
        private bool _added;
        private bool _isExpired;
        private List<IDisposable>? _expirationTokenRegistrations;
        private List<PostEvictionCallbackRegistration2>? _postEvictionCallbacks;

        private List<IChangeToken>? _expirationTokens;
        private Instant? _absoluteExpiration;
        private TimeSpan? _absoluteExpirationRelativeToNow;
        private TimeSpan? _slidingExpiration;
        private long? _size;
        private readonly CacheEntryHelper<TKey, TValue>.ScopeLease _scope;

        private readonly object _lock = new();

        private readonly MemoryCache<TKey, TValue> _parentCollection;

        internal CacheEntry(TKey key, MemoryCache<TKey, TValue> parentCollection)
        {
            ArgumentNullException.ThrowIfNull(key);
            Key = key;
            _parentCollection = parentCollection;
            _scope = CacheEntryHelper<TKey, TValue>.EnterScope(this);
        }

        /// <summary>
        /// Gets or sets an absolute expiration date for the cache entry.
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration
        {
            get => _absoluteExpiration;
            set => _absoluteExpiration = value;
        }

        /// <summary>
        /// Gets or sets an absolute expiration time, relative to now.
        /// </summary>
        public TimeSpan? AbsoluteExpirationRelativeToNow
        {
            get => _absoluteExpirationRelativeToNow;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(AbsoluteExpirationRelativeToNow),
                        value,
                        "The relative expiration value must be positive."
                    );
                }

                _absoluteExpirationRelativeToNow = value;
            }
        }

        /// <summary>
        /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before it will be removed.
        /// This will not extend the entry lifetime beyond the absolute expiration (if set).
        /// </summary>
        public TimeSpan? SlidingExpiration
        {
            get => _slidingExpiration;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(SlidingExpiration),
                        value,
                        "The sliding expiration value must be positive."
                    );
                }

                _slidingExpiration = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IChangeToken"/> instances which cause the cache entry to expire.
        /// </summary>
        public IList<IChangeToken> ExpirationTokens => _expirationTokens ??= [];

        /// <summary>
        /// Gets or sets the callbacks will be fired after the cache entry is evicted from the cache.
        /// </summary>
        public IList<PostEvictionCallbackRegistration2> PostEvictionCallbacks => _postEvictionCallbacks ??= [];

        IList<PostEvictionCallbackRegistration> ICacheEntry.PostEvictionCallbacks => FakeAppendOnlyList<PostEvictionCallbackRegistration>.WrapWithConversionTo(
            PostEvictionCallbacks,
            static reg => new PostEvictionCallbackRegistration2(static (key, value, reason, state) =>
            {
                var reg = (PostEvictionCallbackRegistration)state!;
                reg.EvictionCallback(key, value, reason, state);
            }, reg)
        );

        /// <summary>
        /// Gets or sets the priority for keeping the cache entry in the cache during a
        /// memory pressure triggered cleanup. The default is <see cref="CacheItemPriority.Normal"/>.
        /// </summary>
        public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;

        /// <summary>
        /// Gets or sets the size of the cache entry value.
        /// </summary>
        public long? Size
        {
            get => _size;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(value)} must be non-negative.");
                }

                _size = value;
            }
        }

        public TKey Key { get; }
        object ICacheEntry.Key => Key;

        public TValue Value { get; set; } = default!;
        object ICacheEntry.Value
        {
            get => Value!;
            set => Value = (TValue)value;
        }

        internal Instant LastAccessed;

        internal EvictionReason EvictionReason { get; private set; }

        public void Dispose()
        {
            if (!_added)
            {
                _added = true;
                _scope.Dispose();
                _parentCollection.SetEntry(this); // notify cache of disposal
                PropagateOptions(CacheEntryHelper<TKey, TValue>.Current);
            }
        }

        internal bool CheckExpired(DateTimeOffset now)
        {
            return _isExpired || CheckForExpiredTime(now) || CheckForExpiredTokens();
        }

        internal void SetExpired(EvictionReason reason)
        {
            if (EvictionReason == EvictionReason.None)
            {
                EvictionReason = reason;
            }

            _isExpired = true;
            DetachTokens();
        }

        private bool CheckForExpiredTime(DateTimeOffset now)
        {
            if (_absoluteExpiration.HasValue && _absoluteExpiration.Value <= now)
            {
                SetExpired(EvictionReason.Expired);
                return true;
            }

            if (_slidingExpiration.HasValue && (now - LastAccessed) >= _slidingExpiration)
            {
                SetExpired(EvictionReason.Expired);
                return true;
            }

            return false;
        }

        internal bool CheckForExpiredTokens()
        {
            if (_expirationTokens != null)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var i = 0; i < _expirationTokens.Count; i++)
                {
                    var expiredToken = _expirationTokens[i];
                    if (expiredToken.HasChanged)
                    {
                        SetExpired(EvictionReason.TokenExpired);
                        return true;
                    }
                }
            }

            return false;
        }

        internal void AttachTokens()
        {
            if (_expirationTokens != null)
            {
                lock (_lock)
                {
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < _expirationTokens.Count; i++)
                    {
                        var expirationToken = _expirationTokens[i];
                        if (expirationToken.ActiveChangeCallbacks)
                        {
                            _expirationTokenRegistrations ??= new List<IDisposable>(1);

                            var registration = expirationToken.RegisterChangeCallback(ExpirationTokensExpired, this);
                            _expirationTokenRegistrations.Add(registration);
                        }
                    }
                }
            }
        }

        private static void ExpirationTokensExpired(object? obj)
        {
            // start a new thread to avoid issues with callbacks called from RegisterChangeCallback
            Task.Factory.StartNew(state =>
            {
                var entry = (CacheEntry)state!;
                entry.SetExpired(EvictionReason.TokenExpired);
                entry._parentCollection.EntryExpired(entry); // notify cache of expiration
            }, obj, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        private void DetachTokens()
        {
            lock (_lock)
            {
                var registrations = _expirationTokenRegistrations;
                if (registrations != null)
                {
                    _expirationTokenRegistrations = null;
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < registrations.Count; i++)
                    {
                        var registration = registrations[i];
                        registration.Dispose();
                    }
                }
            }
        }

        internal void InvokeEvictionCallbacks()
        {
            if (_postEvictionCallbacks != null)
            {
                Task.Factory.StartNew(
                    static state => InvokeCallbacks((CacheEntry)state!),
                    this,
                    CancellationToken.None,
                    TaskCreationOptions.DenyChildAttach,
                    TaskScheduler.Default
                );
            }
        }

        private static void InvokeCallbacks(CacheEntry entry)
        {
            var callbackRegistrations = Interlocked.Exchange(ref entry._postEvictionCallbacks, null);
            
            if (callbackRegistrations != null)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < callbackRegistrations.Count; i++)
                {
                    var registration = callbackRegistrations[i];

                    try
                    {
                        registration.EvictionCallback(entry.Key, entry.Value, entry.EvictionReason, registration.State);
                    }
                    catch (Exception)
                    {
                        // This will be invoked on a background thread, don't let it throw.
                        // TODO: LOG
                    }
                }
            }
        }

        internal void PropagateOptions(CacheEntry? parent)
        {
            if (parent == null)
            {
                return;
            }

            // Copy expiration tokens and AbsoluteExpiration to the cache entries hierarchy.
            // We do this regardless of it gets cached because the tokens are associated with the value we'll return.
            if (_expirationTokens != null)
            {
                lock (_lock)
                {
                    lock (parent._lock)
                    {
                        foreach (var expirationToken in _expirationTokens)
                        {
                            parent.AddExpirationToken(expirationToken);
                        }
                    }
                }
            }

            if (_absoluteExpiration.HasValue)
            {
                if (!parent._absoluteExpiration.HasValue || _absoluteExpiration < parent._absoluteExpiration)
                {
                    parent._absoluteExpiration = _absoluteExpiration;
                }
            }
        }
    }
}

internal readonly struct CacheEntryStack<TKey, TValue> where TKey : notnull
{
    private readonly MemoryCache<TKey, TValue>.CacheEntry _entry;

    private CacheEntryStack(MemoryCache<TKey, TValue>.CacheEntry entry)
    {
        _entry = entry;
    }

    public static CacheEntryStack<TKey, TValue> Empty { get; } = new();

    public CacheEntryStack<TKey, TValue> Push(MemoryCache<TKey, TValue>.CacheEntry c)
    {
        return new CacheEntryStack<TKey, TValue>(c);
    }

    public MemoryCache<TKey, TValue>.CacheEntry Peek()
    {
        return _entry;
    }
}

internal static class CacheEntryHelper<TKey, TValue> where TKey : notnull
{
    private static readonly AsyncLocal<CacheEntryStack<TKey, TValue>?> _scopes = new();

    private static CacheEntryStack<TKey, TValue> Scopes
    {
        get => _scopes.Value ??= CacheEntryStack<TKey, TValue>.Empty;
        set => _scopes.Value = value;
    }

    internal static MemoryCache<TKey, TValue>.CacheEntry Current => Scopes.Peek();

    internal static ScopeLease EnterScope(MemoryCache<TKey, TValue>.CacheEntry entry)
    {
        var scopes = Scopes;

        var scopeLease = new ScopeLease(scopes);
        Scopes = scopes.Push(entry);

        return scopeLease;
    }

    internal readonly struct ScopeLease(CacheEntryStack<TKey, TValue> cacheEntryStack) : IDisposable
    {
        public void Dispose()
        {
            Scopes = cacheEntryStack;
        }
    }
}