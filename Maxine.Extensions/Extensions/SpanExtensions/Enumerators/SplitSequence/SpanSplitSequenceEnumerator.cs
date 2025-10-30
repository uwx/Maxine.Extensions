namespace Maxine.Extensions.SpanExtensions;

/// <summary> 
/// Supports iteration over a <see cref="ReadOnlySpan{Char}"/> by splitting it at specified delimiters and based on specified <see cref="StringSplitOptions"/>.  
/// </summary>   
public ref struct SpanSplitSequenceEnumerator<T>(ReadOnlySpan<T> span, ReadOnlySpan<T> delimiter)
    where T : IEquatable<T>
{
    private ReadOnlySpan<T> _span = span;
    private readonly ReadOnlySpan<T> _delimiter = delimiter;

    /// <summary>
    /// Gets the element in the collection at the current position of the enumerator. 
    /// </summary>
    public ReadOnlySpan<T> Current { get; internal set; } = default;

    public SpanSplitSequenceEnumerator<T> GetEnumerator()
    {
        return this;
    }

    /// <summary>
    /// Advances the enumerator to the next element of the collection.
    /// </summary>
    /// <returns><code>true</code> if the enumerator was successfully advanced to the next element; <code>false</code> if the enumerator has passed the end of the collection.</returns>
    public bool MoveNext()
    {
        ReadOnlySpan<T> span = _span;
        if(span.IsEmpty)
        {
            return false;
        }
        var index = span.IndexOf(_delimiter);

        if(index == -1 || index >= span.Length)
        {
            _span = ReadOnlySpan<T>.Empty;
            Current = span;
            return true;
        }
        Current = span[..index];
        _span = span[(index + _delimiter.Length)..];
        return true;
    }

}