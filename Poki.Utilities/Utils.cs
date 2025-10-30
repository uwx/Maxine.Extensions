namespace Poki.Utilities;

public static class Utils
{
    /// <summary>
    /// Splits an array into sections of, at maximum, length <paramref name="length"/>.
    /// </summary>
    /// <param name="source">The source array</param>
    /// <param name="length">The maximum length of each section</param>
    /// <typeparam name="T">The type of the array</typeparam>
    /// <returns>A list containing each section</returns>
    /// <seealso cref="Enumerable.Chunk{TSource}"/>
    public static T[][] MakeChunks<T>(T[] source, int length)
    {
        var chunkCount = (source.Length / length) + (source.Length % length != 0 ? 1 : 0);
        
        var chunks = new T[chunkCount][];
        for (int i = 0, c = 0; i < source.Length; i += length, c++)
        {
            chunks[c] = source[i..Math.Min(i + length, source.Length)];
        }

        return chunks;
    }
    
    /// <summary>
    /// Trims a string to a maximum size and adds ellipses to the end if necessary.
    /// </summary>
    /// <param name="str">The input string</param>
    /// <param name="size">The length, in characters</param>
    /// <param name="useNewline">Whether or not to put a newline before the ellipsis</param>
    /// <returns>The trimmed string</returns>
    public static string TrimToSize(string str, int size, bool useNewline = false)
    {
        if (str.Length < size)
        {
            return str;
        }

        var ellipsis = useNewline ? "\n..." : "...";
        return str[..Math.Min(size - ellipsis.Length, str.Length)] + ellipsis;
    }
}