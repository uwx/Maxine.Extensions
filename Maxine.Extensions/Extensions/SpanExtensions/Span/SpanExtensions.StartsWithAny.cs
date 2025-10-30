namespace Maxine.Extensions.SpanExtensions;

public static partial class SpanExtensions
{
    public static bool StartsWithAnySequence<T>(this Span<T> buf, ReadOnlySpan<T> prefix) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix);

    public static bool StartsWithAnySequence<T>(this Span<T> buf, ReadOnlySpan<T> prefix1, ReadOnlySpan<T> prefix2) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2);

    public static bool StartsWithAnySequence<T>(this Span<T> buf, ReadOnlySpan<T> prefix1, ReadOnlySpan<T> prefix2, ReadOnlySpan<T> prefix3) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3);

    public static bool StartsWithAnySequence<T>(this Span<T> buf, ReadOnlySpan<T> prefix1, ReadOnlySpan<T> prefix2, ReadOnlySpan<T> prefix3, ReadOnlySpan<T> prefix4) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3, prefix4);

    public static bool StartsWithAnySequence<T>(this Span<T> buf, ReadOnlySpan<T> prefix1, ReadOnlySpan<T> prefix2, ReadOnlySpan<T> prefix3, ReadOnlySpan<T> prefix4, ReadOnlySpan<T> prefix5) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3, prefix4, prefix5);
    
    public static bool StartsWithAnySequence<T>(this T[] buf, ReadOnlySpan<T> prefix) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix);

    public static bool StartsWithAnySequence<T>(this T[] buf, ReadOnlySpan<T> prefix1, ReadOnlySpan<T> prefix2) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2);

    public static bool StartsWithAnySequence<T>(this T[] buf, ReadOnlySpan<T> prefix1, ReadOnlySpan<T> prefix2, ReadOnlySpan<T> prefix3) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3);

    public static bool StartsWithAnySequence<T>(this T[] buf, ReadOnlySpan<T> prefix1, ReadOnlySpan<T> prefix2, ReadOnlySpan<T> prefix3, ReadOnlySpan<T> prefix4) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3, prefix4);

    public static bool StartsWithAnySequence<T>(this T[] buf, ReadOnlySpan<T> prefix1, ReadOnlySpan<T> prefix2, ReadOnlySpan<T> prefix3, ReadOnlySpan<T> prefix4, ReadOnlySpan<T> prefix5) where T : IEquatable<T>?
        => ((ReadOnlySpan<T>)buf).StartsWithAnySequence(prefix1, prefix2, prefix3, prefix4, prefix5);

}