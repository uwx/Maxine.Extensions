using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Maxine.Extensions;

// A MemoryStream represents a Stream in memory (ie, it has no backing store).
// This stream may reduce the need for temporary buffers and files in
// an application.
//
// There are two ways to create a MemoryStream.  You can initialize one
// from an unsigned byte array, or you can create an empty one.  Empty
// memory streams are resizable, while ones created with a byte array provide
// a stream "view" of the data.
public class MemoryStream2 : Stream
{
    private Memory<byte> _buffer;    // Either allocated internally or externally.
    private readonly int _origin;       // For user-provided arrays, start at this origin
    private int _position;     // read/write head.
    private int _length;       // Number of bytes within the memory stream
    private int _capacity;     // length of usable portion of buffer for stream
    // Note that _capacity == _buffer.Length for non-user-provided byte[]'s

    private bool _writable;    // Can user write to this stream?
    private readonly bool _exposable;   // Whether the array can be returned to the user.
    private bool _isOpen;      // Is this stream open or closed?

    private const int MemStreamMaxLength = int.MaxValue;

    public MemoryStream2(Memory<byte> buffer, bool writable = true)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        _buffer = buffer;
        _length = _capacity = buffer.Length;
        _writable = writable;
        _isOpen = true;
    }

    public MemoryStream2(Memory<byte> buffer, bool writable, bool publiclyVisible)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        _buffer = buffer;
        _origin = _position = 0;
        _length = _capacity = buffer.Length;
        _writable = writable;
        _exposable = publiclyVisible;  // Can TryGetBuffer/GetBuffer return the array?
        _isOpen = true;
    }

    public override bool CanRead => _isOpen;

    public override bool CanSeek => _isOpen;

    public override bool CanWrite => _writable;

    private void EnsureNotClosed()
    {
        if (!_isOpen)
            ThrowNewObjectDisposedException_StreamIsClosed();
        return;

        [DoesNotReturn]
        static void ThrowNewObjectDisposedException_StreamIsClosed()
        {
            throw new ObjectDisposedException("Stream is closed");
        }
    }

    private void EnsureWriteable()
    {
        if (!CanWrite)
            ThrowNewNotSupportedException_StreamNotWritable();
        return;

        [DoesNotReturn]
        static void ThrowNewNotSupportedException_StreamNotWritable()
        {
            throw new NotSupportedException("Stream is not writable");
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _isOpen = false;
            _writable = false;
            // Don't set buffer to null - allow TryGetBuffer, GetBuffer & ToArray to work.
        }
    }

    // returns a bool saying whether we allocated a new array.
    private bool EnsureCapacity(int value)
    {
        // Check for overflow
        if (value < 0)
            throw new IOException("Stream too long");

        if (value > _capacity)
        {
            throw new NotSupportedException("Stream is not growable");
        }
        return false;
    }

    public override void Flush()
    {
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        try
        {
            Flush();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public virtual Memory<byte> GetBuffer()
    {
        if (!_exposable)
            throw new UnauthorizedAccessException("Stream is not exposable.");
        return _buffer.Slice(_origin, _length - _origin);
    }

    public virtual bool TryGetBuffer(out Memory<byte> buffer)
    {
        if (!_exposable)
        {
            buffer = default;
            return false;
        }

        buffer = _buffer.Slice(_origin, _length - _origin);
        return true;
    }

    // PERF: Get actual length of bytes available for read; do sanity checks; shift position - i.e. everything except actual copying bytes
    internal int InternalEmulateRead(int count)
    {
        EnsureNotClosed();

        int n = _length - _position;
        if (n > count)
            n = count;
        if (n < 0)
            n = 0;

        Debug.Assert(_position + n >= 0);  // len is less than 2^31 -1.
        _position += n;
        return n;
    }

    // Gets & sets the capacity (number of bytes allocated) for this stream.
    // The capacity cannot be set to a value less than the current length
    // of the stream.
    //
    public virtual int Capacity
    {
        get
        {
            EnsureNotClosed();
            return _capacity - _origin;
        }
    }

    public override long Length
    {
        get
        {
            EnsureNotClosed();
            return _length - _origin;
        }
    }

    public override long Position
    {
        get
        {
            EnsureNotClosed();
            return _position - _origin;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            EnsureNotClosed();

            if (value > MemStreamMaxLength - _origin)
                throw new ArgumentOutOfRangeException(nameof(value), "Value is longer than the stream's max length");
            _position = _origin + (int)value;
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);
        EnsureNotClosed();

        int n = _length - _position;
        if (n > count)
            n = count;
        if (n <= 0)
            return 0;

        Debug.Assert(_position + n >= 0);  // len is less than 2^31 -1.

        _buffer.Slice(_position, n).CopyTo(buffer.AsMemory(offset, n));

        _position += n;

        return n;
    }

    public override int Read(Span<byte> buffer)
    {
        if (GetType() != typeof(MemoryStream))
        {
            // MemoryStream is not sealed, and a derived type may have overridden Read(byte[], int, int) prior
            // to this Read(Span<byte>) overload being introduced.  In that case, this Read(Span<byte>) overload
            // should use the behavior of Read(byte[],int,int) overload.
            return base.Read(buffer);
        }

        EnsureNotClosed();

        int n = Math.Min(_length - _position, buffer.Length);
        if (n <= 0)
            return 0;

        _buffer.Span.Slice(_position, n).CopyTo(buffer);

        _position += n;
        return n;
    }

    public override int ReadByte()
    {
        EnsureNotClosed();

        if (_position >= _length)
            return -1;

        return _buffer.Span[_position++];
    }

    public override void CopyTo(Stream destination, int bufferSize)
    {
        // If we have been inherited into a subclass, the following implementation could be incorrect
        // since it does not call through to Read() which a subclass might have overridden.
        // To be safe we will only use this implementation in cases where we know it is safe to do so,
        // and delegate to our base class (which will call into Read) when we are not sure.
        if (GetType() != typeof(MemoryStream))
        {
            base.CopyTo(destination, bufferSize);
            return;
        }

        // Validate the arguments the same way Stream does for back-compat.
        ValidateCopyToArguments(destination, bufferSize);
        EnsureNotClosed();

        int originalPosition = _position;

        // Seek to the end of the MemoryStream.
        int remaining = InternalEmulateRead(_length - originalPosition);

        // If we were already at or past the end, there's no copying to do so just quit.
        if (remaining > 0)
        {
            // Call Write() on the other Stream, using our internal buffer and avoiding any
            // intermediary allocations.
            destination.Write(_buffer.Span.Slice(originalPosition, remaining));
        }
    }

    public override long Seek(long offset, SeekOrigin loc)
    {
        EnsureNotClosed();

        return SeekCore(offset, loc switch
        {
            SeekOrigin.Begin => _origin,
            SeekOrigin.Current => _position,
            SeekOrigin.End => _length,
            _ => throw new ArgumentException("Invalid seek origin")
        });
    }

    private long SeekCore(long offset, int loc)
    {
        if (offset > MemStreamMaxLength - loc)
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset is greater than the stream's max length");
        int tempPosition = unchecked(loc + (int)offset);
        if (unchecked(loc + offset) < _origin || tempPosition < _origin)
            throw new IOException("Offset is smaller than the stream's beginning");
        _position = tempPosition;

        Debug.Assert(_position >= _origin);
        return _position - _origin;
    }

    // Sets the length of the stream to a given value.  The new
    // value must be nonnegative and less than the space remaining in
    // the array, int.MaxValue - origin
    // Origin is 0 in all cases other than a MemoryStream created on
    // top of an existing array and a specific starting offset was passed
    // into the MemoryStream constructor.  The upper bounds prevents any
    // situations where a stream may be created on top of an array then
    // the stream is made longer than the maximum possible length of the
    // array (int.MaxValue).
    //
    public override void SetLength(long value)
    {
        if (value < 0 || value > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(value), "Offset is greater than the stream's max length");

        EnsureWriteable();

        // Origin wasn't publicly exposed above.
        Debug.Assert(MemStreamMaxLength == int.MaxValue);  // Check parameter validation logic in this method if this fails.
        if (value > (int.MaxValue - _origin))
            throw new ArgumentOutOfRangeException(nameof(value), "Offset is greater than the stream's max length");

        int newLength = _origin + (int)value;
        bool allocatedNewArray = EnsureCapacity(newLength);
        if (!allocatedNewArray && newLength > _length)
            _buffer.Span.Slice(_length, newLength - _length).Clear();
        _length = newLength;
        if (_position > newLength)
            _position = newLength;
    }

    public virtual byte[] ToArray()
    {
        int count = _length - _origin;
        if (count == 0)
            return Array.Empty<byte>();
        byte[] copy = GC.AllocateUninitializedArray<byte>(count);
        _buffer.Span.Slice(_origin, count).CopyTo(copy);
        return copy;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);
        EnsureNotClosed();
        EnsureWriteable();

        int i = _position + count;
        // Check for overflow
        if (i < 0)
            throw new IOException("Stream is too long");

        if (i > _length)
        {
            bool mustZero = _position > _length;
            if (i > _capacity)
            {
                bool allocatedNewArray = EnsureCapacity(i);
                if (allocatedNewArray)
                {
                    mustZero = false;
                }
            }
            if (mustZero)
            {
                _buffer.Span.Slice(_length, i - _length).Clear();
            }
            _length = i;
        }
        
        buffer.AsSpan(offset, count).CopyTo(_buffer.Span.Slice(_position, count));
        
        _position = i;
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (GetType() != typeof(MemoryStream))
        {
            // MemoryStream is not sealed, and a derived type may have overridden Write(byte[], int, int) prior
            // to this Write(Span<byte>) overload being introduced.  In that case, this Write(Span<byte>) overload
            // should use the behavior of Write(byte[],int,int) overload.
            base.Write(buffer);
            return;
        }

        EnsureNotClosed();
        EnsureWriteable();

        // Check for overflow
        int i = _position + buffer.Length;
        if (i < 0)
            throw new IOException("Stream is too long");

        if (i > _length)
        {
            bool mustZero = _position > _length;
            if (i > _capacity)
            {
                bool allocatedNewArray = EnsureCapacity(i);
                if (allocatedNewArray)
                {
                    mustZero = false;
                }
            }
            if (mustZero)
            {
                _buffer.Span.Slice(_length, i - _length).Clear();
            }
            _length = i;
        }

        buffer.CopyTo(_buffer.Span.Slice(_position, buffer.Length));
        _position = i;
    }

    public override void WriteByte(byte value)
    {
        EnsureNotClosed();
        EnsureWriteable();

        if (_position >= _length)
        {
            int newLength = _position + 1;
            bool mustZero = _position > _length;
            if (newLength >= _capacity)
            {
                bool allocatedNewArray = EnsureCapacity(newLength);
                if (allocatedNewArray)
                {
                    mustZero = false;
                }
            }
            if (mustZero)
            {
                _buffer.Span.Slice(_length, _position - _length).Clear();
            }
            _length = newLength;
        }
        _buffer.Span[_position++] = value;
    }

    // Writes this MemoryStream to another stream.
    public virtual void WriteTo(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        EnsureNotClosed();

        stream.Write(_buffer.Span.Slice(_origin, _length - _origin));
    }
}