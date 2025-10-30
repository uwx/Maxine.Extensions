namespace WadArchiveJsonRenderer;

// https://stackoverflow.com/a/12389412
public static class Ex
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
}