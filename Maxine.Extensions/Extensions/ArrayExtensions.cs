namespace Maxine.Extensions;

public static class ArrayExtensions
{
    public static T[] Replace<T>(this T[] arr, ReadOnlySpan<T> find, ReadOnlySpan<T> replace) where T : IEquatable<T>?
    {
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