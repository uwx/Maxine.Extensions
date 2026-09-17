// Copyright (c) 2023, Alexandre Mutel
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification
// , are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice, this 
//    list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice, 
//    this list of conditions and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Maxine.Extensions;

/// <summary>
/// Represents a fixed-length string of maximum <c>sizeof(<see cref="TMemory"/>)</c> characters.
/// </summary>
[InterpolatedStringHandler]
public struct FixedStringUtf8<TMemory> : IUtf8SpanFormattable, IEquatable<FixedStringUtf8<TMemory>> where TMemory : unmanaged
{
    public static unsafe int MaxLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => sizeof(TMemory);
    }

    private short _length;
    private TMemory _memory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedString{T}"/> struct.
    /// </summary>
    public FixedStringUtf8(int literalLength, int formattedCount)
    {
        _length = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedString{T}"/> struct.
    /// </summary>
    public FixedStringUtf8(string value)
    {
        _length = 0;
        AppendLiteral(value);
    }

    /// <summary>
    /// Gets the number of characters in the string. The length is always less than or equal to <see cref="MaxLength"/>.
    /// </summary>
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    /// <summary>
    /// Resets this string to an empty string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _length = 0;

    /// <summary>
    /// Appends the string literal to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendLiteral(ReadOnlySpan<char> s)
    {
        if (_length == MaxLength) return;

        var span = AsRemainingSpan();
        Encoding.UTF8.TryGetBytes(s, span, out var bytesWritten);
        _length += (short)bytesWritten;
    }

    /// <summary>
    /// Apppends the specified string to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(string t) => AppendLiteral(t);

    /// <summary>
    /// Apppends the specified string to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(string t, int alignment)
    {
        var startPosition = _length;
        AppendFormatted(t);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T t) where T : IUtf8SpanFormattable
    {
        var span = AsRemainingSpan();
        t.TryFormat(span, out int bytesWritten, new ReadOnlySpan<char>(), null);
        _length += (short)bytesWritten;
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, string? format) where T : IUtf8SpanFormattable
    {
        var span = AsRemainingSpan();
        value.TryFormat(span, out int bytesWritten, format, null);
        _length += (short)bytesWritten;
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, int alignment) where T : IUtf8SpanFormattable
    {
        var startPosition = _length;
        AppendFormatted(value);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <summary>
    /// Apppends the formatted value to this fixed string.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, int alignment, string? format) where T : IUtf8SpanFormattable
    {
        var startPosition = _length;
        AppendFormatted(value, format);
        AppendOrInsertAlignment(startPosition, alignment);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(FixedStringUtf8<TMemory> other) => _length == other._length && AsSpan().SequenceEqual(other.AsSpan());

    /// <inheritedoc />
    public override bool Equals(object? obj) => obj is FixedStringUtf8<TMemory> other && Equals(other);

    /// <inheritedoc />
    public override int GetHashCode()
    {
        // Compute the FNV-1a hash of the string
        int hash = unchecked((int)2166136261);
        foreach (var c in AsSpan())
        {
            hash ^= c;
            hash *= 16777619;
        }

        return hash;
    }

    /// <summary>
    /// Returns a span of characters that contains the characters of this string.
    /// </summary>
    /// <returns>A span of characters that contains the characters of this string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnscopedRef]
    public readonly ReadOnlySpan<byte> AsSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<TMemory, byte>(ref Unsafe.AsRef(in _memory)), _length);

    /// <summary>
    /// Implicit conversion from <see cref="string"/> to <see cref="FixedString{T}"/>.
    /// </summary>
    [SkipLocalsInit]    
    public static implicit operator FixedStringUtf8<TMemory>(string s) => new(s);

    /// <inheritedoc />
    public readonly override string ToString() => ToString(null, null);

    /// <inheritedoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string? format, IFormatProvider? formatProvider) => _length == 0 ? string.Empty : Encoding.UTF8.GetString(AsSpan());

    /// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly bool TryFormat(Span<byte> destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (destination.Length < _length)
        {
            bytesWritten = 0;
            return false;
        }
        AsSpan().CopyTo(destination);
        bytesWritten = _length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<byte> AsUnsafeFullSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<TMemory, byte>(ref _memory), MaxLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnscopedRef]
    private Span<byte> AsRemainingSpan() => AsUnsafeFullSpan().Slice(_length);

    /// <summary>
    /// Appends or inserts the specified alignment at the specified position.
    /// </summary>
    private void AppendOrInsertAlignment(int startPosition, int alignment)
    {
        if (alignment == 0) return;

        int length = _length - startPosition;
        bool padAfter = false;
        if (alignment < 0)
        {
            padAfter = true;
            alignment = -alignment;
        }

        int numberOfCharsToAppendOrInsert = alignment - length;
        if (numberOfCharsToAppendOrInsert <= 0) return;

        if (padAfter)
        {
            numberOfCharsToAppendOrInsert = Math.Min(MaxLength, _length + numberOfCharsToAppendOrInsert) - _length;

            if (numberOfCharsToAppendOrInsert > 0)
            {
                AsRemainingSpan().Slice(0, numberOfCharsToAppendOrInsert).Fill((byte)' ');
                _length += (short)numberOfCharsToAppendOrInsert;
            }
        }
        else 
        {
            numberOfCharsToAppendOrInsert = Math.Min(MaxLength, startPosition + numberOfCharsToAppendOrInsert) - startPosition;

            if (numberOfCharsToAppendOrInsert > 0)
            {
                var endPositionFill = startPosition + numberOfCharsToAppendOrInsert;

                var span = AsUnsafeFullSpan();
                if (endPositionFill < MaxLength)
                {
                    var maxLengthToCopy = Math.Min(MaxLength, endPositionFill + length) - endPositionFill;
                    if (maxLengthToCopy > 0)
                    {
                        span.Slice(startPosition, maxLengthToCopy).CopyTo(span.Slice(endPositionFill, maxLengthToCopy));
                    }
                }

                span.Slice(startPosition, numberOfCharsToAppendOrInsert).Fill((byte)' ');
                _length = (short)Math.Min(endPositionFill + length, MaxLength);
            }
        }
    }

    public static bool operator ==(FixedStringUtf8<TMemory> left, FixedStringUtf8<TMemory> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FixedStringUtf8<TMemory> left, FixedStringUtf8<TMemory> right)
    {
        return !(left == right);
    }
}