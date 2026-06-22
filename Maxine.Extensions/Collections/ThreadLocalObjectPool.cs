namespace Maxine.Extensions.Collections;

public sealed class ThreadLocalObjectPool<T>(Func<T> factory, int maxEntries = int.MaxValue) : IDisposable where T : class
{
    private readonly ThreadLocal<Stack<T>> _pool = new(static () => new Stack<T>());
    private T? _fastItem;

    public T Get()
    {
        var item = _fastItem;
        if (item is null || Interlocked.CompareExchange(ref _fastItem, null, item) != item)
        {
            var stack = _pool.Value!;
            if (stack.Count > 0)
                return stack.Pop();
            return factory();
        }
        return item;
    }

    public void Return(T item)
    {
        if (_fastItem is not null || Interlocked.CompareExchange(ref _fastItem, item, null) != null)
        {
            var stack = _pool.Value!;

            if (stack.Count < maxEntries)
            {
                stack.Push(item);
            }
            else
            {
                DisposeItem(item);
            }
        }
    }

    private static void DisposeItem(T item)
    {
        if (item is IDisposable disposable)
            disposable.Dispose();
    }

    public void Dispose()
    {
        foreach (var value in _pool.Values)
        {
            T[] arr;

            try
            {
                arr = value.ToArray();
            }
            catch
            {
                continue;
            }
            
            foreach (var t in arr)
            {
                DisposeItem(t);
            }
        }

        _pool.Dispose();
    }
}