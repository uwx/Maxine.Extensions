namespace Maxine.Extensions.SpanExtensions;

/// <summary> 
/// Supports iteration over a <see cref="ReadOnlySpan{Char}"/> by splitting it at specified delimiters and based on specified <see cref="StringSplitOptions"/>.  
/// </summary>   
public ref struct SpanSplitAnyStringSplitOptionsEnumerator(
    ReadOnlySpan<char> span,
    ReadOnlySpan<char> delimiters,
    StringSplitOptions options)
{
    private ReadOnlySpan<char> _span = span;
    private readonly ReadOnlySpan<char> _delimiters = delimiters;

    /// <summary>
    /// Gets the element in the collection at the current position of the enumerator. 
    /// </summary>
    public ReadOnlySpan<char> Current { get; internal set; } = default;

    public SpanSplitAnyStringSplitOptionsEnumerator GetEnumerator()
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
        var index = span.IndexOfAny(_delimiters);

        if(index == -1 || index >= span.Length)
        {
            _span = ReadOnlySpan<char>.Empty;
            Current = span;
            return true;
        }
        Current = span[..index];

#if NET5_0_OR_GREATER
        if(options.HasFlag(StringSplitOptions.TrimEntries))
        {
            Current = Current.Trim();
        }
#endif
        if(options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
        {
            if(Current.IsEmpty)
            {
                _span = span[(index + 1)..];
                return MoveNext();
            }
        }
        _span = span[(index + 1)..];
        return true;
    }

}