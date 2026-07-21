using System.Buffers;

namespace Maxine.Extensions.Collections;

/// <summary>
/// An <see cref="ArrayPool{T}"/> wrapper that returns disposable array segments.
/// </summary>
/// <param name="backingPool">The backing <see cref="ArrayPool{T}"/>.</param>
/// <typeparam name="T">The type of element stored in the arrays.</typeparam>
public readonly struct SafeArrayPool<T>(ArrayPool<T> backingPool)
{
    public static SafeArrayPool<T> Shared => new SafeArrayPool<T>(ArrayPool<T>.Shared);
    
    public DisposableArraySegment<T> Rent(int minimumLength)
    {
        return new DisposableArraySegment<T>(backingPool.Rent(minimumLength), 0, minimumLength, static (arr, pool) => ((ArrayPool<T>)pool!).Return(arr), backingPool);
    }
}

public struct DisposableArraySegment<T>(T[]? array, int offset, int count, Action<T[], object?>? disposeFunc = null, object? context = null) : IDisposable
{
    private Action<T[], object?>? _disposeFunc = disposeFunc;
    private object? _context = context;
    
    public T[]? Array { get; private set; } = array;
    public int Offset { get; } = offset;
    public int Count { get; } = count;

    public readonly Span<T> AsSpan(int offset, int count) => Array.AsSpan(Offset, Count).Slice(offset, count);

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
}