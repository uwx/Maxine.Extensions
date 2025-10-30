namespace Maxine.Extensions;

public sealed class NativeMemoryStream : UnmanagedMemoryStream
{
    private readonly bool _leaveOpen;
    private NativeMemoryHandle _handle;
    public ReadOnlySpan<byte> Span => _handle.Span[..(int)Position];
    
    public unsafe NativeMemoryStream(int allocatedSize)
    {
        _handle = new NativeMemoryHandle((nuint)allocatedSize);
        Initialize(_handle, 0L, _handle.ByteLength, FileAccess.Write);
    }

    public unsafe NativeMemoryStream(NativeMemoryHandle existingHandle, bool leaveOpen = false)
    {
        _leaveOpen = leaveOpen;
        _handle = existingHandle;
        Initialize(_handle, 0L, _handle.ByteLength, FileAccess.Write);
    }

    ~NativeMemoryStream()
    {
        if (!_handle.IsDisposed)
        {
            Console.WriteLine($"Leaked {nameof(NativeMemoryStream)}.");
            Dispose(true);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!_leaveOpen)
        {
            _handle.Dispose();
            _handle = default;
        }

        base.Dispose(disposing);
    }

    public override ValueTask DisposeAsync()
    {
        if (!_leaveOpen)
        {
            _handle.Dispose();
            _handle = default;
        }
        
        return base.DisposeAsync();
    }
}