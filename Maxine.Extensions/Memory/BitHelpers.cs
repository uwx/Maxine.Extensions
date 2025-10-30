using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions;

public static class BitHelpers
{
    public static Span<T> AsSpan<T>(scoped ref T value)
    {
        return MemoryMarshal.CreateSpan(ref value, 1);
    }

    public static ReadOnlySpan<T> AsSpan<T>(T value)
    {
        return MemoryMarshal.CreateReadOnlySpan(ref value, 1);
    }

    public static Span<byte> AsBytes<T>(this Span<T> span) where T : struct
    {
        return MemoryMarshal.AsBytes(span);
    }

    public static ReadOnlySpan<byte> AsBytes<T>(this  ReadOnlySpan<T> span) where T : struct
    {
        return MemoryMarshal.AsBytes(span);
    }

    public static ReadOnlySpan<byte> GetBytes<T>(scoped ref T value) where T : struct
    {
        return AsSpan(ref value).AsBytes();
    }

    public static ReadOnlySpan<byte> GetBytes<T>(T value) where T : struct
    {
        return AsSpan(ref value).AsBytes();
    }
}