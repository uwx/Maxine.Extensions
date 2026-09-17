using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Maxine.Extensions.Collections;

public struct PooledList<T, TPool>(TPool pool) : IList<T>, IReadOnlyList<T>, IDisposable
    where TPool : ArrayPool<T>
{
    private readonly TPool _pool = pool;
    private T[] _items = [];
    private int _size = 0;

    public struct Enumerator(PooledList<T, TPool> array) : IEnumerator<T>
    {
        private int _index = -1;

        public readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => array[_index];
        }

        readonly object? IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => array[_index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            _index++;
            return _index < array._size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose()
        {
        }
    }

    public readonly Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return new Enumerator(this);
    }

    readonly IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }

    public void Add(T item)
    {
        var array = _items;
        var size = _size;
        if ((uint)size < (uint)array.Length)
        {
            _size = size + 1;
            array[size] = item;
        }
        else
        {
            AddWithResize(item);
        }
    }

    // Non-inline from List.Add to improve its code quality as uncommon path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        var size = _size;
        Grow(size + 1);
        _size = size + 1;
        _items[size] = item;
    }

    internal void Grow(int capacity)
    {
        capacity = GetNewCapacity(capacity);
        var newArr = _pool.Rent(capacity);
        Array.Copy(_items, 0, newArr, 0, _size);
        if (_items.Length > 0) _pool.Return(_items);
        _items = newArr;
    }

    internal void GrowForInsertion(int indexToInsert, int insertionCount = 1)
    {
        var requiredCapacity = checked(_size + insertionCount);
        var newCapacity = GetNewCapacity(requiredCapacity);

        var newItems = _pool.Rent(newCapacity);
        if (indexToInsert != 0)
        {
            Array.Copy(_items, newItems, length: indexToInsert);
        }

        if (_size != indexToInsert)
        {
            Array.Copy(_items, indexToInsert, newItems, indexToInsert + insertionCount, _size - indexToInsert);
        }

        if (_items.Length > 0) _pool.Return(_items);
        _items = newItems;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly int GetNewCapacity(int capacity)
    {
        var newCapacity = _items.Length == 0 ? 4 : 2 * _items.Length;

        // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
        // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
        if ((uint)newCapacity > Array.MaxLength) newCapacity = Array.MaxLength;

        // If the computed capacity is still less than specified, set to the original argument.
        // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
        if (newCapacity < capacity) newCapacity = capacity;

        return newCapacity;
    }

    public void Clear()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            var size = _size;
            _size = 0;
            if (size > 0)
            {
                Array.Clear(_items, 0, size); // Clear the elements so that the gc can reclaim the references.
            }
        }
        else
        {
            _size = 0;
        }
    }

    public readonly bool Contains(T item)
    {
        return _size != 0 && IndexOf(item) >= 0;
    }

    public readonly void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(_items, 0, array, arrayIndex, _size);
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public readonly int Count => _size;

    public readonly bool IsReadOnly => false;
    
    public readonly int IndexOf(T item)
        => Array.IndexOf(_items, item, 0, _size);

    public void Insert(int index, T item)
    {
        // Note that insertions at the end are legal.
        if ((uint)index > (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, "Index cannot be greater than the size of the collection.");
        }
        if (_size == _items.Length)
        {
            GrowForInsertion(index, 1);
        }
        else if (index < _size)
        {
            Array.Copy(_items, index, _items, index + 1, _size - index);
        }
        _items[index] = item;
    }

    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be less than the size of the collection.");
        }
        _size--;
        if (index < _size)
        {
            Array.Copy(_items, index + 1, _items, index, _size - index);
        }
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _items[_size] = default!;
        }
    }

    public readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index < 0 || index >= _size)
                ThrowArgumentOutOfRange(index);
            return _items[index];

            static void ThrowArgumentOutOfRange(int i)
            {
                throw new ArgumentOutOfRangeException(nameof(i), i, "Index must be non-negative and less than the size of the collection.");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (index < 0 || index >= _size)
                ThrowArgumentOutOfRange(index);
            _items[index] = value;

            static void ThrowArgumentOutOfRange(int i)
            {
                throw new ArgumentOutOfRangeException(nameof(i), i, "Index must be non-negative and less than the size of the collection.");
            }
        }
    }

    readonly int IReadOnlyCollection<T>.Count => _size;

    public Span<T> Span => _items.AsSpan(0, _size);

    public readonly void Dispose()
    {
        if (_items.Length > 0) _pool.Return(_items);
    }
}