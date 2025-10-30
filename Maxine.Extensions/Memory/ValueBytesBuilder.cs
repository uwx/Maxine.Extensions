using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions;

public ref struct ValueBytesBuilder
{
    private byte[]? _arrayToReturnToPool;
    private Span<byte> _bytes;
    private int _pos;

    public ValueBytesBuilder(Span<byte> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _bytes = initialBuffer;
        _pos = 0;
    }

    public ValueBytesBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<byte>.Shared.Rent(initialCapacity);
        _bytes = _arrayToReturnToPool;
        _pos = 0;
    }

    public int Length
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _bytes.Length);
            _pos = value;
        }
    }

    public int Capacity => _bytes.Length;

    public void EnsureCapacity(int capacity)
    {
        // This is not expected to be called this with negative capacity
        Debug.Assert(capacity >= 0);

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if ((uint)capacity > (uint)_bytes.Length)
            Grow(capacity - _pos);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// Does not ensure there is a null byte after <see cref="Length"/>
    /// This overload is pattern matched in the C# 7.3+ compiler so you can omit
    /// the explicit method call, and write eg "fixed (byte* c = builder)"
    /// </summary>
    public ref byte GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(_bytes);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null byte after <see cref="Length"/></param>
    public ref byte GetPinnableReference(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _bytes[Length] = (byte)'\0';
        }
        return ref MemoryMarshal.GetReference(_bytes);
    }

    public ref byte this[int index]
    {
        get
        {
            Debug.Assert(index < _pos);
            return ref _bytes[index];
        }
    }

    public override string ToString()
    {
        var s = _bytes[.._pos].ToString();
        Dispose();
        return s;
    }

    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<byte> RawBytes => _bytes;

    /// <summary>
    /// Returns a span around the contents of the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null byte after <see cref="Length"/></param>
    public ReadOnlySpan<byte> AsSpan(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _bytes[Length] = (byte)'\0';
        }
        return _bytes[.._pos];
    }

    public ReadOnlySpan<byte> AsSpan() => _bytes[.._pos];
    public ReadOnlySpan<byte> AsSpan(int start) => _bytes.Slice(start, _pos - start);
    public ReadOnlySpan<byte> AsSpan(int start, int length) => _bytes.Slice(start, length);

    public bool TryCopyTo(Span<byte> destination, out int bytesWritten)
    {
        if (_bytes[.._pos].TryCopyTo(destination))
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

    public void Insert(int index, byte value, int count)
    {
        if (_pos > _bytes.Length - count)
        {
            Grow(count);
        }

        var remaining = _pos - index;
        _bytes.Slice(index, remaining).CopyTo(_bytes[(index + count)..]);
        _bytes.Slice(index, count).Fill(value);
        _pos += count;
    }

    public void Insert(int index, ReadOnlySpan<byte> s)
    {
        var count = s.Length;

        if (_pos > (_bytes.Length - count))
        {
            Grow(count);
        }

        var remaining = _pos - index;
        _bytes.Slice(index, remaining).CopyTo(_bytes[(index + count)..]);
        s.CopyTo(_bytes[index..]);
        _pos += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(byte c)
    {
        var pos = _pos;
        var bytes = _bytes;
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

    public void Append(byte c, int count)
    {
        if (_pos > _bytes.Length - count)
        {
            Grow(count);
        }

        var dst = _bytes.Slice(_pos, count);
        for (var i = 0; i < dst.Length; i++)
        {
            dst[i] = c;
        }
        _pos += count;
    }

    public unsafe void Append(byte* value, int length)
    {
        var pos = _pos;
        if (pos > _bytes.Length - length)
        {
            Grow(length);
        }

        var dst = _bytes.Slice(_pos, length);
        for (var i = 0; i < dst.Length; i++)
        {
            dst[i] = *value++;
        }
        _pos += length;
    }

    public void Append(ReadOnlySpan<byte> value)
    {
        var pos = _pos;
        if (pos > _bytes.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_bytes[_pos..]);
        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> AppendSpan(int length)
    {
        var origPos = _pos;
        if (origPos > _bytes.Length - length)
        {
            Grow(length);
        }

        _pos = origPos + length;
        return _bytes.Slice(origPos, length);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(byte c)
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
        Debug.Assert(_pos > _bytes.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
        // to double the size if possible, bounding the doubling to not go beyond the max array length.
        var newCapacity = Math.Max(
            (_pos + additionalCapacityBeyondPos),
            Math.Min(_bytes.Length * 2, ArrayMaxLength));

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative.
        // This could also go negative if the actual required length wraps around.
        var poolArray = ArrayPool<byte>.Shared.Rent((int)newCapacity);

        _bytes[.._pos].CopyTo(poolArray);

        var toReturn = _arrayToReturnToPool;
        _bytes = _arrayToReturnToPool = poolArray;
        if (toReturn != null)
        {
            ArrayPool<byte>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        var toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if (toReturn != null)
        {
            ArrayPool<byte>.Shared.Return(toReturn);
        }
    }

    public void Clear()
    {
        _pos = 0;
    }
}

