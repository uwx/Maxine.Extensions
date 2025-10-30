using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Maxine.TU.Sims4Exporter;

// https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/Text/ValueStringBuilder.cs
public ref struct ValueStringBuilder
{
    public const int StackallocCharBufferSizeLimit = 256;
    private char[]? _arrayToReturnToPool;
    private Span<char> _chars;
    private int _pos;

    public ValueStringBuilder(Span<char> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _chars = initialBuffer;
        _pos = 0;
    }

    public ValueStringBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        _chars = _arrayToReturnToPool;
        _pos = 0;
    }

    public int Length
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _chars.Length);
            _pos = value;
        }
    }

    public int Capacity => _chars.Length;

    public void EnsureCapacity(int capacity)
    {
        // This is not expected to be called this with negative capacity
        Debug.Assert(capacity >= 0);

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if ((uint)capacity > (uint)_chars.Length)
            Grow(capacity - _pos);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// Does not ensure there is a null char after <see cref="Length"/>
    /// This overload is pattern matched in the C# 7.3+ compiler so you can omit
    /// the explicit method call, and write eg "fixed (char* c = builder)"
    /// </summary>
    public ref char GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(_chars);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ref char GetPinnableReference(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = '\0';
        }
        return ref MemoryMarshal.GetReference(_chars);
    }

    public ref char this[int index]
    {
        get
        {
            Debug.Assert(index < _pos);
            return ref _chars[index];
        }
    }

    public override string ToString()
    {
        var s = _chars[.._pos].ToString();
        Dispose();
        return s;
    }

    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<char> RawChars => _chars;

    /// <summary>
    /// Returns a span around the contents of the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ReadOnlySpan<char> AsSpan(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = '\0';
        }
        return _chars[.._pos];
    }

    public ReadOnlySpan<char> AsSpan() => _chars[.._pos];
    public ReadOnlySpan<char> AsSpan(int start) => _chars.Slice(start, _pos - start);
    public ReadOnlySpan<char> AsSpan(int start, int length) => _chars.Slice(start, length);

    public bool TryCopyTo(Span<char> destination, out int charsWritten)
    {
        if (_chars[.._pos].TryCopyTo(destination))
        {
            charsWritten = _pos;
            Dispose();
            return true;
        }
        else
        {
            charsWritten = 0;
            Dispose();
            return false;
        }
    }

    public void Insert(int index, char value, int count)
    {
        if (_pos > _chars.Length - count)
        {
            Grow(count);
        }

        var remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars[(index + count)..]);
        _chars.Slice(index, count).Fill(value);
        _pos += count;
    }

    public void Insert(int index, string? s)
    {
        if (s == null)
        {
            return;
        }

        var count = s.Length;

        if (_pos > (_chars.Length - count))
        {
            Grow(count);
        }

        var remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars[(index + count)..]);
        s
#if !NETCOREAPP
            .AsSpan()
#endif
            .CopyTo(_chars[index..]);
        _pos += count;
    }

    /// <summary>Appends the specified interpolated string to this instance.</summary>
    /// <param name="handler">The interpolated string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CA1822
    public void Append([InterpolatedStringHandlerArgument("")] ref ValueAppendInterpolatedStringHandler handler)
#pragma warning restore CA1822
    {
    }

    /// <summary>Appends the specified interpolated string to this instance.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="handler">The interpolated string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CA1822
    public void Append(
#pragma warning restore CA1822
        IFormatProvider? provider,
        [InterpolatedStringHandlerArgument("", nameof(provider))] ref ValueAppendInterpolatedStringHandler handler
    )
    {
    }

    /// <summary>Appends the specified interpolated string followed by the default line terminator to the end of the current StringBuilder object.</summary>
    /// <param name="handler">The interpolated string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine([InterpolatedStringHandlerArgument("")] ref ValueAppendInterpolatedStringHandler handler) => AppendLine();

    /// <summary>Appends the specified interpolated string followed by the default line terminator to the end of the current StringBuilder object.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="handler">The interpolated string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(
        IFormatProvider? provider,
        [InterpolatedStringHandlerArgument("", nameof(provider))] ref ValueAppendInterpolatedStringHandler handler
    )
        => AppendLine();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine() => Append(Environment.NewLine);

    #region ValueAppendInterpolatedStringHandler
    /// <summary>Provides a handler used by the language compiler to append interpolated strings into <see cref="StringBuilder"/> instances.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [InterpolatedStringHandler]
    public ref struct ValueAppendInterpolatedStringHandler
    {
        // Implementation note:
        // As this type is only intended to be targeted by the compiler, public APIs eschew argument validation logic
        // in a variety of places, e.g. allowing a null input when one isn't expected to produce a NullReferenceException rather
        // than an ArgumentNullException.

        /// <summary>The associated StringBuilder to which to append.</summary>
        internal ValueStringBuilder _stringBuilder;
        /// <summary>Optional provider to pass to IFormattable.ToString or ISpanFormattable.TryFormat calls.</summary>
        private readonly IFormatProvider? _provider;
        /// <summary>Whether <see cref="_provider"/> provides an ICustomFormatter.</summary>
        /// <remarks>
        /// Custom formatters are very rare.  We want to support them, but it's ok if we make them more expensive
        /// in order to make them as pay-for-play as possible.  So, we avoid adding another reference type field
        /// to reduce the size of the handler and to reduce required zero'ing, by only storing whether the provider
        /// provides a formatter, rather than actually storing the formatter.  This in turn means, if there is a
        /// formatter, we pay for the extra interface call on each AppendFormatted that needs it.
        /// </remarks>
        private readonly bool _hasCustomFormatter;

        /// <summary>Creates a handler used to append an interpolated string into a <see cref="StringBuilder"/>.</summary>
        /// <param name="literalLength">The number of constant characters outside of interpolation expressions in the interpolated string.</param>
        /// <param name="formattedCount">The number of interpolation expressions in the interpolated string.</param>
        /// <param name="stringBuilder">The associated StringBuilder to which to append.</param>
        /// <remarks>This is intended to be called only by compiler-generated code. Arguments are not validated as they'd otherwise be for members intended to be used directly.</remarks>
        public ValueAppendInterpolatedStringHandler(int literalLength, int formattedCount, ValueStringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
            _provider = null;
            _hasCustomFormatter = false;
        }

        /// <summary>Creates a handler used to translate an interpolated string into a <see cref="string"/>.</summary>
        /// <param name="literalLength">The number of constant characters outside of interpolation expressions in the interpolated string.</param>
        /// <param name="formattedCount">The number of interpolation expressions in the interpolated string.</param>
        /// <param name="stringBuilder">The associated StringBuilder to which to append.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <remarks>This is intended to be called only by compiler-generated code. Arguments are not validated as they'd otherwise be for members intended to be used directly.</remarks>
        public ValueAppendInterpolatedStringHandler(int literalLength, int formattedCount, ValueStringBuilder stringBuilder, IFormatProvider? provider)
        {
            _stringBuilder = stringBuilder;
            _provider = provider;
            _hasCustomFormatter = provider is not null && HasCustomFormatter(provider);
        }
        
        /// <summary>Gets whether the provider provides a custom formatter.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // only used in a few hot path call sites
        private static bool HasCustomFormatter(IFormatProvider provider)
        {
            Debug.Assert(provider is not null);
            Debug.Assert(provider is not CultureInfo || provider.GetFormat(typeof(ICustomFormatter)) is null, "Expected CultureInfo to not provide a custom formatter");
            return
                provider.GetType() != typeof(CultureInfo) && // optimization to avoid GetFormat in the majority case
                provider.GetFormat(typeof(ICustomFormatter)) != null;
        }

        /// <summary>Writes the specified string to the handler.</summary>
        /// <param name="value">The string to write.</param>
        public void AppendLiteral(string value) => _stringBuilder.Append(value);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "TryFormatUnconstrained")]
        private static extern bool TryFormatUnconstrained<TEnum>(Enum ownerType, TEnum value, Span<char> destination, out int charsWritten, [StringSyntax(StringSyntaxAttribute.EnumFormat)] ReadOnlySpan<char> format = default);

        #region AppendFormatted
        // Design note:
        // This provides the same set of overloads and semantics as DefaultInterpolatedStringHandler.

        #region AppendFormatted T
        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value)
        {
            // This method could delegate to AppendFormatted with a null format, but explicitly passing
            // default as the format to TryFormat helps to improve code quality in some cases when TryFormat is inlined,
            // e.g. for Int32 it enables the JIT to eliminate code in the inlined method based on a length check on the format.

            if (_hasCustomFormatter)
            {
                // If there's a custom formatter, always use it.
                AppendCustomFormatter(value, format: null);
            }
            else if (value is IFormattable)
            {
                // Check first for IFormattable, even though we'll prefer to use ISpanFormattable, as the latter
                // requires the former.  For value types, it won't matter as the type checks devolve into
                // JIT-time constants.  For reference types, they're more likely to implement IFormattable
                // than they are to implement ISpanFormattable: if they don't implement either, we save an
                // interface check over first checking for ISpanFormattable and then for IFormattable, and
                // if it only implements IFormattable, we come out even: only if it implements both do we
                // end up paying for an extra interface check.

                if (typeof(T).IsEnum)
                {
                    if (TryFormatUnconstrained(default!, value, _stringBuilder.RemainingCurrentChunk, out var charsWritten))
                    {
                        _stringBuilder._pos += charsWritten;
                    }
                    else
                    {
                        AppendFormattedWithTempSpace(value, 0, format: null);
                    }
                }
                else if (value is ISpanFormattable formattable)
                {
                    var destination = _stringBuilder.RemainingCurrentChunk;
                    if (formattable.TryFormat(destination, out var charsWritten, default, _provider)) // constrained call avoiding boxing for value types
                    {
                        if ((uint)charsWritten > (uint)destination.Length)
                        {
                            // Protect against faulty ISpanFormattable implementations returning invalid charsWritten values.
                            // Other code in _stringBuilder uses Unsafe manipulation, and we want to ensure m_ChunkLength remains safe.
                            ThrowFormatInvalidString();
                        }

                        _stringBuilder._pos += charsWritten;
                    }
                    else
                    {
                        // Not enough room in the current chunk.  Take the slow path that formats into temporary space
                        // and then copies the result into the StringBuilder.
                        AppendFormattedWithTempSpace(value, 0, format: null);
                    }
                }
                else
                {
                    _stringBuilder.Append(((IFormattable)value).ToString(format: null, _provider)); // constrained call avoiding boxing for value types
                }
            }
            else if (value is not null)
            {
                _stringBuilder.Append(value.ToString());
            }
        }

        private static void ThrowFormatInvalidString()
            => throw new FormatException("Input string was not in a correct format.");

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="format">The format string.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, string? format)
        {
            if (_hasCustomFormatter)
            {
                // If there's a custom formatter, always use it.
                AppendCustomFormatter(value, format);
            }
            else if (value is IFormattable formattable)
            {
                // Check first for IFormattable, even though we'll prefer to use ISpanFormattable, as the latter
                // requires the former.  For value types, it won't matter as the type checks devolve into
                // JIT-time constants.  For reference types, they're more likely to implement IFormattable
                // than they are to implement ISpanFormattable: if they don't implement either, we save an
                // interface check over first checking for ISpanFormattable and then for IFormattable, and
                // if it only implements IFormattable, we come out even: only if it implements both do we
                // end up paying for an extra interface check.

                if (typeof(T).IsEnum)
                {
                    if (TryFormatUnconstrained(default!, value, _stringBuilder.RemainingCurrentChunk, out int charsWritten, format))
                    {
                        _stringBuilder._pos += charsWritten;
                    }
                    else
                    {
                        AppendFormattedWithTempSpace(value, 0, format);
                    }
                }
                else if (formattable is ISpanFormattable spanFormattable)
                {
                    var destination = _stringBuilder.RemainingCurrentChunk;
                    if (spanFormattable.TryFormat(destination, out var charsWritten, format, _provider)) // constrained call avoiding boxing for value types
                    {
                        if ((uint)charsWritten > (uint)destination.Length)
                        {
                            // Protect against faulty ISpanFormattable implementations returning invalid charsWritten values.
                            // Other code in _stringBuilder uses Unsafe manipulation, and we want to ensure m_ChunkLength remains safe.
                            ThrowFormatInvalidString();
                        }

                        _stringBuilder._pos += charsWritten;
                    }
                    else
                    {
                        // Not enough room in the current chunk.  Take the slow path that formats into temporary space
                        // and then copies the result into the StringBuilder.
                        AppendFormattedWithTempSpace(value, 0, format);
                    }
                }
                else
                {
                    _stringBuilder.Append(formattable.ToString(format, _provider)); // constrained call avoiding boxing for value types
                }
            }
            else if (value is not null)
            {
                _stringBuilder.Append(value.ToString());
            }
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, int alignment) =>
            AppendFormatted(value, alignment, format: null);

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="format">The format string.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, int alignment, string? format)
        {
            if (alignment == 0)
            {
                // This overload is used as a fallback from several disambiguation overloads, so special-case 0.
                AppendFormatted(value, format);
            }
            else if (alignment < 0)
            {
                // Left aligned: format into the handler, then append any additional padding required.
                var start = _stringBuilder.Length;
                AppendFormatted(value, format);
                var paddingRequired = -alignment - (_stringBuilder.Length - start);
                if (paddingRequired > 0)
                {
                    _stringBuilder.Append(' ', paddingRequired);
                }
            }
            else
            {
                // Right aligned: format into temporary space and then copy that into the handler, appropriately aligned.
                AppendFormattedWithTempSpace(value, alignment, format);
            }
        }

        /// <summary>Formats into temporary space and then appends the result into the StringBuilder.</summary>
        private void AppendFormattedWithTempSpace<T>(T value, int alignment, string? format)
        {
            // It's expected that either there's not enough space in the current chunk to store this formatted value,
            // or we have a non-0 alignment that could require padding inserted. So format into temporary space and
            // then append that written span into the StringBuilder: StringBuilder.Append(span) is able to split the
            // span across the current chunk and any additional chunks required.
            
            var handler = new DefaultInterpolatedStringHandler(0, 0, _provider, stackalloc char[StackallocCharBufferSizeLimit]);
            handler.AppendFormatted(value, format);
            var text = GetText(ref handler);
            
            // dupe
            if (alignment == 0)
            {
                _stringBuilder.Append(text);
            }
            else
            {
                var leftAlign = false;
                if (alignment < 0)
                {
                    leftAlign = true;
                    alignment = -alignment;
                }

                var paddingRequired = alignment - text.Length;
                if (paddingRequired <= 0)
                {
                    _stringBuilder.Append(text);
                }
                else if (leftAlign)
                {
                    _stringBuilder.Append(text);
                    _stringBuilder.Append(' ', paddingRequired);
                }
                else
                {
                    _stringBuilder.Append(' ', paddingRequired);
                    _stringBuilder.Append(text);
                }
            }
            // dupe end
            
            Clear(ref handler);
            return;

            [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Text")]
            static extern ReadOnlySpan<char> GetText(scoped ref readonly DefaultInterpolatedStringHandler instance);
            
            [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Clear")]
            static extern void Clear(scoped ref readonly DefaultInterpolatedStringHandler instance);
        }
        #endregion

        #region AppendFormatted ReadOnlySpan<char>
        /// <summary>Writes the specified character span to the handler.</summary>
        /// <param name="value">The span to write.</param>
        public void AppendFormatted(ReadOnlySpan<char> value) => _stringBuilder.Append(value);

        /// <summary>Writes the specified string of chars to the handler.</summary>
        /// <param name="value">The span to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
        {
            if (alignment == 0)
            {
                _stringBuilder.Append(value);
            }
            else
            {
                var leftAlign = false;
                if (alignment < 0)
                {
                    leftAlign = true;
                    alignment = -alignment;
                }

                var paddingRequired = alignment - value.Length;
                if (paddingRequired <= 0)
                {
                    _stringBuilder.Append(value);
                }
                else if (leftAlign)
                {
                    _stringBuilder.Append(value);
                    _stringBuilder.Append(' ', paddingRequired);
                }
                else
                {
                    _stringBuilder.Append(' ', paddingRequired);
                    _stringBuilder.Append(value);
                }
            }
        }
        #endregion

        #region AppendFormatted string
        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        public void AppendFormatted(string? value)
        {
            if (!_hasCustomFormatter)
            {
                _stringBuilder.Append(value);
            }
            else
            {
                AppendFormatted<string?>(value);
            }
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(string? value, int alignment = 0, string? format = null) =>
            // Format is meaningless for strings and doesn't make sense for someone to specify.  We have the overload
            // simply to disambiguate between ROS<char> and object, just in case someone does specify a format, as
            // string is implicitly convertible to both. Just delegate to the T-based implementation.
            AppendFormatted<string?>(value, alignment, format);
        #endregion

        #region AppendFormatted object
        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(object? value, int alignment = 0, string? format = null) =>
            // This overload is expected to be used rarely, only if either a) something strongly typed as object is
            // formatted with both an alignment and a format, or b) the compiler is unable to target type to T. It
            // exists purely to help make cases from (b) compile. Just delegate to the T-based implementation.
            AppendFormatted<object?>(value, alignment, format);
        #endregion
        #endregion

        /// <summary>Formats the value using the custom formatter from the provider.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="format">The format string.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AppendCustomFormatter<T>(T value, string? format)
        {
            // This case is very rare, but we need to handle it prior to the other checks in case
            // a provider was used that supplied an ICustomFormatter which wanted to intercept the particular value.
            // We do the cast here rather than in the ctor, even though this could be executed multiple times per
            // formatting, to make the cast pay for play.
            Debug.Assert(_hasCustomFormatter);
            Debug.Assert(_provider != null);

            var formatter = (ICustomFormatter?)_provider.GetFormat(typeof(ICustomFormatter));
            Debug.Assert(formatter != null, "An incorrectly written provider said it implemented ICustomFormatter, and then didn't");

            if (formatter is not null)
            {
                _stringBuilder.Append(formatter.Format(format, value, _provider));
            }
        }
    }

    private Span<char> RemainingCurrentChunk
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars[_pos..];
    }
    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        var pos = _pos;
        var chars = _chars;
        if ((uint)pos < (uint)chars.Length)
        {
            chars[pos] = c;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? s)
    {
        if (s == null)
        {
            return;
        }

        var pos = _pos;
        if (s.Length == 1 && (uint)pos < (uint)_chars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
        {
            _chars[pos] = s[0];
            _pos = pos + 1;
        }
        else
        {
            AppendSlow(s);
        }
    }

    private void AppendSlow(string s)
    {
        var pos = _pos;
        if (pos > _chars.Length - s.Length)
        {
            Grow(s.Length);
        }

        s
#if !NETCOREAPP
            .AsSpan()
#endif
            .CopyTo(_chars[pos..]);
        _pos += s.Length;
    }

    public void Append(char c, int count)
    {
        if (_pos > _chars.Length - count)
        {
            Grow(count);
        }

        var dst = _chars.Slice(_pos, count);
        for (var i = 0; i < dst.Length; i++)
        {
            dst[i] = c;
        }
        _pos += count;
    }

    public unsafe void Append(char* value, int length)
    {
        var pos = _pos;
        if (pos > _chars.Length - length)
        {
            Grow(length);
        }

        var dst = _chars.Slice(_pos, length);
        for (var i = 0; i < dst.Length; i++)
        {
            dst[i] = *value++;
        }
        _pos += length;
    }

    public void Append(scoped ReadOnlySpan<char> value)
    {
        var pos = _pos;
        if (pos > _chars.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_chars[_pos..]);
        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
    {
        var origPos = _pos;
        if (origPos > _chars.Length - length)
        {
            Grow(length);
        }

        _pos = origPos + length;
        return _chars.Slice(origPos, length);
    }

    public void AppendLine(string? value)
    {
        Append(value);
        Append(Environment.NewLine);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        Grow(1);
        Append(c);
    }

    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_pos"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
        // to double the size if possible, bounding the doubling to not go beyond the max array length.
        var newCapacity = (int)Math.Max(
            (uint)(_pos + additionalCapacityBeyondPos),
            Math.Min((uint)_chars.Length * 2, ArrayMaxLength));

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative.
        // This could also go negative if the actual required length wraps around.
        var poolArray = ArrayPool<char>.Shared.Rent(newCapacity);

        _chars[.._pos].CopyTo(poolArray);

        var toReturn = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = poolArray;
        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        var toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    #region AppendSpanFormattable
    internal void AppendSpanFormattable<T>(T value, string? format = null, IFormatProvider? provider = null) where T : ISpanFormattable
    {
        if (value.TryFormat(_chars[_pos..], out var charsWritten, format, provider))
        {
            _pos += charsWritten;
        }
        else
        {
            Append(value.ToString(format, provider));
        }
    }
    #endregion

    #region System.Text.ValueStringBuilderExtensions
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(Rune rune)
    {
        var pos = _pos;
        var chars = _chars;
        if ((uint)(pos + 1) < (uint)chars.Length && (uint)pos < (uint)chars.Length)
        {
            if (rune.Value <= 0xFFFF)
            {
                chars[pos] = (char)rune.Value;
                _pos = pos + 1;
            }
            else
            {
                chars[pos] = (char)((rune.Value + ((0xD800u - 0x40u) << 10)) >> 10);
                chars[pos + 1] = (char)((rune.Value & 0x3FFu) + 0xDC00u);
                _pos = pos + 2;
            }
        }
        else
        {
            GrowAndAppend(rune);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(Rune rune)
    {
        Grow(2);
        Append(rune);
    }
    #endregion

    #region ValueStringBuilder.AppendFormat
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteFormatHelper(string format, scoped ReadOnlySpan<object?> args)
    {
        ArgumentNullException.ThrowIfNull(format);

        EnsureCapacity(_pos + (format.Length - (args.Length * 3))); // account for {0} etc

        AppendFormatHelper(null, format!, args); // AppendFormatHelper will appropriately throw ArgumentNullException for a null format
    }
    
    [InlineArray(2)]
    internal struct TwoObjects
    {
        internal object? Arg0;

        public TwoObjects(object? arg0, object? arg1)
        {
            this[0] = arg0;
            this[1] = arg1;
        }
    }

    [InlineArray(3)]
    internal struct ThreeObjects
    {
        internal object? Arg0;

        public ThreeObjects(object? arg0, object? arg1, object? arg2)
        {
            this[0] = arg0;
            this[1] = arg1;
            this[2] = arg2;
        }
    }

    public void AppendFormat(FormattableString formattableString)
    {
        Append(formattableString.Format, formattableString.GetArguments());
    }

    public void Append([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
    {
        WriteFormatHelper(format, new ReadOnlySpan<object?>(in arg0));
    }

    public void Append([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
    {
        var two = new TwoObjects(arg0, arg1);
        WriteFormatHelper(format, MemoryMarshal.CreateReadOnlySpan(ref two.Arg0, 2));
    }

    public void Append([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
    {
        var three = new ThreeObjects(arg0, arg1, arg2);
        WriteFormatHelper(format, MemoryMarshal.CreateReadOnlySpan(ref three.Arg0, 3));
    }

    public void Append([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] arg)
    {
        if (arg == null)
        {
            throw new ArgumentNullException(format is null ? nameof(format) : nameof(arg)); // same as base logic
        }
        WriteFormatHelper(format, arg);
    }

    public void AppendLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
    {
        WriteFormatHelper(format, new ReadOnlySpan<object?>(in arg0));
        AppendLine();
    }

    public void AppendLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
    {
        var two = new TwoObjects(arg0, arg1);
        WriteFormatHelper(format, MemoryMarshal.CreateReadOnlySpan(ref two.Arg0, 2));
        AppendLine();
    }

    public void AppendLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
    {
        var three = new ThreeObjects(arg0, arg1, arg2);
        WriteFormatHelper(format, MemoryMarshal.CreateReadOnlySpan(ref three.Arg0, 3));
        AppendLine();
    }

    public void AppendLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] arg)
    {
        ArgumentNullException.ThrowIfNull(arg);
        WriteFormatHelper(format, arg);
        AppendLine();
    }

    // Copied from StringBuilder, can't be done via generic extension
    // as ValueStringBuilder is a ref struct and cannot be used in a generic.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendFormatHelper(IFormatProvider? provider, string format, scoped ReadOnlySpan<object?> args)
    {
        // Undocumented exclusive limits on the range for Argument Hole Index and Argument Hole Alignment.
        const int IndexLimit = 1_000_000; // Note:            0 <= ArgIndex < IndexLimit
        const int WidthLimit = 1_000_000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

        // Query the provider (if one was supplied) for an ICustomFormatter.  If there is one,
        // it needs to be used to transform all arguments.
        var cf = (ICustomFormatter?)provider?.GetFormat(typeof(ICustomFormatter));

        // Repeatedly find the next hole and process it.
        var pos = 0;
        while (true)
        {
            // Skip until either the end of the input or the first unescaped opening brace, whichever comes first.
            // Along the way we need to also unescape escaped closing braces.
            char ch;
            while (true)
            {
                // Find the next brace.  If there isn't one, the remainder of the input is text to be appended, and we're done.
                if ((uint)pos >= (uint)format.Length)
                {
                    return;
                }

                var remainder = format.AsSpan(pos);
                var countUntilNextBrace = remainder.IndexOfAny('{', '}');
                if (countUntilNextBrace < 0)
                {
                    Append(remainder);
                    return;
                }

                // Append the text until the brace.
                Append(remainder[..countUntilNextBrace]);
                pos += countUntilNextBrace;

                // Get the brace.  It must be followed by another character, either a copy of itself in the case of being
                // escaped, or an arbitrary character that's part of the hole in the case of an opening brace.
                var brace = format[pos];
                ch = MoveNext(format, ref pos);
                if (brace == ch)
                {
                    Append(ch);
                    pos++;
                    continue;
                }

                // This wasn't an escape, so it must be an opening brace.
                if (brace != '{')
                {
                    ThrowInputStringNotInCorrectFormat_EndsPrematurely(pos);
                }

                // Proceed to parse the hole.
                break;
            }

            // We're now positioned just after the opening brace of an argument hole, which consists of
            // an opening brace, an index, an optional width preceded by a comma, and an optional format
            // preceded by a colon, with arbitrary amounts of spaces throughout.
            var width = 0;
            var leftJustify = false;
            ReadOnlySpan<char> itemFormatSpan = default; // used if itemFormat is null

            // First up is the index parameter, which is of the form:
            //     at least on digit
            //     optional any number of spaces
            // We've already read the first digit into ch.
            Debug.Assert(format[pos - 1] == '{');
            Debug.Assert(ch != '{');
            var index = ch - '0';
            if ((uint)index >= 10u)
            {
                ThrowInputStringNotInCorrectFormat_ExpectedAscii(pos);
            }

            // Common case is a single digit index followed by a closing brace.  If it's not a closing brace,
            // proceed to finish parsing the full hole format.
            ch = MoveNext(format, ref pos);
            if (ch != '}')
            {
                // Continue consuming optional additional digits.
                while (char.IsAsciiDigit(ch) && index < IndexLimit)
                {
                    index = index * 10 + ch - '0';
                    ch = MoveNext(format, ref pos);
                }

                // Consume optional whitespace.
                while (ch == ' ')
                {
                    ch = MoveNext(format, ref pos);
                }

                // Parse the optional alignment, which is of the form:
                //     comma
                //     optional any number of spaces
                //     optional -
                //     at least one digit
                //     optional any number of spaces
                if (ch == ',')
                {
                    // Consume optional whitespace.
                    do
                    {
                        ch = MoveNext(format, ref pos);
                    }
                    while (ch == ' ');

                    // Consume an optional minus sign indicating left alignment.
                    if (ch == '-')
                    {
                        leftJustify = true;
                        ch = MoveNext(format, ref pos);
                    }

                    // Parse alignment digits. The read character must be a digit.
                    width = ch - '0';
                    if ((uint)width >= 10u)
                    {
                        ThrowInputStringNotInCorrectFormat_ExpectedAscii(pos);
                    }
                    ch = MoveNext(format, ref pos);
                    while (char.IsAsciiDigit(ch) && width < WidthLimit)
                    {
                        width = width * 10 + ch - '0';
                        ch = MoveNext(format, ref pos);
                    }

                    // Consume optional whitespace
                    while (ch == ' ')
                    {
                        ch = MoveNext(format, ref pos);
                    }
                }

                // The next character needs to either be a closing brace for the end of the hole,
                // or a colon indicating the start of the format.
                if (ch != '}')
                {
                    if (ch != ':')
                    {
                        // Unexpected character
                        ThrowInputStringNotInCorrectFormat_EndsPrematurely(pos);
                    }

                    // Search for the closing brace; everything in between is the format,
                    // but opening braces aren't allowed.
                    var startingPos = pos;
                    while (true)
                    {
                        ch = MoveNext(format, ref pos);

                        if (ch == '}')
                        {
                            // Argument hole closed
                            break;
                        }

                        if (ch == '{')
                        {
                            // Braces inside the argument hole are not supported
                            ThrowInputStringNotInCorrectFormat_EndsPrematurely(pos);
                        }
                    }

                    startingPos++;
                    itemFormatSpan = format.AsSpan(startingPos, pos - startingPos);
                }
            }

            // Construct the output for this arg hole.
            Debug.Assert(format[pos] == '}');
            pos++;
            string? s = null;
            string? itemFormat = null;

            if ((uint)index >= (uint)args.Length)
            {
                ThrowIndexOutOfRange();
            }
            var arg = args[index];

            if (cf != null)
            {
                if (!itemFormatSpan.IsEmpty)
                {
                    itemFormat = new string(itemFormatSpan);
                }

                s = cf.Format(itemFormat, arg, provider);
            }

            if (s == null)
            {
                // If arg is ISpanFormattable and the beginning doesn't need padding,
                // try formatting it into the remaining current chunk.
                if ((leftJustify || width == 0) &&
                    arg is ISpanFormattable spanFormattableArg &&
                    spanFormattableArg.TryFormat(_chars[_pos..], out var charsWritten, itemFormatSpan, provider))
                {
                    _pos += charsWritten;

                    // Pad the end, if needed.
                    if (leftJustify && width > charsWritten)
                    {
                        Append(' ', width - charsWritten);
                    }

                    // Continue to parse other characters.
                    continue;
                }

                // Otherwise, fallback to trying IFormattable or calling ToString.
                if (arg is IFormattable formattableArg)
                {
                    if (itemFormatSpan.Length != 0)
                    {
                        itemFormat ??= new string(itemFormatSpan);
                    }
                    s = formattableArg.ToString(itemFormat, provider);
                }
                else
                {
                    s = arg?.ToString();
                }

                s ??= string.Empty;
            }

            // Append it to the final output of the Format String.
            if (width <= s.Length)
            {
                Append(s);
            }
            else if (leftJustify)
            {
                Append(s);
                Append(' ', width - s.Length);
            }
            else
            {
                Append(' ', width - s.Length);
                Append(s);
            }

            // Continue parsing the rest of the format string.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static char MoveNext(string format, ref int pos)
        {
            pos++;
            if ((uint)pos >= (uint)format.Length)
            {
                ThrowInputStringNotInCorrectFormat_EndsPrematurely(pos);
            }
            return format[pos];
        }
        
        static void ThrowIndexOutOfRange()
        {
            throw new FormatException(
                "Index (zero based) must be greater than or equal to zero and less than the size of the argument list."
            );
        }

        // ReSharper disable once InconsistentNaming
        static void ThrowInputStringNotInCorrectFormat_ExpectedAscii(int pos)
        {
            throw new FormatException(
                $"Input string was not in a correct format. Failure to parse near offset {pos}. Expected an ASCII digit."
            );
        }

        // ReSharper disable once InconsistentNaming
        static void ThrowInputStringNotInCorrectFormat_EndsPrematurely(int pos)
        {
            throw new FormatException(
                $"Input string was not in a correct format. Failure to parse near offset {pos}. Format item ends prematurely."
            );
        }

    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char Peek()
    {
        var index = _pos-1;
        return index > 0 ? _chars[index] : '\0';
    }
}