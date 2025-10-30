namespace Maxine.Extensions.SpanExtensions;

/// <summary> 
/// Supports iteration over a <see cref="ReadOnlySpan{Char}"/> by splitting it at a specified delimiter and based on specified <see cref="StringSplitOptions"/>  with an upper limit of splits performed.  
/// </summary>   
public ref struct SpanSplitSequenceStringSplitOptionsWithCountEnumerator(
    ReadOnlySpan<char> span,
    ReadOnlySpan<char> delimiter,
    StringSplitOptions options,
    int count)
{
    private ReadOnlySpan<char> _span = span;
    private readonly ReadOnlySpan<char> _delimiter = delimiter;
    private int _currentCount = 0;

    /// <summary>
    /// Gets the element in the collection at the current position of the enumerator. 
    /// </summary>
    public ReadOnlySpan<char> Current { get; internal set; } = default;

    public SpanSplitSequenceStringSplitOptionsWithCountEnumerator GetEnumerator()
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
        var index = span.IndexOf(_delimiter);

        if(index == -1 || index >= span.Length)
        {
            _span = ReadOnlySpan<char>.Empty;
            Current = span;
            return true;
        }
        _currentCount++;
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
                _span = span[(index + _delimiter.Length)..];
                return MoveNext();
            }
        }
        _span = span[(index + _delimiter.Length)..];
        return true;
    }

}