namespace Maxine.Extensions;

// https://stackoverflow.com/a/12389412
public static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<TValue[]> ChunkAsync<TValue>(
        this IAsyncEnumerable<TValue> values,
        int chunkSize
    )
    {
        var list = new List<TValue>();
        await using var enumerator = values.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync())
        {
            var chunkSizeTemp = chunkSize;
            do
            {
                list.Add(enumerator.Current);
            } while (--chunkSizeTemp > 0 && await enumerator.MoveNextAsync());

            yield return list.ToArray();
            list.Clear();
        }
    }

    /// <summary>
    /// Projects each element of an async-enumerable sequence to an async-enumerable sequence and merges the resulting async-enumerable sequences into one async-enumerable sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements in the projected inner sequences and the elements in the merged result sequence.</typeparam>
    /// <param name="source">An async-enumerable sequence of elements to project.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>An async-enumerable sequence whose elements are the result of invoking the one-to-many transform function on each element of the input sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    public static async IAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
    {
        await foreach (var v in source)
        {
            foreach (var result in selector(v))
            {
                yield return result;
            }
        }
    }
}

