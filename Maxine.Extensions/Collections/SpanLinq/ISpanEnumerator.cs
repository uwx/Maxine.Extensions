namespace Maxine.Extensions.Collections.SpanLinq;

public interface ISpanEnumerator<TSpan, out TElement>
    where TElement : allows ref struct
{
    TElement Current { get; }
    bool MoveNext(ReadOnlySpan<TSpan> span);
}