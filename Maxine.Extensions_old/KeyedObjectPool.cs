using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
// using Microsoft.Extensions.ObjectPool;

namespace Maxine.TU.UploadHelpers;

// https://stackoverflow.com/a/60719233
internal readonly struct ConcurrentQueueMultiDictionary<TKey, TValue> : IDictionary<TKey, ConcurrentQueue<TValue>>, IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, ConcurrentQueue<TValue>> _dictionary = new();

    public ConcurrentQueueMultiDictionary()
    {
    }

    void ICollection<KeyValuePair<TKey, ConcurrentQueue<TValue>>>.Add(KeyValuePair<TKey, ConcurrentQueue<TValue>> item)
    {
        _dictionary[item.Key] = item.Value;
    }

    public void Clear()
    {
        foreach (var (_, v) in this)
        {
            if (v is IDisposable dis)
            {
                dis.Dispose();
            }
        }
        _dictionary.Clear();
    }

    bool ICollection<KeyValuePair<TKey, ConcurrentQueue<TValue>>>.Contains(KeyValuePair<TKey, ConcurrentQueue<TValue>> item)
    {
        return _dictionary.Contains(item);
    }
    
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return Contains(item.Key, item.Value);
    }

    void ICollection<KeyValuePair<TKey, ConcurrentQueue<TValue>>>.CopyTo(KeyValuePair<TKey, ConcurrentQueue<TValue>>[] array, int arrayIndex)
    {
        ((IDictionary<TKey, ConcurrentQueue<TValue>>)_dictionary).CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<TKey, ConcurrentQueue<TValue>>>.Remove(KeyValuePair<TKey, ConcurrentQueue<TValue>> item)
    {
        return _dictionary.TryRemove(item);
    }

    public int Count => _dictionary.Count;
    
    bool ICollection<KeyValuePair<TKey, ConcurrentQueue<TValue>>>.IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        _dictionary.AddOrUpdate(
            key,
            static (_, value1) =>
            {
                var concurrentQueue = new ConcurrentQueue<TValue>();
                concurrentQueue.Enqueue(value1);
                return concurrentQueue;
            },
            static (_, bag, value1) =>
            {
                bag.Enqueue(value1);
                return bag;
            },
            value
        );
    }

    public bool TryDequeue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_dictionary.TryGetValue(key, out var bag))
        {
            var v = bag.TryDequeue(out value);
            if (v)
            {
                if (bag.IsEmpty)
                {
                    _dictionary.TryRemove(KeyValuePair.Create(key, bag));
                }
            }

            return v;
        }

        value = default;
        return false;
    }

    public bool TryGetValues(TKey key, [NotNullWhen(true)] out IReadOnlyCollection<TValue>? values)
    {
        if (_dictionary.TryGetValue(key, out var values1))
        {
            values = values1;
            return true;
        }

        values = default;
        return false;
    }

    public bool Contains(TKey key, TValue value)
    {
        return _dictionary.TryGetValue(key, out var bag) && bag.Contains(value);
    }

    void IDictionary<TKey, ConcurrentQueue<TValue>>.Add(TKey key, ConcurrentQueue<TValue> value)
    {
        if (!_dictionary.TryAdd(key, value))
        {
            throw new ArgumentException("Key is already present in the dictionary.");
        }
    }

    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

    public bool Remove(TKey key)
    {
        var b = _dictionary.TryRemove(key, out var res);

        if (b)
        {
            foreach (var value in res!)
            {
                if (value is IDisposable dis)
                {
                    dis.Dispose();
                }
            }
        }

        return b;
    }

    bool IDictionary<TKey, ConcurrentQueue<TValue>>.TryGetValue(TKey key, [MaybeNullWhen(false)] out ConcurrentQueue<TValue> value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    ConcurrentQueue<TValue> IDictionary<TKey, ConcurrentQueue<TValue>>.this[TKey key]
    {
        get => _dictionary[key];
        set => _dictionary[key] = value;
    }

    public ICollection<TKey> Keys => _dictionary.Keys;
    public ICollection<ConcurrentQueue<TValue>> Values => _dictionary.Values;

    IEnumerator<KeyValuePair<TKey, ConcurrentQueue<TValue>>> IEnumerable<KeyValuePair<TKey, ConcurrentQueue<TValue>>>.GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var key in _dictionary.Keys)
        {
            if (TryGetValues(key, out var bag))
            {
                foreach (var value in bag)
                {
                    yield return KeyValuePair.Create(key, value);
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>Represents a policy for managing pooled objects.</summary>
/// <typeparam name="T">The type of object which is being pooled.</typeparam>
/// <typeparam name="TKey">The key type</typeparam>
public interface IPooledObjectPolicy<in TKey, T> where T : notnull
{
    /// <summary>
    /// Create a <typeparamref name="T" />.
    /// </summary>
    /// <returns>The <typeparamref name="T" /> which was created.</returns>
    T Create(TKey key);

    /// <summary>
    /// Runs some processing when an object was returned to the pool. Can be used to reset the state of an object and indicate if the object should be returned to the pool.
    /// </summary>
    /// <param name="obj">The object to return to the pool.</param>
    /// <returns><see langword="true" /> if the object should be returned to the pool. <see langword="false" /> if it's not possible/desirable for the pool to keep the object.</returns>
    bool Return(T obj);
}

/// <summary>
/// Default implementation of <see cref="ObjectPool{T}"/>.
/// </summary>
/// <typeparam name="T">The type to pool objects for.</typeparam>
/// <typeparam name="TKey">The key type</typeparam>
/// <remarks>This implementation keeps a cache of retained objects. This means that if objects are returned when the pool has already reached "maximumRetained" objects they will be available to be Garbage Collected.</remarks>
public sealed class KeyedObjectPool<T, TKey> where T : class
{
    private readonly Func<T, TKey> _getKeyForElement;
    private readonly IEqualityComparer<TKey> _keyComparer;
    private readonly Func<TKey, T> _createFunc;
    private readonly Func<T, bool> _returnFunc;
    private readonly int _maxCapacity;
    private int _numItems;

    private protected readonly ConcurrentQueueMultiDictionary<TKey, T> _items = new();

    private T? _fastItem;

    private volatile bool _isDisposed;

    /// <summary>
    /// Creates an instance of <see cref="DefaultObjectPool{T}"/>.
    /// </summary>
    /// <param name="policy">The pooling policy to use.</param>
    public KeyedObjectPool(IPooledObjectPolicy<TKey, T> policy, Func<T, TKey> getKeyForElement, IEqualityComparer<TKey>? keyComparer = null)
        : this(policy, Environment.ProcessorCount * 2, getKeyForElement, keyComparer)
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="DefaultObjectPool{T}"/>.
    /// </summary>
    /// <param name="policy">The pooling policy to use.</param>
    /// <param name="maximumRetained">The maximum number of objects to retain in the pool.</param>
    public KeyedObjectPool(
        IPooledObjectPolicy<TKey, T> policy, int maximumRetained, Func<T, TKey> getKeyForElement, IEqualityComparer<TKey>? keyComparer = null
    )
        : this(policy.Create, policy.Return, maximumRetained, getKeyForElement, keyComparer)
    {
        
    }

    public KeyedObjectPool(
        Func<TKey, T> createFunc,
        Func<T, bool> returnFunc,
        int maximumRetained,
        Func<T, TKey> getKeyForElement,
        IEqualityComparer<TKey>? keyComparer = null
    )
    {
        _getKeyForElement = getKeyForElement;
        _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;

        // cache the target interface methods, to avoid interface lookup overhead
        _createFunc = createFunc;
        _returnFunc = returnFunc;
        _maxCapacity = maximumRetained - 1;  // -1 to account for _fastItem
    }
    

    public T Get(TKey key)
    {
        if (_isDisposed)
        {
            ThrowObjectDisposedException();
        }

        var item = _fastItem;
        if (item is null || !_keyComparer.Equals(_getKeyForElement(item), key) || Interlocked.CompareExchange(ref _fastItem, null, item) != item)
        {
            if (_items.TryDequeue(key, out var item1))
            {
                Interlocked.Decrement(ref _numItems);
                return item1;
            }

            // no object available, so go get a brand new one
            return _createFunc(key);
        }

        return item;

        void ThrowObjectDisposedException()
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    public void Return(T obj)
    {
        // When the pool is disposed or the obj is not returned to the pool, dispose it
        if (_isDisposed || !ReturnCore(obj))
        {
            DisposeItem(obj);
        }
    }

    public void Dispose()
    {
        _isDisposed = true;

        DisposeItem(_fastItem);
        _fastItem = null;

        foreach (var (_, v) in _items)
        {
            if (v is IDisposable dis)
            {
                dis.Dispose();
            }
        }
    }

    private static void DisposeItem(T? item)
    {
        if (item is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <summary>
    /// Returns an object to the pool.
    /// </summary>
    /// <returns>true if the object was returned to the pool</returns>
    private bool ReturnCore(T obj)
    {
        if (!_returnFunc(obj))
        {
            // policy says to drop this object
            return false;
        }

        if (_fastItem is not null || Interlocked.CompareExchange(ref _fastItem, obj, null) != null)
        {
            if (Interlocked.Increment(ref _numItems) <= _maxCapacity)
            {
                _items.Add(_getKeyForElement(obj), obj);
                return true;
            }

            // no room, clean up the count and drop the object on the floor
            Interlocked.Decrement(ref _numItems);
            return false;
        }

        return true;
    }
}
