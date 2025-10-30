namespace Maxine.Extensions.SpanExtensions;

/// <summary>
/// Supports iteration over a <see cref="ReadOnlySpan{T}"/> by splitting it at specified delimiters of type <typeparamref name="T"/> with an upper limit of splits performed.  
/// </summary>
/// <typeparam name="T">The type of elements in the enumerated <see cref="ReadOnlySpan{T}"/></typeparam>  
public ref struct SpanSplitAnyWithCountEnumerator<T>(ReadOnlySpan<T> span, ReadOnlySpan<T> delimiters, int count)
    where T : IEquatable<T>
{
    private ReadOnlySpan<T> _span = span;
    private readonly ReadOnlySpan<T> _delimiters = delimiters;
    private int _currentCount = 0;

    /// <summary>
    /// Gets the element in the collection at the current position of the enumerator. 
    /// </summary>
    public ReadOnlySpan<T> Current { get; internal set; } = default;

    public SpanSplitAnyWithCountEnumerator<T> GetEnumerator()
    {
        return this;
    }

    /// <summary>
    /// Advances the enumerator to the next element of the collection.
    /// </summary>
    /// <returns><code>true</code> if the enumerator was successfully advanced to the next element; <code>false</code> if the enumerator has passed the end of the collection.</returns>
    public bool MoveNext()
    {
        var span = _span;
        if(span.IsEmpty)
        {
            return false;
        }
        if(_currentCount == count)
        {
            return false;
        }
        var index = span.IndexOfAny(_delimiters);
        if(index == -1 || index >= span.Length)
        {
            _span = ReadOnlySpan<T>.Empty;
            Current = span;
            return true;
        }
        _currentCount++;
        Current = span[..index];
        _span = span[(index + 1)..];
        return true;
    }

}