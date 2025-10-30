using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions;

public ref struct ValueArrayBuilder<T>
#if NET9_0_OR_GREATER
    : IBufferWriter<T>, IDisposable
#endif
{
    private T[]? _arrayToReturnToPool;
    private Span<T> _span;
    private int _pos;

    public ValueArrayBuilder(Span<T> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _span = initialBuffer;
        _pos = 0;
    }

    public ValueArrayBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(initialCapacity);
        _span = _arrayToReturnToPool;
        _pos = 0;
    }

    public int Length
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _span.Length);
            _pos = value;
        }
    }

    public int Capacity => _span.Length;

    public void EnsureCapacity(int capacity)
    {
        // This is not expected to be called this with negative capacity
        Debug.Assert(capacity >= 0);

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if ((uint)capacity > (uint)_span.Length)
            Grow(capacity - _pos);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// Does not ensure there is a null T after <see cref="Length"/>
    /// This overload is pattern matched in the C# 7.3+ compiler so you can omit
    /// the explicit method call, and write eg "fixed (T* c = builder)"
    /// </summary>
    public ref T GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(_span);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null T after <see cref="Length"/></param>
    public ref T GetPinnableReference(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _span[Length] = default!;
        }
        return ref MemoryMarshal.GetReference(_span);
    }

    public ref T this[int index]
    {
        get
        {
            Debug.Assert(index < _pos);
            return ref _span[index];
        }
    }

    public override string ToString()
    {
        return _span[.._pos].ToString();
    }

    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<T> RawSpan => _span;

    /// <summary>
    /// Returns a span around the contents of the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a default(T) after <see cref="Length"/></param>
    public ReadOnlySpan<T> AsSpan(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _span[Length] = default!;
        }
        return _span[.._pos];
    }

    public ReadOnlySpan<T> AsSpan() => _span[.._pos];
    public ReadOnlySpan<T> AsSpan(int start) => _span.Slice(start, _pos - start);
    public ReadOnlySpan<T> AsSpan(int start, int length) => _span.Slice(start, length);

    public T[] ToArrayAndDispose()
    {
        var arr = AsSpan().ToArray();
        Dispose();
        return arr;
    }

    public bool TryCopyTo(Span<T> destination, out int bytesWritten)
    {
        if (_span[.._pos].TryCopyTo(destination))
        {
            bytesWritten = _pos;
            Dispose();
            return true;
        }
        else
        {
            bytesWritten = 0;
            Dispose();
            return false;
        }
    }

    public void Insert(int index, T value, int count)
    {
        if (_pos > _span.Length - count)
        {
            Grow(count);
        }

        var remaining = _pos - index;
        _span.Slice(index, remaining).CopyTo(_span[(index + count)..]);
        _span.Slice(index, count).Fill(value);
        _pos += count;
    }

    public void Insert(int index, ReadOnlySpan<T> s)
    {
        var count = s.Length;

        if (_pos > (_span.Length - count))
        {
            Grow(count);
        }

        var remaining = _pos - index;
        _span.Slice(index, remaining).CopyTo(_span[(index + count)..]);
        s.CopyTo(_span[index..]);
        _pos += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(T c)
    {
        var pos = _pos;
        var bytes = _span;
        if ((uint)pos < (uint)bytes.Length)
        {
            bytes[pos] = c;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    public void Append(T c, int count)
    {
        if (_pos > _span.Length - count)
        {
            Grow(count);
        }

        var dst = _span.Slice(_pos, count);
        for (var i = 0; i < dst.Length; i++)
        {
            dst[i] = c;
        }
        _pos += count;
    }

    public unsafe void Append(T* value, int length)
    {
        var pos = _pos;
        if (pos > _span.Length - length)
        {
            Grow(length);
        }

        var dst = _span.Slice(_pos, length);
        for (var i = 0; i < dst.Length; i++)
        {
            dst[i] = *value++;
        }
        _pos += length;
    }

    public void Append(scoped ReadOnlySpan<T> value)
    {
        var pos = _pos;
        if (pos > _span.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_span[_pos..]);
        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AppendSpan(int length)
    {
        var origPos = _pos;
        if (origPos > _span.Length - length)
        {
            Grow(length);
        }

        _pos = origPos + length;
        return _span.Slice(origPos, length);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(T c)
    {
        Grow(1);
        Append(c);
    }

    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_pos"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of bytes requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _span.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
        // to double the size if possible, bounding the doubling to not go beyond the max array length.
        var newCapacity = Math.Max(
            (_pos + additionalCapacityBeyondPos),
            Math.Min(_span.Length * 2, ArrayMaxLength));

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative.
        // This could also go negative if the actual required length wraps around.
        var poolArray = ArrayPool<T>.Shared.Rent((int)newCapacity);

        _span[.._pos].CopyTo(poolArray);

        var toReturn = _arrayToReturnToPool;
        _span = _arrayToReturnToPool = poolArray;
        if (toReturn != null)
        {
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        var toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if (toReturn != null)
        {
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }

    public void Clear()
    {
        _pos = 0;
    }

    public void Shrink(int count)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, _pos, nameof(count));
        _pos -= count;
    }

    #region IBufferWriter<T> impl
    
    private const int DefaultMinimumBufferWriterBufferSize = 1;
    // NB about IBufferWriter<T> implementation: if Advance is not called and Grow() is called, anything after _pos will
    // be erased. The interface appears to allow this.
    
    public void Advance(int count)
    {
        if (_pos > _span.Length - count)
        {
            Grow(count);
        }

        _pos += count;
    }

    public Memory<T> GetMemory(int sizeHint = 0)
    {
        if (sizeHint == 0)
        {
            sizeHint = Math.Max(DefaultMinimumBufferWriterBufferSize, _span.Length - _pos);
        }

        if (_pos > _span.Length - sizeHint)
        {
            Grow(sizeHint);
        }

        // force creation of an array that can be wrapped into a Memory<T>, if one does not exist yet.
        if (_arrayToReturnToPool == null)
        {
            var poolArray = ArrayPool<T>.Shared.Rent(_span.Length);
            _span[.._pos].CopyTo(poolArray);
            _span = _arrayToReturnToPool = poolArray;
        }

        return _arrayToReturnToPool.AsMemory(_pos);
    }

    public Span<T> GetSpan(int sizeHint = 0)
    {
        if (sizeHint == 0)
        {
            sizeHint = Math.Max(DefaultMinimumBufferWriterBufferSize, _span.Length - _pos);
        }
        
        if (_pos > _span.Length - sizeHint)
        {
            Grow(sizeHint);
        }

        return _span[_pos..];
    }
    
    #endregion
}