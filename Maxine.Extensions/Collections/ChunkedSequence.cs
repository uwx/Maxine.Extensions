using System.Buffers;

namespace Maxine.Extensions.Collections;

public ref struct ChunkedSequence<T>
{
    private ReadOnlyChunk? _first;
    private ReadOnlyChunk? _current;

    private bool _changed;
    private ReadOnlySequence<T>? _sequence;

    public ChunkedSequence()
    {
        _first = _current = null;
        _sequence = null;
        _changed = false;
    }

    public ChunkedSequence(ReadOnlySequence<T> sequence) : this()
    {
        Append(sequence);
    }

    public ChunkedSequence<T> Append(ReadOnlySequence<T> sequence)
    {
        var pos = sequence.Start;
        while (sequence.TryGet(ref pos, out var mem, true))
        {
            Append(mem);
        }

        return this;
    }

    public ChunkedSequence<T> Append(ReadOnlyMemory<T> memory)
    {
        if (_current == null)
        {
            _first = _current = new ReadOnlyChunk(memory);
        }
        else
        {
            _current = _current.Append(memory);
        }

        _changed = true;
        return this;
    }

    internal ReadOnlySequence<T> GetSequence()
    {
        if (_changed)
        {
            _sequence = new ReadOnlySequence<T>(_first, 0, _current, _current.Memory.Length);
        }
        else _sequence ??= new ReadOnlySequence<T>();

        return _sequence.Value;
    }

    public static implicit operator ReadOnlySequence<T>(ChunkedSequence<T> sequence)
    {
        return sequence.GetSequence();
    }

    private sealed class ReadOnlyChunk : ReadOnlySequenceSegment<T>
    {
        public ReadOnlyChunk(ReadOnlyMemory<T> memory)
        {
            Memory = memory;
        }

        public ReadOnlyChunk Append(ReadOnlyMemory<T> memory)
        {
            var nextChunk = new ReadOnlyChunk(memory)
            {
                RunningIndex = RunningIndex + Memory.Length
            };

            Next = nextChunk;
            return nextChunk;
        }
    }
}
