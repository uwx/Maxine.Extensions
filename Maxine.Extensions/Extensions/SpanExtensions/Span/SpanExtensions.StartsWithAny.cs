namespace Maxine.Extensions.SpanExtensions;

public static partial class SpanExtensions
{
    public static bool StartsWithAnySequence<T>(this scoped Span<T> buf, scoped ReadOnlySpan<T> prefix) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix);

    public static bool StartsWithAnySequence<T>(this scoped Span<T> buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2);

    public static bool StartsWithAnySequence<T>(this scoped Span<T> buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2, scoped ReadOnlySpan<T> prefix3) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3);

    public static bool StartsWithAnySequence<T>(this scoped Span<T> buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2, scoped ReadOnlySpan<T> prefix3, scoped ReadOnlySpan<T> prefix4) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3, prefix4);

    public static bool StartsWithAnySequence<T>(this scoped Span<T> buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2, scoped ReadOnlySpan<T> prefix3, scoped ReadOnlySpan<T> prefix4, scoped ReadOnlySpan<T> prefix5) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3, prefix4, prefix5);
    
    public static bool StartsWithAnySequence<T>(this T[] buf, scoped ReadOnlySpan<T> prefix) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix);

    public static bool StartsWithAnySequence<T>(this T[] buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2);

    public static bool StartsWithAnySequence<T>(this T[] buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2, scoped ReadOnlySpan<T> prefix3) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3);

    public static bool StartsWithAnySequence<T>(this T[] buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2, scoped ReadOnlySpan<T> prefix3, scoped ReadOnlySpan<T> prefix4) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3, prefix4);

    public static bool StartsWithAnySequence<T>(this T[] buf, scoped ReadOnlySpan<T> prefix1, scoped ReadOnlySpan<T> prefix2, scoped ReadOnlySpan<T> prefix3, scoped ReadOnlySpan<T> prefix4, scoped ReadOnlySpan<T> prefix5) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3, prefix4, prefix5);

}