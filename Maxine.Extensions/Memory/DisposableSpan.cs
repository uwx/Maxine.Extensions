using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

namespace Maxine.Extensions;

// # .NET Community Toolkit
// 
// Copyright © .NET Foundation and Contributors
// 
// All rights reserved.
// 
// ## MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Sample usage:
// DisposableSpan<T> s = x.Length < 15 ? stackalloc T[x.Length] : DisposableSpan<T>.Allocate(x.Length); 
public ref struct DisposableSpan<T>
#if NET9_0_OR_GREATER
    : IDisposable
#endif
{
    private Span<T> _span;
    private ArrayPool<T>? _pool;
    private T[]? _arrayToReturnToPool;

    public DisposableSpan(Span<T> span)
    {
        _span = span;
    }
    
    public DisposableSpan(int size, ArrayPool<T>? pool, AllocationMode mode)
    {
        _pool = pool ?? ArrayPool<T>.Shared;
        _arrayToReturnToPool = _pool.Rent(size);
        _span = _arrayToReturnToPool.AsSpan(0, size);
        
        if (mode == AllocationMode.Clear)
        {
            _span.Clear();
        }
    }
    
    // devirt constructor
    public DisposableSpan(int size, AllocationMode mode)
    {
        _pool = ArrayPool<T>.Shared;
        _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(size);
        _span = _arrayToReturnToPool.AsSpan(0, size);
        
        if (mode == AllocationMode.Clear)
        {
            _span.Clear();
        }
    }

    public void Dispose()
    {
        if (_arrayToReturnToPool is { } arrayToReturnToPool)
        {
            var pool = _pool;
            this = default;
            pool!.Return(arrayToReturnToPool);
        }
    }
    
    /// <summary>
    /// Gets an empty <see cref="SpanOwner{T}"/> instance.
    /// </summary>
    public static DisposableSpan<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Span<T>.Empty);
    }

    /// <summary>
    /// Creates a new <see cref="DisposableSpan{T}"/> instance with the specified parameters.
    /// </summary>
    /// <param name="size">The length of the new memory buffer to use.</param>
    /// <param name="mode">Indicates the allocation mode to use for the new buffer to rent.</param>
    /// <returns>A <see cref="DisposableSpan{T}"/> instance of the requested length.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size"/> is not valid.</exception>
    /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DisposableSpan<T> Allocate(int size, AllocationMode mode = AllocationMode.Default) => new(size, mode);

    /// <summary>
    /// Creates a new <see cref="DisposableSpan{T}"/> instance with the specified parameters.
    /// </summary>
    /// <param name="size">The length of the new memory buffer to use.</param>
    /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
    /// <param name="mode">Indicates the allocation mode to use for the new buffer to rent.</param>
    /// <returns>A <see cref="DisposableSpan{T}"/> instance of the requested length.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size"/> is not valid.</exception>
    /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DisposableSpan<T> Allocate(int size, ArrayPool<T> pool, AllocationMode mode = AllocationMode.Default) => new(size, pool, mode);

    /// <summary>
    /// Gets the number of items in the current instance
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> wrapping the memory belonging to the current instance.
    /// </summary>
    public Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span;
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _span[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(_span);
    }

    public static implicit operator DisposableSpan<T>(Span<T> span) => new(span);
}