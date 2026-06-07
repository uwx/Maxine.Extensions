using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Maxine.Extensions.Io;

public sealed class StreamBufferWriter<T>(Stream stream, int bufferSize = 65536) : IBufferWriter<T> where T : struct
{
    private Memory<T> _slice;

    private Memory<T> SizeBuffer(int sizeHint)
    {
        if (_slice.IsEmpty)
        {
            _slice = ArrayPool<T>.Shared.Rent(Math.Max(bufferSize, sizeHint));
        }
        else if (_slice.Length < sizeHint)
        {
            var newBuffer = ArrayPool<T>.Shared.Rent(sizeHint);
            _slice.CopyTo(newBuffer);
            ArrayPool<T>.Shared.Return(MemoryMarshal.TryGetArray<T>(_slice, out var arraySegment) ? arraySegment.Array! : ThrowUnableToReturnBuffer());
            _slice = newBuffer;
        }
        
        return _slice;

        [DoesNotReturn]
        static T[] ThrowUnableToReturnBuffer()
        {
            throw new InvalidOperationException("Unable to return buffer to pool.");
        }
    }
    
    public void Advance(int count)
    {
        stream.Write(MemoryMarshal.AsBytes(_slice.Span[..count]));
        _slice = _slice[count..];
    }

    public Memory<T> GetMemory(int sizeHint = 0)
    {
        return SizeBuffer(sizeHint);
    }

    public Span<T> GetSpan(int sizeHint = 0)
    {
        return GetMemory(sizeHint).Span;
    }
}