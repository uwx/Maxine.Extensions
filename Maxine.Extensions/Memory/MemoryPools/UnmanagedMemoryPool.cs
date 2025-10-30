using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Maxine.Extensions.MemoryPools;

public unsafe class UnmanagedMemoryPool<T>(int? blockSize = null) : MemoryPool<T>
    where T : unmanaged
{
    private const int DefaultBlockSizeBytes = 4096;
    private readonly int _blockSizeBytes = blockSize != null ? blockSize.Value * sizeof(T) : Math.Max(DefaultBlockSizeBytes / sizeof(T), 16);
    private readonly Queue<UnmanagedPooledMemoryManager<T>> _blocks = [];
    // private readonly HashSet<UnmanagedPooledMemoryManager<T>> _sanityBlocks = [];

    protected override void Dispose(bool disposing)
    {
        foreach (var manager in _blocks)
        {
            manager._memory.Dispose();
        }

        // var bad = false;
        // foreach (var manager in _sanityBlocks)
        // {
        //     if (!manager._memory.IsDisposed)
        //     {
        //         bad = true;
        //         manager._memory.Dispose();
        //     }
        // }
        //
        // if (bad)
        // {
        //     throw new InvalidOperationException("At least one block was not returned to the pool before the pool was disposed");
        // }
    }

    public override IMemoryOwner<T> Rent(int minBufferSize = -1)
    {
        var sizeBytes = minBufferSize == -1 ? DefaultBlockSizeBytes : (minBufferSize * sizeof(T));

        if (sizeBytes > _blockSizeBytes)
        {
            return new UnmanagedPooledMemoryManager<T>(this, sizeBytes);
        }

        if (_blocks.TryDequeue(out var block))
        {
            return block;
        }

        var instance = new UnmanagedPooledMemoryManager<T>(this, _blockSizeBytes);
        // _sanityBlocks.Add(instance);
        return instance;
    }

    public override int MaxBufferSize => _blockSizeBytes * sizeof(T);

    internal void Return(UnmanagedPooledMemoryManager<T> manager)
    {
        if (manager._memory.ByteLength > _blockSizeBytes)
        {
            manager._memory.Dispose();
        }
        else
        {
            _blocks.Enqueue(manager);
        }
    }
}

internal sealed unsafe class UnmanagedPooledMemoryManager<T> : MemoryManager<T>
    where T : unmanaged
{
    private readonly UnmanagedMemoryPool<T> _pool;
    internal NativeMemoryHandle _memory;
    private bool _pinned;
    private bool _disposed;

    internal UnmanagedPooledMemoryManager(UnmanagedMemoryPool<T> pool, int length)
    {
        _pool = pool;

        _memory = new NativeMemoryHandle((UIntPtr)length * (UIntPtr)sizeof(T));
    }

    ~UnmanagedPooledMemoryManager()
    {
        if (!_disposed)
        {
            Dispose(true);
            Console.WriteLine($"Leaked {nameof(UnmanagedPooledMemoryManager<T>)}.");
        }
    }

    public override Span<T> GetSpan()
    {
        return MemoryMarshal.Cast<byte, T>(_memory.Span);
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
        _pinned = true;
        return new MemoryHandle(_memory.Pointer);
    }

    public override void Unpin()
    {
        _pinned = false;
    }
    
    protected override void Dispose(bool disposing)
    {
        if (_pinned)
        {
            AttemptedToDisposeWhilePinned();
        }
        _pool.Return(this);
        _disposed = true;
        
        return;

        static void AttemptedToDisposeWhilePinned()
        {
            throw new InvalidOperationException("Attempted to dispose while pinned");
        }
    } 
}