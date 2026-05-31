
#nullable enable
using System.Runtime.CompilerServices;
using System.Text;

namespace Maxine.Extensions.Collections;

/// <typeparam name="T" />
[InlineArray(2)]
public struct InlineArray2Ex<T> : IEquatable<InlineArray2Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray2<T>(in InlineArray2Ex<T> array)
        => Unsafe.As<InlineArray2Ex<T>, InlineArray2<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray2Ex<T>(in InlineArray2<T> array)
        => Unsafe.As<InlineArray2<T>, InlineArray2Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 2; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this[0], this[1]);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray2Ex<T> other)
            return false;
        for (var i = 0; i < 2; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray2Ex<T> other)
    {
        for (var i = 0; i < 2; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 2; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 2; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(3)]
public struct InlineArray3Ex<T> : IEquatable<InlineArray3Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray3<T>(in InlineArray3Ex<T> array)
        => Unsafe.As<InlineArray3Ex<T>, InlineArray3<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray3Ex<T>(in InlineArray3<T> array)
        => Unsafe.As<InlineArray3<T>, InlineArray3Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 3; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this[0], this[1], this[2]);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray3Ex<T> other)
            return false;
        for (var i = 0; i < 3; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray3Ex<T> other)
    {
        for (var i = 0; i < 3; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 3; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 3; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(4)]
public struct InlineArray4Ex<T> : IEquatable<InlineArray4Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray4<T>(in InlineArray4Ex<T> array)
        => Unsafe.As<InlineArray4Ex<T>, InlineArray4<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray4Ex<T>(in InlineArray4<T> array)
        => Unsafe.As<InlineArray4<T>, InlineArray4Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 4; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this[0], this[1], this[2], this[3]);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray4Ex<T> other)
            return false;
        for (var i = 0; i < 4; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray4Ex<T> other)
    {
        for (var i = 0; i < 4; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 4; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 4; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(5)]
public struct InlineArray5Ex<T> : IEquatable<InlineArray5Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray5<T>(in InlineArray5Ex<T> array)
        => Unsafe.As<InlineArray5Ex<T>, InlineArray5<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray5Ex<T>(in InlineArray5<T> array)
        => Unsafe.As<InlineArray5<T>, InlineArray5Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 5; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this[0], this[1], this[2], this[3], this[4]);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray5Ex<T> other)
            return false;
        for (var i = 0; i < 5; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray5Ex<T> other)
    {
        for (var i = 0; i < 5; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 5; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 5; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(6)]
public struct InlineArray6Ex<T> : IEquatable<InlineArray6Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray6<T>(in InlineArray6Ex<T> array)
        => Unsafe.As<InlineArray6Ex<T>, InlineArray6<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray6Ex<T>(in InlineArray6<T> array)
        => Unsafe.As<InlineArray6<T>, InlineArray6Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 6; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this[0], this[1], this[2], this[3], this[4], this[5]);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray6Ex<T> other)
            return false;
        for (var i = 0; i < 6; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray6Ex<T> other)
    {
        for (var i = 0; i < 6; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 6; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 6; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(7)]
public struct InlineArray7Ex<T> : IEquatable<InlineArray7Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray7<T>(in InlineArray7Ex<T> array)
        => Unsafe.As<InlineArray7Ex<T>, InlineArray7<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray7Ex<T>(in InlineArray7<T> array)
        => Unsafe.As<InlineArray7<T>, InlineArray7Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 7; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this[0], this[1], this[2], this[3], this[4], this[5], this[6]);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray7Ex<T> other)
            return false;
        for (var i = 0; i < 7; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray7Ex<T> other)
    {
        for (var i = 0; i < 7; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 7; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 7; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(8)]
public struct InlineArray8Ex<T> : IEquatable<InlineArray8Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray8<T>(in InlineArray8Ex<T> array)
        => Unsafe.As<InlineArray8Ex<T>, InlineArray8<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray8Ex<T>(in InlineArray8<T> array)
        => Unsafe.As<InlineArray8<T>, InlineArray8Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 8; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7]);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray8Ex<T> other)
            return false;
        for (var i = 0; i < 8; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray8Ex<T> other)
    {
        for (var i = 0; i < 8; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 8; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 8; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(9)]
public struct InlineArray9Ex<T> : IEquatable<InlineArray9Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray9<T>(in InlineArray9Ex<T> array)
        => Unsafe.As<InlineArray9Ex<T>, InlineArray9<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray9Ex<T>(in InlineArray9<T> array)
        => Unsafe.As<InlineArray9<T>, InlineArray9Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 9; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (var i = 0; i < 9; i++)
            hash.Add(this[i]);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray9Ex<T> other)
            return false;
        for (var i = 0; i < 9; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray9Ex<T> other)
    {
        for (var i = 0; i < 9; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 9; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 9; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(10)]
public struct InlineArray10Ex<T> : IEquatable<InlineArray10Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray10<T>(in InlineArray10Ex<T> array)
        => Unsafe.As<InlineArray10Ex<T>, InlineArray10<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray10Ex<T>(in InlineArray10<T> array)
        => Unsafe.As<InlineArray10<T>, InlineArray10Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 10; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (var i = 0; i < 10; i++)
            hash.Add(this[i]);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray10Ex<T> other)
            return false;
        for (var i = 0; i < 10; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray10Ex<T> other)
    {
        for (var i = 0; i < 10; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 10; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 10; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(11)]
public struct InlineArray11Ex<T> : IEquatable<InlineArray11Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray11<T>(in InlineArray11Ex<T> array)
        => Unsafe.As<InlineArray11Ex<T>, InlineArray11<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray11Ex<T>(in InlineArray11<T> array)
        => Unsafe.As<InlineArray11<T>, InlineArray11Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 11; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (var i = 0; i < 11; i++)
            hash.Add(this[i]);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray11Ex<T> other)
            return false;
        for (var i = 0; i < 11; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray11Ex<T> other)
    {
        for (var i = 0; i < 11; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 11; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 11; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(12)]
public struct InlineArray12Ex<T> : IEquatable<InlineArray12Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray12<T>(in InlineArray12Ex<T> array)
        => Unsafe.As<InlineArray12Ex<T>, InlineArray12<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray12Ex<T>(in InlineArray12<T> array)
        => Unsafe.As<InlineArray12<T>, InlineArray12Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 12; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (var i = 0; i < 12; i++)
            hash.Add(this[i]);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray12Ex<T> other)
            return false;
        for (var i = 0; i < 12; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray12Ex<T> other)
    {
        for (var i = 0; i < 12; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 12; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 12; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(13)]
public struct InlineArray13Ex<T> : IEquatable<InlineArray13Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray13<T>(in InlineArray13Ex<T> array)
        => Unsafe.As<InlineArray13Ex<T>, InlineArray13<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray13Ex<T>(in InlineArray13<T> array)
        => Unsafe.As<InlineArray13<T>, InlineArray13Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 13; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (var i = 0; i < 13; i++)
            hash.Add(this[i]);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray13Ex<T> other)
            return false;
        for (var i = 0; i < 13; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray13Ex<T> other)
    {
        for (var i = 0; i < 13; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 13; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 13; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(14)]
public struct InlineArray14Ex<T> : IEquatable<InlineArray14Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray14<T>(in InlineArray14Ex<T> array)
        => Unsafe.As<InlineArray14Ex<T>, InlineArray14<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray14Ex<T>(in InlineArray14<T> array)
        => Unsafe.As<InlineArray14<T>, InlineArray14Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 14; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (var i = 0; i < 14; i++)
            hash.Add(this[i]);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray14Ex<T> other)
            return false;
        for (var i = 0; i < 14; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray14Ex<T> other)
    {
        for (var i = 0; i < 14; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 14; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 14; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(15)]
public struct InlineArray15Ex<T> : IEquatable<InlineArray15Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray15<T>(in InlineArray15Ex<T> array)
        => Unsafe.As<InlineArray15Ex<T>, InlineArray15<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray15Ex<T>(in InlineArray15<T> array)
        => Unsafe.As<InlineArray15<T>, InlineArray15Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 15; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (var i = 0; i < 15; i++)
            hash.Add(this[i]);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray15Ex<T> other)
            return false;
        for (var i = 0; i < 15; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray15Ex<T> other)
    {
        for (var i = 0; i < 15; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 15; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 15; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

[InlineArray(16)]
public struct InlineArray16Ex<T> : IEquatable<InlineArray16Ex<T>>, ISpanFormattable
{
#nullable disable
    private T t;
#nullable enable

    public static implicit operator InlineArray16<T>(in InlineArray16Ex<T> array)
        => Unsafe.As<InlineArray16Ex<T>, InlineArray16<T>>(ref Unsafe.AsRef(in array));

    public static implicit operator InlineArray16Ex<T>(in InlineArray16<T> array)
        => Unsafe.As<InlineArray16<T>, InlineArray16Ex<T>>(ref Unsafe.AsRef(in array));

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < 16; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (var i = 0; i < 16; i++)
            hash.Add(this[i]);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InlineArray16Ex<T> other)
            return false;
        for (var i = 0; i < 16; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public bool Equals(InlineArray16Ex<T> other)
    {
        for (var i = 0; i < 16; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
                return false;
        }
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var sb = new ValueStringBuilder();
        var handler = new ValueStringBuilder.ValueAppendInterpolatedStringHandler(-1, -1, sb, formatProvider);
        sb.Append('[');
        for (var i = 0; i < 16; i++)
        {
            if (i > 0)
                sb.Append(", ");

            handler.AppendFormatted(this[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(6, 4, destination, provider, out var shouldAppend);
        if (shouldAppend)
        {
            var formatString = new string(format);
            for (var i = 0; i < 16; i++)
            {
                if (i > 0)
                    handler.AppendLiteral(", ");
                handler.AppendFormatted(this[i], formatString);
            }

            return destination.TryWrite(provider, ref handler, out charsWritten);
        }

        charsWritten = 0;
        return false;
    }
}

