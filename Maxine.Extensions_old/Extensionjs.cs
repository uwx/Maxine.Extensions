using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.TU.Sims4Exporter;

public static class Extensionjs
{
    public static string Truncate(this string str, int length)
    {
        return str.Length <= length ? str : str[..length];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerateWithIndex<T> WithIndex<T>(this IEnumerable<T> enumerable, int start = 0)
        => new(enumerable, start);

    public readonly struct EnumerateWithIndex<T> : IEnumerable<(int Index, T Element)>
    {
        private readonly IEnumerable<T> _enumerable;
        private readonly int _start;

        internal EnumerateWithIndex(IEnumerable<T> enumerable, int start = 0)
        {
            _enumerable = enumerable;
            _start = start-1;
        }

        public struct EnumeratorWithIndex : IEnumerator<(int Index, T Element)>
        {
            private readonly IEnumerator<T> _enumerator;
            private int _index;

            internal EnumeratorWithIndex(IEnumerator<T> enumerator, int start = -1)
            {
                _enumerator = enumerator;
                _index = start;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                var v = _enumerator.MoveNext();
                if (!v) return false;

                _index++;
                return true;

            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _enumerator.Reset();
                _index = -1;
            }

            public (int Index, T Element) Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => (_index, _enumerator.Current);
            }

            object IEnumerator.Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Current;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void IDisposable.Dispose()
            {
                _enumerator.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EnumeratorWithIndex GetEnumerator() => new(_enumerable.GetEnumerator(), _start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<(int Index, T Element)> IEnumerable<(int Index, T Element)>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public static int FindIndex2<T>(this IEnumerable<T> items, Predicate<T> predicate)
    {
        var index = 0;
        foreach (var obj in items)
        {
            if (predicate(obj))
                return index;
            index++;
        }
        return -1;
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
}