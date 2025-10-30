namespace Maxine.Extensions.SpanExtensions;

public static partial class ReadOnlySpanExtensions
{
    public static bool StartsWithAnySequence<T>(this scoped ReadOnlySpan<T> buf, scoped ReadOnlySpan<T> prefix) where T : IEquatable<T>?
        => buf.StartsWith(prefix);

    public static bool StartsWithAnySequence<T>(this scoped ReadOnlySpan<T> buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2) where T : IEquatable<T>?
        => buf.StartsWith(prefix1) || buf.StartsWith(prefix2);

    public static bool StartsWithAnySequence<T>(this scoped ReadOnlySpan<T> buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2, scoped ReadOnlySpan<T> prefix3) where T : IEquatable<T>?
        => buf.StartsWith(prefix1) || buf.StartsWith(prefix2) || buf.StartsWith(prefix3);

    public static bool StartsWithAnySequence<T>(this scoped ReadOnlySpan<T> buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2, scoped ReadOnlySpan<T> prefix3, scoped ReadOnlySpan<T> prefix4) where T : IEquatable<T>?
        => buf.StartsWith(prefix1) || buf.StartsWith(prefix2) || buf.StartsWith(prefix3) || buf.StartsWith(prefix4);

    public static bool StartsWithAnySequence<T>(this scoped ReadOnlySpan<T> buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2, scoped ReadOnlySpan<T> prefix3, scoped ReadOnlySpan<T> prefix4, scoped ReadOnlySpan<T> prefix5) where T : IEquatable<T>?
        => buf.StartsWith(prefix1) || buf.StartsWith(prefix2) || buf.StartsWith(prefix3) || buf.StartsWith(prefix4) || buf.StartsWith(prefix5);
}