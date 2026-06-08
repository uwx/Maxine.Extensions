using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Maxine.Extensions.Io;

public sealed class StreamArrayPooledReadOnlySequenceProvider : IDisposable
{
    private readonly ReadOnlySequence<byte> _sequence;
    private readonly ArrayPool<byte> _pool;

    public ReadOnlySequence<byte> Sequence => _sequence;

    public StreamArrayPooledReadOnlySequenceProvider(Stream stream, ArrayPool<byte>? pool = null, int bufferSize = 1024 * 1024)
    {
        var segments = new List<(byte[] array, int length)>();
        
        pool ??= ArrayPool<byte>.Shared;
        while (true)
        {
            var buffer = pool.Rent(bufferSize);
            var bytesRead = stream.Read(buffer, 0, bufferSize);

            if (bytesRead == 0)
            {
                pool.Return(buffer);
                break;
            }

            segments.Add((buffer, bytesRead));
        }
        
        _sequence = BuildSequence(segments);
        _pool = pool;
    }

    private sealed class PooledSegment : ReadOnlySequenceSegment<byte>
    {
        public PooledSegment(byte[] array, int length, long runningIndex)
        {
            Memory = array.AsMemory(0, length);
            RunningIndex = runningIndex;
        }

        public PooledSegment Append(byte[] array, int length)
        {
            var next = new PooledSegment(array, length, RunningIndex + Memory.Length);
            Next = next;
            return next;
        }
    }

    private static ReadOnlySequence<byte> BuildSequence(List<(byte[] array, int length)> segments)
    {
        if (segments.Count == 0)
            return ReadOnlySequence<byte>.Empty;

        if (segments.Count == 1)
        {
            var (arr, len) = segments[0];

            // Single segment: wrap directly (still needs manual return later)
            return new ReadOnlySequence<byte>(arr, 0, len);
        }

        var (firstArr, firstLen) = segments[0];
        var first = new PooledSegment(firstArr, firstLen, 0);
        var last = first;

        for (int i = 1; i < segments.Count; i++)
        {
            var (arr, len) = segments[i];
            last = last.Append(arr, len);
        }

        return new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);
    }

    public void Dispose()
    {
        if (_sequence.IsEmpty) return;

        if (_sequence.IsSingleSegment)
        {
            var arr = MemoryMarshal.TryGetArray(_sequence.First, out var segment) ? segment.Array : null;
            if (arr != null)
            {
                _pool.Return(arr);
            }
            else
            {
                ThrowInvalidStateSingleSegment();
            }
        }
        else
        {
            foreach (var memory in _sequence)
            {
                var arr = MemoryMarshal.TryGetArray(memory, out var arraySegment) ? arraySegment.Array : null;
                if (arr != null)
                {
                    _pool.Return(arr);
                }
                else
                {
                    ThrowInvalidStateMultiSegment();
                }
            }
        }

        [DoesNotReturn]
        static void ThrowInvalidStateSingleSegment()
        {
            throw new InvalidOperationException("Expected a single segment in the sequence, but it was not found.");
        }

        [DoesNotReturn]
        static void ThrowInvalidStateMultiSegment()
        {
            throw new InvalidOperationException("Expected multiple segments in the sequence, but at least one segment was not found.");
        }
    }
}