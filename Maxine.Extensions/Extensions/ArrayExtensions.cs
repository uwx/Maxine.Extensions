namespace Maxine.Extensions;

public static class ArrayExtensions
{
    public static T[] Replace<T>(this T[] arr, ReadOnlySpan<T> find, ReadOnlySpan<T> replace) where T : IEquatable<T>?
    {
        // If find pattern is empty, return original array unchanged
        if (find.IsEmpty)
        {
            return arr;
        }

        using var builder = new ValueArrayBuilder<T>(arr.Length);

        var span = arr.AsSpan();
        while (span.IndexOf(find) is var idx and not -1)
        {
            builder.Append(span[..idx]);
            builder.Append(replace);
            span = span[(idx + find.Length)..];
        }

        builder.Append(span);

        return builder.AsSpan().ToArray();
    }
}