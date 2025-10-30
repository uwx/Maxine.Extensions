namespace Maxine.Extensions.SpanExtensions;

/// <summary>
/// Supports iteration over a <see cref="ReadOnlySpan{T}"/> by splitting it at a specified delimiter of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of elements in the enumerated <see cref="ReadOnlySpan{T}"/></typeparam>
public ref struct SpanSplitEnumerator<T>(ReadOnlySpan<T> source, T delimiter) where T : IEquatable<T>
{
    private ReadOnlySpan<T> _span = source;

    /// <summary>
    /// Gets the element in the collection at the current position of the enumerator. 
    /// </summary>
    public ReadOnlySpan<T> Current { get; internal set; } = default;

    public SpanSplitEnumerator<T> GetEnumerator()
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
        var index = span.IndexOf(delimiter);

        if(index == -1 || index >= span.Length)
        {
            _span = ReadOnlySpan<T>.Empty;
            Current = span;
            return true;
        }
        Current = span[..index];
        _span = span[(index + 1)..];
        return true;
    }
}