using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Maxine.Extensions;

public static class LinqExtensions_GenericMath
{
    public static T Sum<T>(this IEnumerable<T> source) where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
    {
        if (typeof(T) == typeof(long)) return (T)(object)Enumerable.Sum((IEnumerable<long>)source);
        if (typeof(T) == typeof(int)) return (T)(object)Enumerable.Sum((IEnumerable<int>)source);
        if (typeof(T) == typeof(float)) return (T)(object)Enumerable.Sum((IEnumerable<float>)source);
        if (typeof(T) == typeof(double)) return (T)(object)Enumerable.Sum((IEnumerable<double>)source);
        if (typeof(T) == typeof(decimal)) return (T)(object)Enumerable.Sum((IEnumerable<decimal>)source);

        return source.Aggregate(T.AdditiveIdentity, static (a, b) => a + b);
    }

    public static IEnumerable<T> Range<T, TStep>(T start, T end, TStep step) where T : IAdditionOperators<T, TStep, T>, IComparisonOperators<T, T, bool> where TStep : notnull
    {
        for (var i = start; i < end; i += step)
        {
            yield return i;
        }
    }

    public static IEnumerable<T> Range<T>(T start, T end, T step) where T : IAdditionOperators<T, T, T>, IComparisonOperators<T, T, bool>
    {
        for (var i = start; i < end; i += step)
        {
            yield return i;
        }
    }

    public static T Average<T>(this IEnumerable<T> source) where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IDivisionOperators<T, T, T>, INumberBase<T>
    {
        if (typeof(T) == typeof(long)) return (T)(object)Enumerable.Average((IEnumerable<long>)source);
        if (typeof(T) == typeof(int)) return (T)(object)Enumerable.Average((IEnumerable<int>)source);
        if (typeof(T) == typeof(float)) return (T)(object)Enumerable.Average((IEnumerable<float>)source);
        if (typeof(T) == typeof(double)) return (T)(object)Enumerable.Average((IEnumerable<double>)source);
        if (typeof(T) == typeof(decimal)) return (T)(object)Enumerable.Average((IEnumerable<decimal>)source);

        var (val, count) = source.Aggregate((val: T.AdditiveIdentity, count: 0), static (acc, b) => (acc.val + b, acc.count+1));
        return val / T.CreateChecked(count);
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
}
