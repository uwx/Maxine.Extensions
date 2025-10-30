using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions;

public static class StringExtensions
{
    public static string StripLeadingBom(this string str) => str.TrimStart('\uFEFF', '\u200B');
    public static ReadOnlySpan<char> StripLeadingBom(this ReadOnlySpan<char> str) => str.TrimStart(['\uFEFF', '\u200B']);
    public static Span<char> StripLeadingBom(this Span<char> str) => str.TrimStart(['\uFEFF', '\u200B']);

    public static string Truncate(this string str, int length)
    {
        return str.Length <= length ? str : str[..length];
    }
    
    public static SplitEnumeratorArgs1 SplitEnumerator(this string s, char c) => new(s, c);

    public static SplitEnumeratorArgsN SplitEnumerator(this string s, ReadOnlySpan<char> c) => new(s, c);

    public static SplitEnumeratorArgs1WithOptions SplitEnumerator(this string s, char c, StringSplitOptions options) => new(s, c, options);

    public static SplitEnumeratorArgsNWithOptions SplitEnumerator(this string s, ReadOnlySpan<char> c, StringSplitOptions options) => new(s, c, options);

    public static SplitEnumeratorArgsN SplitEnumerator(this string s, params char[] c) => new(s, c);

    public static SplitEnumeratorArgs1 SplitEnumerator(this ReadOnlySpan<char> s, char c) => new(s, c);

    public static SplitEnumeratorArgsN SplitEnumerator(this ReadOnlySpan<char> s, ReadOnlySpan<char> c) => new(s, c);

    public static SplitEnumeratorArgs1WithOptions SplitEnumerator(this ReadOnlySpan<char> s, char c, StringSplitOptions options) => new(s, c, options);

    public static SplitEnumeratorArgsNWithOptions SplitEnumerator(this ReadOnlySpan<char> s, ReadOnlySpan<char> c, StringSplitOptions options) => new(s, c, options);

    public static SplitEnumeratorArgsN SplitEnumerator(this ReadOnlySpan<char> s, params char[] c) => new(s, c);
    
    public static SplitEnumeratorArgs1 SplitEnumerator(this Span<char> s, char c) => new(s, c);

    public static SplitEnumeratorArgsN SplitEnumerator(this Span<char> s, ReadOnlySpan<char> c) => new(s, c);

    public static SplitEnumeratorArgs1WithOptions SplitEnumerator(this Span<char> s, char c, StringSplitOptions options) => new(s, c, options);

    public static SplitEnumeratorArgsNWithOptions SplitEnumerator(this Span<char> s, ReadOnlySpan<char> c, StringSplitOptions options) => new(s, c, options);

    public static SplitEnumeratorArgsN SplitEnumerator(this Span<char> s, params char[] c) => new(s, c);

    public ref struct SplitEnumeratorArgs1
    {
        private int _c1;
        private readonly ReadOnlySpan<char> _s;
        private readonly char _c;
        private int _pos;

        internal SplitEnumeratorArgs1(ReadOnlySpan<char> s, char c)
        {
            _s = s;
            _c = c;
            _pos = 0;
            _c1 = 0;
        }

        public SplitEnumeratorArgs1 GetEnumerator() => this;

        public bool MoveNext()
        {
            if (_pos == -1)
            {
                return false;
            }

            var oldpos = _pos;
            var pos = _pos = _s[(_pos + 1)..].IndexOf(_c);
            if (pos != -1)
            {
                _c1 = oldpos + 1;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _pos = 0;
        }
        
        public ReadOnlySpan<char> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _s[_c1.._pos];
        }
    }
    
    public ref struct SplitEnumeratorArgsN
    {
        private int _c1;
        private readonly ReadOnlySpan<char> _s;
        private readonly ReadOnlySpan<char> _c;
        private int _pos;

        internal SplitEnumeratorArgsN(ReadOnlySpan<char> s, ReadOnlySpan<char> c)
        {
            _s = s;
            _c = c;
            _pos = 0;
            _c1 = 0;
        }

        public SplitEnumeratorArgsN GetEnumerator() => this;

        public bool MoveNext()
        {
            if (_pos == -1)
            {
                return false;
            }

            var oldpos = _pos;
            var pos = _pos = _s[(_pos + 1)..].IndexOfAny(_c);
            if (pos != -1)
            {
                _c1 = oldpos + 1;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _pos = 0;
        }

        public ReadOnlySpan<char> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _s[_c1.._pos];
        }
    }
    
    public ref struct SplitEnumeratorArgs1WithOptions
    {
        private int _c1;
        private readonly ReadOnlySpan<char> _s;
        private readonly char _c;
        private readonly StringSplitOptions _options;
        private int _pos;

        internal SplitEnumeratorArgs1WithOptions(ReadOnlySpan<char> s, char c, StringSplitOptions options)
        {
            _s = s;
            _c = c;
            _options = options;
            _pos = 0;
            _c1 = 0;
        }

        public SplitEnumeratorArgs1WithOptions GetEnumerator() => this;

        public bool MoveNext()
        {
            if (_pos == -1)
            {
                return false;
            }

            do
            {
                var oldpos = _pos;
                var pos = _pos = _s[(_pos + 1)..].IndexOf(_c);
                if (pos != -1)
                {
                    _c1 = oldpos + 1;
                    continue;
                }

                return false;
            } while ((_options & StringSplitOptions.RemoveEmptyEntries) != 0 && Current.IsEmpty);

            return true;
        }

        public void Reset()
        {
            _pos = 0;
        }

        public ReadOnlySpan<char> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var v = _s[_c1.._pos];
                return (_options & StringSplitOptions.TrimEntries) != 0 ? v.Trim() : v;
            }
        }
    }
    
    public ref struct SplitEnumeratorArgsNWithOptions
    {
        private int _c1;
        private readonly ReadOnlySpan<char> _s;
        private readonly ReadOnlySpan<char> _c;
        private readonly StringSplitOptions _options;
        private int _pos;

        internal SplitEnumeratorArgsNWithOptions(ReadOnlySpan<char> s, ReadOnlySpan<char> c, StringSplitOptions options)
        {
            _s = s;
            _c = c;
            _options = options;
            _pos = 0;
            _c1 = 0;
        }

        public SplitEnumeratorArgsNWithOptions GetEnumerator() => this;

        public bool MoveNext()
        {
            if (_pos == -1)
            {
                return false;
            }

            do
            {
                var oldpos = _pos;
                var pos = _pos = _s[(_pos + 1)..].IndexOfAny(_c);
                if (pos != -1)
                {
                    _c1 = oldpos + 1;
                    continue;
                }

                return false;
            } while ((_options & StringSplitOptions.RemoveEmptyEntries) != 0 && Current.IsEmpty);

            return true;
        }

        public void Reset()
        {
            _pos = 0;
        }

        public ReadOnlySpan<char> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var v = _s[_c1.._pos];
                return (_options & StringSplitOptions.TrimEntries) != 0 ? v.Trim() : v;
            }
        }
    }
    
    [InlineArray(2)]
    private struct Buffer2<T>
    {
        internal T? Arg0;

        public ReadOnlySpan<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateReadOnlySpan(ref Arg0, 2)!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer2(T arg0, T arg1)
        {
            this[0] = arg0;
            this[1] = arg1;
        }
    }

    [InlineArray(3)]
    private struct Buffer3<T>
    {
        internal T? Arg0;

        public ReadOnlySpan<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateReadOnlySpan(ref Arg0, 3)!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer3(T arg0, T arg1, T arg2)
        {
            this[0] = arg0;
            this[1] = arg1;
            this[2] = arg2;
        }
    }

    public static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span, char trimChar1, char trimChar2)
        => span.TrimEnd(new Buffer2<char>(trimChar1, trimChar2).Span);

    public static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span, char trimChar1, char trimChar2, char trimChar3)
        => span.TrimEnd(new Buffer3<char>(trimChar1, trimChar2, trimChar3).Span);
    
    public static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span, char trimChar1, char trimChar2)
        => span.TrimStart(new Buffer2<char>(trimChar1, trimChar2).Span);

    public static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span, char trimChar1, char trimChar2, char trimChar3)
        => span.TrimStart(new Buffer3<char>(trimChar1, trimChar2, trimChar3).Span);
    
    public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span, char trimChar1, char trimChar2)
        => span.Trim(new Buffer2<char>(trimChar1, trimChar2).Span);

    public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span, char trimChar1, char trimChar2, char trimChar3)
        => span.Trim(new Buffer3<char>(trimChar1, trimChar2, trimChar3).Span);
    
    public static bool EqualsIgnoreCase(this string? a, string? b)
    {
        if (a == null) return b == null;
        return a.Equals(b, StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Removes a suffix from a given string if it is present in the string; returns the unmodified string otherwise.
    /// </summary>
    /// <param name="str">The input string</param>
    /// <param name="suffix">The substring representing the suffix to remove</param>
    /// <returns>
    /// A string without the suffix <paramref name="suffix"/>, or the unmodified string if it didn't have one
    /// </returns>
    public static string RemoveFromEnd(this string str, string suffix)
    {
        return str.EndsWith(suffix) ? str[..^suffix.Length] : str;
    }
    
    /// <summary>
    /// Removes a prefix from a given string if it is present in the string; returns the unmodified string otherwise.
    /// </summary>
    /// <param name="str">The input string</param>
    /// <param name="prefix">The substring representing the prefix to remove</param>
    /// <returns>
    /// A string without the prefix <paramref name="prefix"/>, or the unmodified string if it didn't have one
    /// </returns>
    public static string RemoveFromStart(this string str, string prefix)
    {
        return str.StartsWith(prefix) ? str[prefix.Length..] : str;
    }
}