using Microsoft.Extensions.Primitives;

namespace Maxine.Extensions;

public static class StringSegmentExtensions
{
    /// <summary>
    /// Removes all leading and trailing whitespaces.
    /// </summary>
    /// <returns>The trimmed <see cref="StringSegment"/>.</returns>
    public static StringSegment Trim(this StringSegment segment, char c) => segment.TrimStart(c).TrimEnd(c);

    /// <summary>
    /// Removes all leading whitespaces.
    /// </summary>
    /// <returns>The trimmed <see cref="StringSegment"/>.</returns>
    public static StringSegment TrimStart(this StringSegment segment, char c)
    {
        var span = segment.AsSpan();

        int i;
        for (i = 0; i < span.Length; i++)
        {
            if (span[i] != c)
            {
                break;
            }
        }

        return segment.Subsegment(i);
    }

    /// <summary>
    /// Removes all trailing whitespaces.
    /// </summary>
    /// <returns>The trimmed <see cref="StringSegment"/>.</returns>
    public static StringSegment TrimEnd(this StringSegment segment, char c)
    {
        var span = segment.AsSpan();

        int i;
        for (i = span.Length - 1; i >= 0; i--)
        {
            if (span[i] != c)
            {
                break;
            }
        }

        return segment.Subsegment(0, i + 1);
    }
}