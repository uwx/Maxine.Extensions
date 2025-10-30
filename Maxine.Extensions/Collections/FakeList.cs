using System.Collections;

namespace Maxine.Extensions.Collections;

public abstract class FakeList<T> : IList<T>
{
    public abstract IEnumerator<T> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public abstract int Count { get; }
    public virtual bool IsReadOnly => false;

    public abstract void Add(T item);

    public abstract void Clear();

    public abstract bool Contains(T item);

    public virtual void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
        if (array.Length - arrayIndex < Count)
            throw new ArgumentException($"The number of elements in the source {nameof(ICollection<T>)} is greater than the available space from {nameof(arrayIndex)} to the end of the destination array.");

        foreach (var t in this)
        {
            array[arrayIndex++] = t;
        }
    }

    public virtual bool Remove(T item)
    {
        var index = -1;
        var i = 0;
        foreach (var t in this)
        {
            if (EqualityComparer<T>.Default.Equals(t, item))
            {
                index = i;
                break;
            }

            i++;
        }

        if (index > -1)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public abstract int IndexOf(T item);

    public abstract void Insert(int index, T item);

    public abstract void RemoveAt(int index);

    public abstract T this[int index] { get; set; }
}

public abstract class FakeAppendOnlyList<T> : FakeList<T>
{
    private sealed class ConversionWrapList<TSource>(IList<TSource> sourceList, Func<T, TSource> conversionFunc) : FakeAppendOnlyList<T>
    {
        public override int Count => sourceList.Count;
        public override void Add(T item)
        {
            sourceList.Add(conversionFunc(item));
        }
    }
    
    public static IList<T> WrapWithConversionTo<TSource>(IList<TSource> sourceList, Func<T, TSource> conversionFunc)
        => new ConversionWrapList<TSource>(sourceList, conversionFunc);
    
    public override IEnumerator<T> GetEnumerator()
    {
        throw new NotSupportedException("Collection is append-only.");
    }

    public override bool Contains(T item)
    {
        throw new NotSupportedException("Collection is append-only.");
    }

    public override void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotSupportedException("Collection is append-only.");
    }

    public override bool Remove(T item)
    {
        throw new NotSupportedException("Collection is append-only.");
    }

    public override int IndexOf(T item)
    {
        throw new NotSupportedException("Collection is append-only.");
    }

    public override T this[int index]
    {
        get => throw new NotSupportedException("Collection is append-only.");
        set => throw new NotSupportedException("Collection is append-only.");
    }

    public override void Clear()
    {
        throw new NotSupportedException("Collection is append-only.");
    }

    public override void Insert(int index, T item)
    {
        throw new NotSupportedException("Collection is append-only.");
    }

    public override void RemoveAt(int index)
    {
        throw new NotSupportedException("Collection is append-only.");
    }
}