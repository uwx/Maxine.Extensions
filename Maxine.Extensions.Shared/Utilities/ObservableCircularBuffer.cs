using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using ObservableCollections;

namespace Maxine.Extensions.Shared;

/// <inheritdoc cref="IEnumerable{T}" />
/// <summary>
/// Circular buffer.
/// 
/// When writing to a full buffer:
/// PushBack -> removes this[0] / Front()
/// PushFront -> removes this[Size-1] / Back()
/// 
/// this implementation is inspired by
/// http://www.boost.org/doc/libs/1_53_0/libs/circular_buffer/doc/circular_buffer.html
/// because I liked their interface.
/// </summary>
public sealed class ObservableCircularBuffer<T> : IList<T>, IList, IReadOnlyList<T>
{
    private readonly T?[] _buffer;

    /// <summary>
    /// The _start. Index of the first element in buffer.
    /// </summary>
    private int _start;

    /// <summary>
    /// The _end. Index after the last element in the buffer.
    /// </summary>
    private int _end;

    /// <summary>
    /// The _size. Buffer size.
    /// </summary>
    private int _size;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCircularBuffer{T}"/> class.
    /// 
    /// </summary>
    /// <param name='capacity'>
    /// Buffer capacity. Must be positive.
    /// </param>
    public ObservableCircularBuffer(int capacity)
        : this(capacity, Array.Empty<T>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCircularBuffer{T}"/> class.
    /// 
    /// </summary>
    /// <param name='capacity'>
    /// Buffer capacity. Must be positive.
    /// </param>
    /// <param name='items'>
    /// Items to fill buffer with. Items length must be less than capacity.
    /// Suggestion: use Skip(x).Take(y).ToArray() to build this argument from
    /// any enumerable.
    /// </param>
    public ObservableCircularBuffer(int capacity, T[] items)
    {
        if (capacity < 1)
        {
            throw new ArgumentException(
                "Circular buffer cannot have negative or zero capacity.", nameof(capacity));
        }
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        if (items.Length > capacity)
        {
            throw new ArgumentException(
                "Too many items to fit circular buffer", nameof(items));
        }

        _buffer = new T[capacity];

        Array.Copy(items, _buffer, items.Length);
        _size = items.Length;

        _start = 0;
        _end = _size == capacity ? 0 : _size;
    }

    /// <summary>
    /// Maximum capacity of the buffer. Elements pushed into the buffer after
    /// maximum capacity is reached (IsFull = true), will remove an element.
    /// </summary>
    public int Capacity => _buffer.Length;

    /// <summary>
    /// Boolean indicating if Circular is at full capacity.
    /// Adding more elements when the buffer is full will
    /// cause elements to be removed from the other end
    /// of the buffer.
    /// </summary>
    public bool IsFull => Count == Capacity;

    /// <summary>
    /// True if has no elements.
    /// </summary>
    public bool IsEmpty => Count == 0;

    /// <summary>
    /// Element at the front of the buffer - this[0].
    /// </summary>
    /// <returns>The value of the element of type T at the front of the buffer.</returns>
    public T Front()
    {
        ThrowIfEmpty();
        return _buffer[_start]!;
    }

    /// <summary>
    /// Element at the back of the buffer - this[Size - 1].
    /// </summary>
    /// <returns>The value of the element of type T at the back of the buffer.</returns>
    public T Back()
    {
        ThrowIfEmpty();
        return _buffer[(_end != 0 ? _end : Capacity) - 1]!;
    }

    /// <summary>
    /// Index access to elements in buffer.
    /// Index does not loop around like when adding elements,
    /// valid interval is [0;Size]
    /// </summary>
    /// <param name="index">Index of element to access.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when index is outside of [; Size] interval.</exception>
    public T this[int index]
    {
        get
        {
            if (IsEmpty)
            {
                throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");
            }
            if (index >= _size)
            {
                throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {_size}");
            }
            var actualIndex = InternalIndex(index);
            return _buffer[actualIndex]!;
        }
        set
        {
            CheckReentrancy();

            if (IsEmpty)
            {
                throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");
            }
            if (index >= _size)
            {
                throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {_size}");
            }
            var actualIndex = InternalIndex(index);
            SwapAndGetOld(ref _buffer[actualIndex], value, out var oldValue);

            OnReplace(value, oldValue!, index);
        }
    }

    /// <summary>
    /// Pushes a new element to the back of the buffer. Back()/this[Size-1]
    /// will now return this element.
    /// 
    /// When the buffer is full, the element at Front()/this[0] will be 
    /// popped to allow for this new element to fit.
    /// </summary>
    /// <param name="item">Item to push to the back of the buffer</param>
    public void PushBack(T item)
    {
        CheckReentrancy();

        if (IsFull)
        {
            PopFront();
        }

        _buffer[_end] = item;
        Increment(ref _end);
        _size++;

        OnPushBack(item, _size - 1);
    }

    /// <summary>
    /// Pushes a new element to the front of the buffer. Front()/this[0]
    /// will now return this element.
    /// 
    /// When the buffer is full, the element at Back()/this[Size-1] will be 
    /// popped to allow for this new element to fit.
    /// </summary>
    /// <param name="item">Item to push to the front of the buffer</param>
    public void PushFront(T item)
    {
        CheckReentrancy();

        if (IsFull)
        {
            PopBack();
        }

        Decrement(ref _start);
        _buffer[_start] = item;
        _size++;

        OnPushFront(item, 0);
    }

    /// <summary>
    /// Removes the element at the back of the buffer. Decreasing the 
    /// Buffer size by 1.
    /// </summary>
    public void PopBack()
    {
        CheckReentrancy();

        ThrowIfEmpty("Cannot take elements from an empty buffer.");
        Decrement(ref _end);
        SwapAndGetOld(ref _buffer[_end], default, out var popped);
        --_size;
        
        OnPopBack(popped!, _size);
    }

    /// <summary>
    /// Removes the element at the front of the buffer. Decreasing the 
    /// Buffer size by 1.
    /// </summary>
    public void PopFront()
    {
        CheckReentrancy();

        ThrowIfEmpty("Cannot take elements from an empty buffer.");
        SwapAndGetOld(ref _buffer[_start], default, out var popped);
        Increment(ref _start);
        --_size;
        
        OnPopFront(popped!, 0);
    }

    private static void SwapAndGetOld(ref T? element, T? newValue, out T? oldValue)
    {
        oldValue = element;
        element = newValue;
    }

    #region ICollection<T> Implementation

    void ICollection.CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Current buffer size (the number of elements that the buffer has).
    /// </summary>
    public int Count => _size;

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot { get; } = new();

    void ICollection<T>.Add(T item)
    {
        PushBack(item);
    }

    /// <summary>
    /// Clears the contents of the array. Size = 0, Capacity is unchanged.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Clear()
    {
        CheckReentrancy();

        // to clear we just reset everything.
        _start = 0;
        _end = 0;
        _size = 0;
        Array.Clear(_buffer, 0, _buffer.Length);
        OnCollectionReset();
    }

    public bool Contains(T item)
    {
        var segments = ToArraySegments();

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var segment in segments)
        {
            if (segment.Contains(item))
            {
                return true;
            }
        }

        return false;
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        var segments = ToArraySegments();
        foreach (var segment in segments)
        {
            Array.Copy(segment.Array!, segment.Offset, array, arrayIndex, segment.Count);
            arrayIndex += segment.Count;
        }
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new InvalidOperationException("Not supported.");
    }

    bool ICollection<T>.IsReadOnly => false;
    #endregion

    #region IList<T> Implementation
    public int IndexOf(T item)
    {
        foreach (var segment in ToArraySegments())
        {
            for (var i = 0; i < segment.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(item, segment[i]))
                {
                    return ExternalIndex(segment.Offset + i);
                }
            }
        }

        return -1;
    }

    void IList<T>.Insert(int index, T item)
    {
        throw new InvalidOperationException("Not supported.");
    }

    void IList<T>.RemoveAt(int index)
    {
        throw new InvalidOperationException("Not supported.");
    }
    #endregion

    #region IList Implementation
    void IList.Remove(object? value)
    {
        throw new InvalidOperationException("Not supported.");
    }

    void IList.RemoveAt(int index)
    {
        throw new InvalidOperationException("Not supported.");
    }

    bool IList.IsFixedSize => false;

    bool IList.IsReadOnly => false;

    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value!;
    }

    int IList.Add(object? value)
    {
        PushBack((T)value!);
        return _size - 1;
    }

    bool IList.Contains(object? value)
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var segment in ToArraySegments())
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in segment)
            {
                if (Equals(value, item))
                {
                    return true;
                }
            }
        }

        return false;
    }

    int IList.IndexOf(object? value)
    {
        foreach (var segment in ToArraySegments())
        {
            for (var i = 0; i < segment.Count; i++)
            {
                if (Equals(value, segment[i]))
                {
                    return ExternalIndex(segment.Offset + i);
                }
            }
        }

        return -1;
    }

    void IList.Insert(int index, object? value)
    {
        throw new InvalidOperationException("Not supported.");
    }
    #endregion

    /// <summary>
    /// Copies the buffer contents to an array, according to the logical
    /// contents of the buffer (i.e. independent of the internal 
    /// order/contents)
    /// </summary>
    /// <returns>A new array with a copy of the buffer contents.</returns>
    public T[] ToArray()
    {
        var newArray = new T[Count];
        var newArrayOffset = 0;
        var segments = ToArraySegments();
        foreach (var segment in segments)
        {
            Array.Copy(segment.Array!, segment.Offset, newArray, newArrayOffset, segment.Count);
            newArrayOffset += segment.Count;
        }
        return newArray;
    }

    /// <summary>
    /// Get the contents of the buffer as 2 ArraySegments.
    /// Respects the logical contents of the buffer, where
    /// each segment and items in each segment are ordered
    /// according to insertion.
    ///
    /// Fast: does not copy the array elements.
    /// Useful for methods like <c>Send(IList&lt;ArraySegment&lt;Byte&gt;&gt;)</c>.
    /// 
    /// <remarks>Segments may be empty.</remarks>
    /// </summary>
    /// <returns>An IList with 2 segments corresponding to the buffer content.</returns>
    public IReadOnlyList<ArraySegment<T>> ToArraySegments()
    {
        return new[] { ArrayOne(), ArrayTwo() };
    }

    #region IEnumerable<T> implementation
    /// <summary>
    /// Returns an enumerator that iterates through this buffer.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate this collection.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        var segments = ToArraySegments();
        foreach (var segment in segments)
        {
            for (var i = 0; i < segment.Count; i++)
            {
                yield return segment.Array![segment.Offset + i];
            }
        }
    }
    #endregion
    #region IEnumerable implementation
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion

    private void ThrowIfEmpty(string message = "Cannot access an empty buffer.")
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Increments the provided index variable by one, wrapping
    /// around if necessary.
    /// </summary>
    /// <param name="index"></param>
    private void Increment(ref int index)
    {
        if (++index == Capacity)
        {
            index = 0;
        }
    }

    /// <summary>
    /// Decrements the provided index variable by one, wrapping
    /// around if necessary.
    /// </summary>
    /// <param name="index"></param>
    private void Decrement(ref int index)
    {
        if (index == 0)
        {
            index = Capacity;
        }
        index--;
    }

    /// <summary>
    /// Converts the index in the argument to an index in <code>_buffer</code>
    /// </summary>
    /// <returns>
    /// The transformed index.
    /// </returns>
    /// <param name='index'>
    /// External index.
    /// </param>
    internal int InternalIndex(int index)
    {
        int x;
        if (index < Capacity - _start)
            x = index;
        else
            x = index - Capacity;

        return _start + x;
    }

    // Reverse of InternalIndex
    internal int ExternalIndex(int index)
    {
        index -= _start;

        if (index < 0)
            return index + Capacity;
        else
            return index;
    }

    // doing ArrayOne and ArrayTwo methods returning ArraySegment<T> as seen here: 
    // http://www.boost.org/doc/libs/1_37_0/libs/circular_buffer/doc/circular_buffer.html#classboost_1_1circular__buffer_1957cccdcb0c4ef7d80a34a990065818d
    // http://www.boost.org/doc/libs/1_37_0/libs/circular_buffer/doc/circular_buffer.html#classboost_1_1circular__buffer_1f5081a54afbc2dfc1a7fb20329df7d5b
    // should help a lot with the code.

    #region Array items easy access.
    // The array is composed by at most two non-contiguous segments, 
    // the next two methods allow easy access to those.

    private ArraySegment<T> ArrayOne()
    {
        if (IsEmpty)
        {
            return new ArraySegment<T>(Array.Empty<T>());
        }

        if (_start < _end)
        {
            return new ArraySegment<T>(_buffer!, _start, _end - _start);
        }
        
        return new ArraySegment<T>(_buffer!, _start, _buffer.Length - _start);
    }

    private ArraySegment<T> ArrayTwo()
    {
        if (IsEmpty)
        {
            return new ArraySegment<T>(Array.Empty<T>());
        }

        if (_start < _end)
        {
            return new ArraySegment<T>(_buffer!, _end, 0);
        }

        return new ArraySegment<T>(_buffer!, 0, _end);
    }
    #endregion

    #region Observable Implementation
    
    /// <summary>
    /// Helper to raise CollectionChanged event to any listeners
    /// </summary>
    private void OnPushBack(T item, int index)
    {
        if (!_suppressUpdates)
            OnCollectionChanged(new NotifyCircularBufferChangedEventArgs<T>(BufferChangedAction.PushBack, item, index: index));
    }

    /// <summary>
    /// Helper to raise CollectionChanged event to any listeners
    /// </summary>
    private void OnPushFront(T item, int index)
    {
        if (!_suppressUpdates)
            OnCollectionChanged(new NotifyCircularBufferChangedEventArgs<T>(BufferChangedAction.PushFront, item, index: index));
    }

    /// <summary>
    /// Helper to raise CollectionChanged event to any listeners
    /// </summary>
    private void OnPopBack(T item, int index)
    {
        if (!_suppressUpdates)
            OnCollectionChanged(new NotifyCircularBufferChangedEventArgs<T>(BufferChangedAction.PopBack, item, index: index));
    }
    
    /// <summary>
    /// Helper to raise CollectionChanged event to any listeners
    /// </summary>
    private void OnPopFront(T item, int index)
    {
        if (!_suppressUpdates)
            OnCollectionChanged(new NotifyCircularBufferChangedEventArgs<T>(BufferChangedAction.PopFront, item, index: index));
    }

    /// <summary>
    /// Helper to raise CollectionChanged event to any listeners
    /// </summary>
    private void OnReplace(T oldItem, T newItem, int index)
    {
        if (!_suppressUpdates)
            OnCollectionChanged(new NotifyCircularBufferChangedEventArgs<T>(BufferChangedAction.Replace, oldItem, newItem, index));
    }

    /// <summary>
    /// Helper to raise CollectionChanged event with action == Reset to any listeners
    /// </summary>
    private void OnCollectionReset()
    {
        if (!_suppressUpdates)
            OnCollectionChanged(new NotifyCircularBufferChangedEventArgs<T>(BufferChangedAction.Clear));
    }

    private int _blockReentrancyCount;

    /// <summary>
    /// Raise CollectionChanged event to any listeners.
    /// Properties/methods modifying this ObservableCollection will raise
    /// a collection changed event through this virtual method.
    /// </summary>
    /// <remarks>
    /// When overriding this method, either call its base implementation
    /// or call <see cref="BlockReentrancy"/> to guard against reentrant collection changes.
    /// </remarks>
    private void OnCollectionChanged(in NotifyCircularBufferChangedEventArgs<T> e)
    {
        if (BufferChanged is {} handler)
        {
            // Not calling BlockReentrancy() here to avoid the SimpleMonitor allocation.
            _blockReentrancyCount++;
            try
            {
                handler(e);
            }
            finally
            {
                _blockReentrancyCount--;
            }
        }
    }

    /// <summary> Check and assert for reentrant attempts to change this collection. </summary>
    /// <exception cref="InvalidOperationException"> raised when changing the collection
    /// while another collection change is still being notified to other listeners </exception>
    private void CheckReentrancy()
    {
        if (_blockReentrancyCount > 0)
        {
            // we can allow changes if there's only one listener - the problem
            // only arises if reentrant changes make the original event args
            // invalid for later listeners.  This keeps existing code working
            // (e.g. Selector.SelectedItems).
            if (BufferChanged?.GetInvocationList().Length > 1)
                throw new InvalidOperationException("Reentrancy not allowed.");
        }
    }
    
    /// <summary>
    /// Disallow reentrant attempts to change this collection. E.g. an event handler
    /// of the CollectionChanged event is not allowed to make changes to this collection.
    /// </summary>
    /// <remarks>
    /// typical usage is to wrap e.g. a OnCollectionChanged call with a using() scope:
    /// <code>
    ///         using (BlockReentrancy())
    ///         {
    ///             CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
    ///         }
    /// </code>
    /// </remarks>
    private SimpleMonitor BlockReentrancy()
    {
        _blockReentrancyCount++;
        return EnsureMonitorInitialized();
    }
    
    private SimpleMonitor? _monitor; // Lazily allocated only when a subclass calls BlockReentrancy() or during serialization. Do not rename (binary serialization)

    private SimpleMonitor EnsureMonitorInitialized() => _monitor ??= new SimpleMonitor(this);
    
    // this class helps prevent reentrant calls
    private readonly struct SimpleMonitor : IDisposable
    {
        internal readonly ObservableCircularBuffer<T> _collection;

        public SimpleMonitor(ObservableCircularBuffer<T> collection)
        {
            _collection = collection;
        }

        public void Dispose() => _collection._blockReentrancyCount--;
    }
    #endregion
    
    public event NotifyCircularBufferChangedEventHandler<T>? BufferChanged;

    private bool _suppressUpdates;
    public SuppressingUpdates SuppressUpdates()
    {
        _suppressUpdates = true;
        return new SuppressingUpdates(this);
    }

    public readonly struct SuppressingUpdates : IDisposable
    {
        private readonly ObservableCircularBuffer<T> _collection;

        public SuppressingUpdates(ObservableCircularBuffer<T> collection)
        {
            _collection = collection;
        }

        public void Dispose()
        {
            _collection._suppressUpdates = false;
        }
    }
}

public enum BufferChangedAction
{
    PopFront, PopBack, PushFront, PushBack, Replace, Clear
}

public delegate void NotifyCircularBufferChangedEventHandler<T>(in NotifyCircularBufferChangedEventArgs<T> e);
public readonly ref struct NotifyCircularBufferChangedEventArgs<T>
{
    public readonly BufferChangedAction Action;
    public readonly T Operand;
    public readonly T? ReplacedWith;
    public readonly int Index;

    public NotifyCircularBufferChangedEventArgs(BufferChangedAction action, T operand = default!, T? replacedWith = default, int index = -1)
    {
        Action = action;
        Operand = operand;
        ReplacedWith = replacedWith;
        Index = index;
    }
}