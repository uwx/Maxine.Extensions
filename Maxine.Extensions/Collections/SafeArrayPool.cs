using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Maxine.Extensions.Collections;

/// <summary>
/// An <see cref="ArrayPool{T}"/> wrapper that returns disposable array segments.
/// </summary>
/// <param name="backingPool">The backing <see cref="ArrayPool{T}"/>.</param>
/// <typeparam name="T">The type of element stored in the arrays.</typeparam>
public readonly struct SafeArrayPool<T>(ArrayPool<T> backingPool)
{
    public static SafeArrayPool<T> Shared => new(ArrayPool<T>.Shared);
    
    public DisposableArraySegment<T> Rent(int minimumLength)
    {
        return new DisposableArraySegment<T>(backingPool.Rent(minimumLength), 0, minimumLength, static (arr, pool) => ((ArrayPool<T>)pool!).Return(arr), backingPool);
    }
}

public struct DisposableArraySegment<T>(T[]? array, int offset, int count, Action<T[], object?>? disposeFunc = null, object? context = null) : IDisposable, IList<T>, IReadOnlyList<T>, IEquatable<DisposableArraySegment<T>>
{
    private Action<T[], object?>? _disposeFunc = disposeFunc;
    private object? _context = context;
    
    public T[]? Array { get; private set; } = array;
    public int Offset { get; } = offset;
    public int Count { get; } = count;

    public readonly Span<T> AsSpan(int offset, int count) => Array.AsSpan(Offset, Count).Slice(offset, count);

    /// <summary>
    /// Returns a <see cref="Memory{T}"/> which will dispose the array when it is disposed.
    /// </summary>
    /// <remarks>This incurs an extra allocation.</remarks>
    /// <returns>The safe <see cref="Memory{T}"/></returns>
    public readonly Memory<T> AsSafeMemory()
        => new PooledArrayMemoryManager(Array, Offset, Count, _disposeFunc, _context).Memory;

    private sealed class PooledArrayMemoryManager(
        T[]? array,
        int offset,
        int count,
        Action<T[], object?>? disposeFunc,
        object? context)
        : MemoryManager<T>
    {
        private T[]? _array = array;
        private Action<T[], object?>? _disposeFunc = disposeFunc;
        private object? _context = context;

        protected override void Dispose(bool disposing)
        {
            if (_array != null)
            {
                _disposeFunc?.Invoke(_array, _context);
                _array = null;
                _context = null;
                _disposeFunc = null;
            }
        }

        public override Span<T> GetSpan() => new(_array, offset, count);

        public override MemoryHandle Pin(int elementIndex = 0)
        {
            return _array.AsMemory(offset + elementIndex, count).Pin();
        }

        public override void Unpin()
        {
        }
    }

    public T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Count)
            {
                ThrowArgumentOutOfRange_IndexMustBeLessException();
            }

            return Array![Offset + index];
        }
        set
        {
            if ((uint)index >= (uint)Count)
            {
                ThrowArgumentOutOfRange_IndexMustBeLessException();
            }

            Array![Offset + index] = value;
        }
    }

    [DoesNotReturn]
    private static void ThrowArgumentOutOfRange_IndexMustBeLessException()
    {
        // ReSharper disable once NotResolvedInText
        throw new ArgumentOutOfRangeException("value", "Index must be smaller than the size of the collection");
    }
    
    public Enumerator GetEnumerator()
    {
        ThrowInvalidOperationIfDefault();
        return new Enumerator(this);
    }

    public override int GetHashCode() =>
        Array is null ? 0 : HashCode.Combine(Offset, Count, Array.GetHashCode(), _disposeFunc, _context);

    public void CopyTo(T[] destination) => CopyTo(destination, 0);

    public void CopyTo(T[] destination, int destinationIndex)
    {
        ThrowInvalidOperationIfDefault();
        global::System.Array.Copy(Array!, Offset, destination, destinationIndex, Count);
    }

    public void CopyTo(ArraySegment<T> destination)
    {
        ThrowInvalidOperationIfDefault();

        if (Count > destination.Count)
        {
            ThrowArgumentException_DestinationTooShort();
        }

        global::System.Array.Copy(Array!, Offset, destination.Array!, destination.Offset, Count);

        return;

        static void ThrowArgumentException_DestinationTooShort()
        {
            throw new ArgumentException("Destination too small", "destination");
        }
    }

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ArraySegment<T> other && Equals(other);

    public bool Equals(DisposableArraySegment<T> obj) =>
        obj.Array == Array && obj.Offset == Offset && obj.Count == Count;

    public ArraySegment<T> Slice(int index)
    {
        ThrowInvalidOperationIfDefault();

        if ((uint)index > (uint)Count)
        {
            ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
        }

        return new ArraySegment<T>(Array!, Offset + index, Count - index);
    }

    public ArraySegment<T> Slice(int index, int count)
    {
        ThrowInvalidOperationIfDefault();

        if ((uint)index > (uint)Count || (uint)count > (uint)(Count - index))
        {
            ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
        }

        return new ArraySegment<T>(Array!, Offset + index, count);
    }

    [DoesNotReturn]
    private static void ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException()
    {
        throw new ArgumentOutOfRangeException("index", "Index must be less or equal to the size of the collection");
    }

    public T[] ToArray()
    {
        ThrowInvalidOperationIfDefault();

        if (Count == 0)
        {
            return [];
        }

        var array = new T[Count];
        global::System.Array.Copy(Array!, Offset, array, 0, Count);
        return array;
    }

    public static bool operator ==(DisposableArraySegment<T> a, DisposableArraySegment<T> b) => a.Equals(b);

    public static bool operator !=(DisposableArraySegment<T> a, DisposableArraySegment<T> b) => !(a == b);

    public static implicit operator DisposableArraySegment<T>(T[]? array) => array != null ? new DisposableArraySegment<T>(array, 0, array.Length) : default;

    #region IList<T>
    T IList<T>.this[int index]
    {
        get
        {
            ThrowInvalidOperationIfDefault();
            if (index < 0 || index >= Count)
                ThrowArgumentOutOfRange_IndexMustBeLessException();

            return Array![Offset + index];
        }

        set
        {
            ThrowInvalidOperationIfDefault();
            if (index < 0 || index >= Count)
                ThrowArgumentOutOfRange_IndexMustBeLessException();

            Array![Offset + index] = value;
        }
    }

    int IList<T>.IndexOf(T item)
    {
        ThrowInvalidOperationIfDefault();

        int index = global::System.Array.IndexOf(Array!, item, Offset, Count);

        Debug.Assert(index < 0 ||
                        (index >= Offset && index < Offset + Count));

        return index >= 0 ? index - Offset : -1;
    }

    void IList<T>.Insert(int index, T item) => ThrowNotSupportedException();

    void IList<T>.RemoveAt(int index) => ThrowNotSupportedException();
    #endregion

    #region IReadOnlyList<T>
    T IReadOnlyList<T>.this[int index]
    {
        get
        {
            ThrowInvalidOperationIfDefault();
            if (index < 0 || index >= Count)
                ThrowArgumentOutOfRange_IndexMustBeLessException();

            return Array![Offset + index];
        }
    }
    #endregion IReadOnlyList<T>

    #region ICollection<T>
    bool ICollection<T>.IsReadOnly =>
        // the indexer setter does not throw an exception although IsReadOnly is true.
        // This is to match the behavior of arrays.
        true;

    void ICollection<T>.Add(T item) => ThrowNotSupportedException();

    void ICollection<T>.Clear() => ThrowNotSupportedException();

    bool ICollection<T>.Contains(T item)
    {
        ThrowInvalidOperationIfDefault();

        int index = global::System.Array.IndexOf(Array!, item, Offset, Count);

        Debug.Assert(index < 0 ||
                        (index >= Offset && index < Offset + Count));

        return index >= 0;
    }

    bool ICollection<T>.Remove(T item)
    {
        ThrowNotSupportedException();
        return default;
    }

    [DoesNotReturn]
    private static void ThrowNotSupportedException()
    {
        throw new NotSupportedException();
    }

    #endregion

    #region IEnumerable<T>

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        ThrowInvalidOperationIfDefault();
        return
            Count == 0 ? Enumerable.Empty<T>().GetEnumerator() :
            new Enumerator(this);
    }
    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    #endregion

    private void ThrowInvalidOperationIfDefault()
    {
        if (Array == null)
        {
            ThrowInvalidOperationException();
        }
        return;

        [DoesNotReturn]
        static void ThrowInvalidOperationException()
        {
            throw new InvalidOperationException("ArraySegment array is null");
        }
    }

    public struct Enumerator : IEnumerator<T>
    {
        private readonly T[]? _array;
        private readonly int _start;
        private readonly int _end; // cache Offset + Count, since it's a little slow
        private int _current;

        internal Enumerator(ArraySegment<T> arraySegment)
        {
            Debug.Assert(arraySegment.Array != null);
            Debug.Assert(arraySegment.Offset >= 0);
            Debug.Assert(arraySegment.Count >= 0);
            Debug.Assert(arraySegment.Offset + arraySegment.Count <= arraySegment.Array.Length);

            _array = arraySegment.Array;
            _start = arraySegment.Offset;
            _end = arraySegment.Offset + arraySegment.Count;
            _current = arraySegment.Offset - 1;
        }

        public bool MoveNext()
        {
            if (_current < _end)
            {
                _current++;
                return _current < _end;
            }
            return false;
        }

        public T Current
        {
            get
            {
                if (_current < _start)
                    ThrowInvalidOperationException_InvalidOperation_EnumNotStarted();

                if (_current >= _end)
                    ThrowInvalidOperationException_InvalidOperation_EnumEnded();
                return _array![_current];
                
                [DoesNotReturn]
                static void ThrowInvalidOperationException_InvalidOperation_EnumNotStarted()
                {
                    throw new InvalidOperationException("Enumeration has not started");
                }
                
                [DoesNotReturn]
                static void ThrowInvalidOperationException_InvalidOperation_EnumEnded()
                {
                    throw new InvalidOperationException("Enumeration has ended");
                }
            }
        }

        object? IEnumerator.Current => Current;

        void IEnumerator.Reset()
        {
            _current = _start - 1;
        }

        public void Dispose()
        {
        }
    }

    public void Dispose()
    {
        if (Array != null)
        {
            _disposeFunc?.Invoke(Array, _context);
            Array = null;
            _context = null;
            _disposeFunc = null;
        }
    }
    
    public static implicit operator Memory<T>(DisposableArraySegment<T> segment) => segment.Array.AsMemory(segment.Offset, segment.Count);
    public static implicit operator ReadOnlyMemory<T>(DisposableArraySegment<T> segment) => segment.Array.AsMemory(segment.Offset, segment.Count);
    public static implicit operator Span<T>(DisposableArraySegment<T> segment) => segment.Array.AsSpan(segment.Offset, segment.Count);
    public static implicit operator ReadOnlySpan<T>(DisposableArraySegment<T> segment) => segment.Array.AsSpan(segment.Offset, segment.Count);
    public static implicit operator ArraySegment<T>(DisposableArraySegment<T> segment) => segment.Array != null ? new ArraySegment<T>(segment.Array, segment.Offset, segment.Count) : default;

    public ref T GetPinnableReference() => ref Array.AsSpan(Offset, Count).GetPinnableReference();
}