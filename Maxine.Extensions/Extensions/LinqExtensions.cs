namespace Maxine.Extensions;

public static class LinqExtensions
{
    // https://stackoverflow.com/a/12389412
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
    
    public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TSource, TKey, TValue>(
        this IEnumerable<TSource> enumerable,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> elementSelector
    ) where TKey : notnull
    {
        var dict = new OrderedDictionary<TKey, TValue>();

        foreach (var source in enumerable)
        {
            dict[keySelector(source)] = elementSelector(source);
        }

        return dict;
    }

}