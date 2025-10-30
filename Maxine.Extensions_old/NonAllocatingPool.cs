#if ON
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WadArchiveJsonRenderer;

public abstract class NonAllocatingPool<T> : MemoryPool<T>
{
    public override int MaxBufferSize => int.MaxValue;

    public new static Impl Shared { get; } = new();

    protected override void Dispose(bool disposing) { }

    public override IMemoryOwner<T> Rent(int minBufferSize = -1) => RentCore(minBufferSize);

    private static Rental RentCore(int minBufferSize) => new(minBufferSize);

    public sealed class Impl : NonAllocatingPool<T>
    {
        // Typed to return the actual type rather than the
        // interface to avoid boxing, like how List<T>.GetEnumerator()
        // returns List<T>.Enumerator instead of IEnumerator<T>.
        public new Rental Rent(int minBufferSize) => RentCore(minBufferSize);
    }

    // Struct implements the interface so it can be boxed if necessary.
    public struct Rental : IMemoryOwner<T>
    {
        private T[] _array;

        public Rental(int minBufferSize)
        {
            _array = ArrayPool<T>.Shared.Rent(minBufferSize);
        }

        public Memory<T> Memory
        {
            get
            {
                if (_array == null)
                    throw new ObjectDisposedException(...);

                return new Memory<T>(_array);
            }
        }

        public void Dispose()
        {
            if (_array != null)
            {
                ArrayPool<T>.Shared.Return(_array);
                _array = null!;
            }
        }
    }
}

public abstract class UnmanagedBufferPool<T> : MemoryPool<T>
{
    public override int MaxBufferSize => int.MaxValue;

    public new static Impl Shared { get; } = new();

    protected override void Dispose(bool disposing) { }

    private protected UnmanagedBufferAllocator _allocator = new(UnmanagedBufferAllocator.DefaultBlockSize);

    public override IMemoryOwner<T> Rent(int minBufferSize = -1) => RentCore(minBufferSize, ref _allocator);

    private static Rental RentCore(int minBufferSize, ref UnmanagedBufferAllocator allocator) => new(minBufferSize, ref allocator);

    public sealed class Impl : UnmanagedBufferPool<T>
    {
        // Typed to return the actual type rather than the
        // interface to avoid boxing, like how List<T>.GetEnumerator()
        // returns List<T>.Enumerator instead of IEnumerator<T>.
        public new Rental Rent(int minBufferSize) => RentCore(minBufferSize, ref _allocator);
    }

    // Struct implements the interface so it can be boxed if necessary.
    public struct Rental : IMemoryOwner<T>
    {
        private T[] _array;

        internal Rental(int minBufferSize, ref UnmanagedBufferAllocator allocator)
        {
            _array = allocator.AllocAsSpan<>()
        }

        public Memory<T> Memory
        {
            get
            {
                if (_array == null)
                    throw new ObjectDisposedException(...);

                return new Memory<T>(_array);
            }
        }

        public void Dispose()
        {
            if (_array != null)
            {
                ArrayPool<T>.Shared.Return(_array);
                _array = null!;
            }
        }
    }
}

/// <summary>
/// A MemoryManager over a raw pointer
/// </summary>
/// <remarks>The pointer is assumed to be fully unmanaged, or externally pinned - no attempt will be made to pin this data</remarks>
public sealed unsafe class UnmanagedMemoryManager<T> : MemoryManager<T>
    where T : unmanaged
{
    private readonly T* _pointer;
    private readonly int _length;

    /// <summary>
    /// Create a new UnmanagedMemoryManager instance at the given pointer and size
    /// </summary>
    /// <remarks>It is assumed that the span provided is already unmanaged or externally pinned</remarks>
    public UnmanagedMemoryManager(Span<T> span)
    {
        fixed (T* ptr = &MemoryMarshal.GetReference(span))
        {
            _pointer = ptr;
            _length = span.Length;
        }
    }
    /// <summary>
    /// Create a new UnmanagedMemoryManager instance at the given pointer and size
    /// </summary>
    public UnmanagedMemoryManager(T* pointer, int length)
    {
        if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
        _pointer = pointer;
        _length = length;
    }
    /// <summary>
    /// Obtains a span that represents the region
    /// </summary>
    public override Span<T> GetSpan() => new(_pointer, _length);

    /// <summary>
    /// Provides access to a pointer that represents the data (note: no actual pin occurs)
    /// </summary>
    public override MemoryHandle Pin(int elementIndex = 0)
    {
        if (elementIndex < 0 || elementIndex >= _length)
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        return new MemoryHandle(_pointer + elementIndex);
    }
    /// <summary>
    /// Has no effect
    /// </summary>
    public override void Unpin() { }

    /// <summary>
    /// Releases all resources associated with this object
    /// </summary>
    protected override void Dispose(bool disposing) { }
}

#endif