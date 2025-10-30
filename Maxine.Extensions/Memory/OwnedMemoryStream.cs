using System.Buffers;

namespace Maxine.Extensions;

public sealed class OwnedMemoryStream : UnmanagedMemoryStream
{
    private readonly bool _leaveOpen;
    private IMemoryOwner<byte>? _owner;
    private MemoryHandle _handle;
    public ReadOnlySpan<byte> Span => _owner.Memory.Span[..(int)Position];

    public unsafe OwnedMemoryStream(IMemoryOwner<byte> owner, bool leaveOpen = false)
    {
        _leaveOpen = leaveOpen;
        _owner = owner;
        _handle = _owner.Memory.Pin();
        Initialize((byte*)_handle.Pointer, 0L, _owner.Memory.Length, FileAccess.ReadWrite);
    }

    protected override void Dispose(bool disposing)
    {
        _handle.Dispose();
        
        if (!_leaveOpen)
        {
            _owner?.Dispose();
            _owner = null;
        }

        base.Dispose(disposing);
    }
}