using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions;

public unsafe struct NativeMemoryHandle : IDisposable
{
    private readonly void* _handle;
    
    public void* Pointer => CheckDisposed() ? _handle : null;

    public IntPtr SafePointer => CheckDisposed() ? (nint)_handle : IntPtr.Zero;

    public long ByteLength { get; }

    public Span<byte> Span => CheckDisposed() ? new(_handle, (int)ByteLength) : default;

    public bool IsDisposed => _handle == null;

    public NativeMemoryHandle(nuint size)
    {
        ByteLength = (long)size;
        _handle = NativeMemory.Alloc(size);
    }

    public void Dispose()
    {
        NativeMemory.Free(_handle);
        this = default;
    }

    public static implicit operator void*(NativeMemoryHandle h) => h._handle;
    public static implicit operator IntPtr(NativeMemoryHandle h) => (nint)h._handle;
    public static implicit operator byte*(NativeMemoryHandle h) => (byte*)h._handle;

    public UnmanagedMemoryStream OpenStream()
    {
        return new UnmanagedMemoryStream(this, 0, ByteLength, FileAccess.Write);
    }

    public Span<byte> GetStreamSpan(UnmanagedMemoryStream stream)
    {
        return Span[..(int)stream.Position];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CheckDisposed()
    {
        if (_handle == null)
        {
            Throw();
        }
        
        return true;

        [DoesNotReturn]
        static void Throw()
        {
            throw new ObjectDisposedException(nameof(NativeMemoryHandle), "Handle is disposed");
        }
    }
}