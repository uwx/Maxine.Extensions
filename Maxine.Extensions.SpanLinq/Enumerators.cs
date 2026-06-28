namespace Maxine.Extensions.Collections.SpanLinq;

/// <summary>
/// A span enumerator that just enumerates the span.
/// </summary>
public struct SpanEnumerator<TElement>() : ISpanEnumerator<TElement, TElement>
{
    private TElement _current = default!;
    private int _currentIndex;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TElement> span)
    {
        if (_currentIndex < span.Length)
        {
            _current = span[_currentIndex];
            _currentIndex++;
            return true;
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that appends an additional element
/// </summary>
public struct AppendEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    TElement element) : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private bool _appended;
    private TElement _current = default!;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_enumerator.MoveNext(span))
        {
            _current = _enumerator.Current;
            return true;
        }
        else if (!_appended)
        {
            _appended = true;
            _current = element;
            return true;
        }
        else
        {
            return false;
        }
    }
}

/// <summary>
/// A span enumerator that filters to a specific type.
/// </summary>
public struct CastEnumerator<TSpan, TElement, TEnumerator, TResult>(TEnumerator enumerator)
    : ISpanEnumerator<TSpan, TResult>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private TResult _current = default!;

    public TResult Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_enumerator.MoveNext(span))
        {
            if (_enumerator.Current is TResult tvalue)
            {
                _current = tvalue;
                return true;
            }
            else
            {
                throw Exceptions.GetElementCannotBeCastToType(typeof(TResult));
            }
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that produces chunks of elements
/// </summary>
public struct ChunkToArrayEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    int chunkSize) : ISpanEnumerator<TSpan, TElement[]>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private List<TElement>? _chunk;
    private TElement[] _current = default!;

    public TElement[] Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        while (_enumerator.MoveNext(span))
        {
            if (_chunk == null)
                _chunk = new List<TElement>(chunkSize);

            _chunk.Add(_enumerator.Current);

            if (_chunk.Count == chunkSize)
            {
                _current = _chunk.ToArray();
                _chunk.Clear();
                return true;
            }
        }

        if (_chunk != null && _chunk.Count > 0)
        {
            _current = _chunk.ToArray();
            _chunk.Clear();
            return true;
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that yields the concatenation of the
/// underlying enumerator and the list of elements.
/// </summary>
public struct ConcatEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    IEnumerable<TElement> elements) : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private IEnumerator<TElement>? _elementsEnumerator;
    private TElement _current = default!;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
    top:
        if (_elementsEnumerator != null)
        {
            if (_elementsEnumerator.MoveNext())
            {
                _current = _elementsEnumerator.Current;
                return true;
            }
        }
        else if (_enumerator.MoveNext(span))
        {
            _current = _enumerator.Current;
            return true;
        }
        else if (_elementsEnumerator == null)
        {
            _elementsEnumerator = elements.GetEnumerator();
            goto top;
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that yields an default item if the underlying enumerator is empty.
/// </summary>
public struct DefaultIfEmptyEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    TElement defaultValue) : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private bool _hadElements;
    private TElement _current = default!;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_enumerator.MoveNext(span))
        {
            _current = _enumerator.Current;
            _hadElements = true;
            return true;
        }
        else if (!_hadElements)
        {
            _hadElements = true;
            _current = defaultValue;
            return true;
        }
        else
        {
            return false;
        }
    }
}

/// <summary>
/// A span enumerator that yields only distinct values.
/// </summary>
public struct DistinctByEnumerator<TSpan, TElement, TEnumerator, TKey>(
    TEnumerator enumerator,
    Func<TElement, TKey> keySelector,
    IEqualityComparer<TKey> keyComparer)
    : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private HashSet<TKey>? _keys;
    private TElement _current = default!;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        while (_enumerator.MoveNext(span))
        {
            var key = keySelector(_enumerator.Current);

            if (_keys == null)
                _keys = new HashSet<TKey>(keyComparer);

            if (_keys.Add(key))
            {
                _current = _enumerator.Current;
                return true;
            }
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that enumerates only the elements not in the elements list.
/// </summary>
public struct ExceptByEnumerator<TSpan, TElement, TEnumerator, TKey>(
    TEnumerator enumerator,
    IEnumerable<TKey> keys,
    Func<TElement, TKey> keySelector,
    IEqualityComparer<TKey> keyComparer)
    : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private HashSet<TKey>? _excluded;
    private TElement _current = default!;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        while (_enumerator.MoveNext(span))
        {
            if (_excluded == null)
                _excluded = keys.ToHashSet(keyComparer);

            var key = keySelector(_enumerator.Current);
            if (!_excluded.Contains(key))
            {
                _current = _enumerator.Current;
                return true;
            }
        }

        return false;
    }
}


/// <summary>
/// A span enumerator that yields the group-by results.
/// </summary>
public struct GroupByEnumerator<TSpan, TElement, TEnumerator, TKey>(
    TEnumerator enumerator,
    Func<TElement, TKey> keySelector,
    IEqualityComparer<TKey> comparer)
    : ISpanEnumerator<TSpan, IGrouping<TKey, TElement>>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private IEnumerator<IGrouping<TKey, TElement>>? _groupEnumerator;
    private IGrouping<TKey, TElement> _current = default!;

    public IGrouping<TKey, TElement> Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_groupEnumerator == null)
        {
            var list = new List<TElement>();
            while (_enumerator.MoveNext(span))
            {
                list.Add(_enumerator.Current);
            }
            _groupEnumerator = list.GroupBy(keySelector, comparer).GetEnumerator();
        }

        if (_groupEnumerator.MoveNext())
        {
            _current = _groupEnumerator.Current;
            return true;
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that produces the group by result of another span enumerator
/// </summary>
public struct GroupJoinEnumerator<TSpan, TElement, TEnumerator, TInner, TKey, TResult>(
    TEnumerator enumerator,
    IEnumerable<TInner> inner,
    Func<TElement, TKey> outerKeySelector,
    Func<TInner, TKey> innerKeySelector,
    Func<TElement, IEnumerable<TInner>, TResult> resultSelector,
    IEqualityComparer<TKey> comparer)
    : ISpanEnumerator<TSpan, TResult>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private readonly IEqualityComparer<TKey> _comparer = comparer;
    private ILookup<TKey, TInner>? _innerLookup;
    private TResult _current = default!;

    public TResult Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_innerLookup == null)
            _innerLookup = inner.ToLookup(innerKeySelector);

        if (_enumerator.MoveNext(span))
        {
            var key = outerKeySelector(_enumerator.Current);
            var innerItems = _innerLookup[key];
            _current = resultSelector(_enumerator.Current, innerItems);
            return true;
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that yields only the elements in both elements lists.
/// </summary>
public struct IntersectByEnumerator<TSpan, TElement, TEnumerator, TKey>(
    TEnumerator enumerator,
    IEnumerable<TKey> keys,
    Func<TElement, TKey> keySelector,
    IEqualityComparer<TKey> keyComparer)
    : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private HashSet<TKey>? _keySet;
    private HashSet<TKey>? _yielded;
    private TElement _current = default!;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_keySet == null)
            _keySet = keys.ToHashSet(keyComparer);

        if (_yielded == null)
            _yielded = new HashSet<TKey>();

        while (_enumerator.MoveNext(span))
        {
            var key = keySelector(_enumerator.Current);
            if (_keySet.Contains(key) && _yielded.Add(key))
            {
                _current = _enumerator.Current;
                return true;
            }
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that enumerates only the elements not in the elements list.
/// </summary>
public struct JoinEnumerator<TSpan, TElement, TEnumerator, TInner, TKey, TResult>(
    TEnumerator enumerator,
    IEnumerable<TInner> inner,
    Func<TElement, TKey> outerKeySelector,
    Func<TInner, TKey> innerKeySelector,
    Func<TElement, TInner, TResult> resultSelector,
    IEqualityComparer<TKey> keyComparer)
    : ISpanEnumerator<TSpan, TResult>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private ILookup<TKey, TInner>? _innerLookup;
    private IEnumerator<TInner>? _innerEnumerator;
    private TElement _currentOuter = default!;
    private TResult _currentResult = default!;

    public TResult Current => _currentResult;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        while (true)
        {
            if (_innerEnumerator != null
                && _innerEnumerator.MoveNext())
            {
                _currentResult = resultSelector(_currentOuter, _innerEnumerator.Current);
                return true;
            }

            if (_enumerator.MoveNext(span))
            {
                if (_innerLookup == null)
                    _innerLookup = inner.ToLookup(innerKeySelector, keyComparer);

                _innerEnumerator = _innerLookup[outerKeySelector(_enumerator.Current)].GetEnumerator();
            }
            else
            {
                return false;
            }
        }
    }
}

/// <summary>
/// A span enumerator that filters to a specific type.
/// </summary>
public struct OfTypeEnumerator<TSpan, TElement, TEnumerator, TResult>(TEnumerator enumerator)
    : ISpanEnumerator<TSpan, TResult>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private TResult _current = default!;

    public TResult Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        while (_enumerator.MoveNext(span))
        {
            if (_enumerator.Current is TResult tvalue)
            {
                _current = tvalue;
                return true;
            }
        }

        return false;
    }
}

public struct OrderByEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    Comparison<TElement> comparison)
    : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    internal TEnumerator _enumerator = enumerator;
    internal readonly Comparison<TElement> _comparison = comparison;
    private IEnumerator<TElement>? _listEnumerator;
    private TElement _current = default!;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_listEnumerator == null)
        {
            var list = new List<TElement>();
            while (_enumerator.MoveNext(span))
            {
                list.Add(_enumerator.Current);
            }
            list.Sort(_comparison);
            _listEnumerator = list.GetEnumerator();
        }

        if (_listEnumerator.MoveNext())
        {
            _current = _listEnumerator.Current;
            return true;
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that prepends an additional element
/// </summary>
public struct PrependSpanEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    TElement element) : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private bool _prepended;
    private TElement _current = default!;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (!_prepended)
        {
            _prepended = true;
            _current = element;
            return true;
        }
        else if (_enumerator.MoveNext(span))
        {
            _current = _enumerator.Current;
            return true;
        }
        else
        {
            return false;
        }
    }
}

/// <summary>
/// A span enumerator that yields the elements in reverse.
/// </summary>
public struct ReverseEnumerator<TSpan, TElement, TEnumerator>(TEnumerator enumerator) : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private Stack<TElement>? _stack;
    private TElement _current = default!;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_stack == null)
        {
            _stack = new Stack<TElement>();
            while (_enumerator.MoveNext(span))
            {
                _stack.Push(_enumerator.Current);
            }
        }

        if (_stack.Count > 0)
        {
            _current = _stack.Pop();
            return true;
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that converts the element values of another span enumerator.
/// </summary>
public struct SelectEnumerator<TSpan, TElement, TEnumerator, TResult>(
    TEnumerator enumerator,
    Func<TElement, int, TResult> selector)
    : ISpanEnumerator<TSpan, TResult>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private TResult _current = default!;
    private int _currentIndex = -1;

    public TResult Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_enumerator.MoveNext(span))
        {
            _currentIndex++;
            _current = selector(_enumerator.Current, _currentIndex);
            return true;
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that yields the many elements associated with each element.
/// </summary>
public struct SelectManyEnumerator<TSpan, TElement, TEnumerator, TCollection, TResult>(
    TEnumerator enumerator,
    Func<TElement, int, IEnumerable<TCollection>> collectionSelector,
    Func<TElement, TCollection, TResult> resultSelector)
    : ISpanEnumerator<TSpan, TResult>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private IEnumerator<TCollection>? _collectionEnumerator = default;
    private TResult _current = default!;
    private int _currentIndex = -1;

    public TResult Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        while (true)
        {
            if (_collectionEnumerator == null)
            {
                if (!_enumerator.MoveNext(span))
                    return false;
                _currentIndex++;               
                _collectionEnumerator = collectionSelector(_enumerator.Current, _currentIndex).GetEnumerator();
            }

            if (_collectionEnumerator.MoveNext())
            {
                _current = resultSelector(_enumerator.Current, _collectionEnumerator.Current);
                return true;
            }

            _collectionEnumerator = null;
        }
    }
}


/// <summary>
/// A span enumerator that skips the first N elements.
/// </summary>
public struct SkipEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    int count) : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private TElement _current = default!;
    private int _skipped;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        while (_enumerator.MoveNext(span))
        {
            if (_skipped < count)
            {
                _skipped++;
                continue;
            }
            else
            {
                _current = _enumerator.Current;
                return true;
            }
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that skips the last N elements.
/// </summary>
public struct SkipLastEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    int count) : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private TElement _current = default!;
    private Queue<TElement>? _queue;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_queue == null)
            _queue = new Queue<TElement>();

        while (_enumerator.MoveNext(span))
        {
            _queue.Enqueue(_enumerator.Current);
            if (_queue.Count > count)
            {
                _current = _queue.Dequeue();
                return true;
            }
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that skips the first elements while the condition is true.
/// </summary>
public struct SkipWhileEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    Func<TElement, int, bool> predicate)
    : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private TElement _current = default!;
    private int _nextIndex;
    private bool _doneSkipping;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        while (_enumerator.MoveNext(span))
        {
            if (!_doneSkipping)
            {
                if (predicate(_enumerator.Current, _nextIndex++))
                    continue;
                _doneSkipping = true;
            }

            _current = _enumerator.Current;
            return true;
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that yields only the first N elements.
/// </summary>
public struct TakeEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    int count) : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private TElement _current = default!;
    private int _taken;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_taken < count
            && _enumerator.MoveNext(span))
        {
            _taken++;
            _current = _enumerator.Current;
            return true;
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that yields on the N elements.
/// </summary>
public struct TakeLastEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    int count) : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private TElement _current = default!;
    private Queue<TElement>? _queue;
    private bool atEnd;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (!atEnd)
        {
            if (_queue == null)
                _queue = new Queue<TElement>();

            while (_enumerator.MoveNext(span))
            {
                _queue.Enqueue(_enumerator.Current);
                if (_queue.Count > count)
                {
                    _current = _queue.Dequeue();
                    continue;
                }
            }

            atEnd = true;
        }

        if (_queue != null && _queue.Count > 0)
        {
            _current = _queue.Dequeue();
            return true;
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that yields the first elements while the condition is true.
/// </summary>
public struct TakeWhileEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    Func<TElement, int, bool> predicate)
    : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private TElement _current = default!;
    private int _nextIndex;
    private bool _doneTaking;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (!_doneTaking && _enumerator.MoveNext(span))
        {
            if (predicate(_enumerator.Current, _nextIndex++))
            {
                _current = _enumerator.Current;
                return true;
            }
            else
            {
                _doneTaking = true;
                return false;
            }
        }

        return false;
    }
}


/// <summary>
/// A span enumerator that filters a span enumerator
/// </summary>
public struct WhereEnumerator<TSpan, TElement, TEnumerator>(
    TEnumerator enumerator,
    Func<TElement, int, bool> predicate)
    : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private TElement _current = default!;
    private int _currentIndex = -1;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        while (_enumerator.MoveNext(span))
        {
            _currentIndex++;
            if (predicate(_enumerator.Current, _currentIndex))
            {
                _current = _enumerator.Current;
                return true;
            }
        }

        return false;
    }
}

/// <summary>
/// A span enumerator that enumerates the elements that appear in eiter list.
/// </summary>
public struct UnionByEnumerator<TSpan, TElement, TEnumerator, TKey>(
    TEnumerator enumerator,
    IEnumerable<TElement> elements,
    Func<TElement, TKey> keySelector,
    IEqualityComparer<TKey> keyComparer)
    : ISpanEnumerator<TSpan, TElement>
    where TEnumerator : ISpanEnumerator<TSpan, TElement>
{
    private TEnumerator _enumerator = enumerator;
    private readonly IEqualityComparer<TKey> _keyComparer = keyComparer;
    private HashSet<TKey>? _returned;
    private IEnumerator<TElement>? _elementsEnumerator;
    private TElement _current = default!;

    public TElement Current => _current;

    public bool MoveNext(ReadOnlySpan<TSpan> span)
    {
        if (_returned == null)
            _returned = new HashSet<TKey>();

        while (true)
        {
            if (_elementsEnumerator != null)
            {
                while (_elementsEnumerator.MoveNext())
                {
                    var key = keySelector(_elementsEnumerator.Current);
                    if (_returned.Add(key))
                    {
                        _current = _elementsEnumerator.Current;
                        return true;
                    }
                }

                return false;
            }

            while (_enumerator.MoveNext(span))
            {
                var key = keySelector(_enumerator.Current);
                if (_returned.Add(key))
                {
                    _current = _enumerator.Current;
                    return true;
                }
            }

            _elementsEnumerator = elements.GetEnumerator();
        }
    }
}