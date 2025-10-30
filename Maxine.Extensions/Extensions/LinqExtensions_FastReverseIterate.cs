using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions;

public static class LinqExtensions_FastReverseIterate
{
    private static class StackAccessor<T>
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = nameof(_array))]
        public static extern T[] _array(Stack<T> stack);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = nameof(_size))]
        public static extern int _size(Stack<T> stack);
    }

    private static class ListAccessor<T>
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = nameof(_items))]
        public static extern T[] _items(List<T> list);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = nameof(_size))]
        public static extern int _size(List<T> list);
    }

    private static Span<T> AsSpan<T>(Stack<T>? stack)
        => stack is null ? default : new Span<T>(StackAccessor<T>._array(stack), 0, StackAccessor<T>._size(stack));

    private static Memory<T> AsMemory<T>(Stack<T>? stack)
        => stack is null ? default : new Memory<T>(StackAccessor<T>._array(stack), 0, StackAccessor<T>._size(stack));

    private static Memory<T> AsMemory<T>(List<T>? list)
        => list is null ? default : new Memory<T>(ListAccessor<T>._items(list), 0, ListAccessor<T>._size(list));

    public static ReverseIterator<T> FastReverseIterate<T>(this Stack<T> stack)
    {
        var span = AsSpan(stack);
        return new ReverseIterator<T>(span);
    }
    
    public static ReverseIterator<T> FastReverseIterate<T>(this List<T> list)
    {
        var span = CollectionsMarshal.AsSpan(list);
        return new ReverseIterator<T>(span);
    }
    
    public static ListReverseIterator<T> FastReverseIterate<T>(this IList<T> list)
    {
        return new ListReverseIterator<T>(list);
    }
    
    public static ReadOnlyListReverseIterator<T> FastReverseIterate<T>(this IReadOnlyList<T> list)
    {
        return new ReadOnlyListReverseIterator<T>(list);
    }

    public static IEnumerable<T> FastReverseIterate<T>(this IEnumerable<T> enumerable)
    {
        switch (enumerable)
        {
            case Stack<T> stack:
            {
                return Iter(stack);

                static IEnumerable<T> Iter(Stack<T> stack)
                {
                    var items = StackAccessor<T>._array(stack);
                    var len = StackAccessor<T>._size(stack);
                    for (var i = len - 1; i >= 0; i--)
                    {
                        yield return items[i];
                    }
                }
            }
            case IList<T> ilist:
            {
                return FastReverseIterate(ilist);
            }
            case IReadOnlyList<T> ireadonlylist:
            {
                return FastReverseIterate(ireadonlylist);
            }
            default:
                return enumerable.Reverse();
        }
    }

    /// <summary>
    /// An iterator that yields the items of an <see cref="IEnumerable{TSource}"/> in reverse.
    /// </summary>
    /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
    public ref struct ReverseIterator<TSource>(Span<TSource> source)
    {
        private readonly Span<TSource> _source = source;
        private int _state = 1;
        private TSource? _current;

        public readonly ReverseIterator<TSource> GetEnumerator() => this;

        public readonly TSource Current => _state > 1 ? _current! : throw new InvalidOperationException();

        public bool MoveNext()
        {
            if (_state - 2 <= -2)
            {
                // Either someone called a method and cast us to IEnumerable without calling GetEnumerator,
                // or we were already disposed. In either case, iteration has ended, so return false.
                // A comparison is made against -2 instead of _state <= 0 because we want to handle cases where
                // the source is really large and adding the bias causes _state to overflow.
                Debug.Assert(_state is -1 or 0);
                return false;
            }

            switch (_state)
            {
                case 1:
                    // Iteration has just started. Capture the source into an array and set _state to 2 + the count.
                    // Having an extra field for the count would be more readable, but we save it into _state with a
                    // bias instead to minimize field size of the iterator.
                    _state = _source.Length + 2;
                    goto default;
                default:
                    // At this stage, _state starts from 2 + the count. _state - 3 represents the current index into the
                    // buffer. It is continuously decremented until it hits 2, which means that we've run out of items to
                    // yield and should return false.
                    var index = _state - 3;
                    if (index != -1)
                    {
                        _current = _source[index];
                        --_state;
                        return true;
                    }

                    break;
            }
            return false;
        }
    }
    
    /// <summary>
    /// An iterator that yields the items of an <see cref="IEnumerable{TSource}"/> in reverse.
    /// </summary>
    /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
    public struct ListReverseIterator<TSource>(IList<TSource> source) : IEnumerator<TSource>, IEnumerable<TSource>
    {
        private int _state = 1;
        private TSource? _current;

        public readonly ListReverseIterator<TSource> GetEnumerator() => this;
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => this;
        readonly IEnumerator IEnumerable.GetEnumerator() => this;

        object IEnumerator.Current => Current!;

        public readonly TSource Current => _state > 1 ? _current! : throw new InvalidOperationException();

        public bool MoveNext()
        {
            if (_state - 2 <= -2)
            {
                // Either someone called a method and cast us to IEnumerable without calling GetEnumerator,
                // or we were already disposed. In either case, iteration has ended, so return false.
                // A comparison is made against -2 instead of _state <= 0 because we want to handle cases where
                // the source is really large and adding the bias causes _state to overflow.
                Debug.Assert(_state is -1 or 0);
                return false;
            }

            switch (_state)
            {
                case 1:
                    // Iteration has just started. Capture the source into an array and set _state to 2 + the count.
                    // Having an extra field for the count would be more readable, but we save it into _state with a
                    // bias instead to minimize field size of the iterator.
                    _state = source.Count + 2;
                    goto default;
                default:
                    // At this stage, _state starts from 2 + the count. _state - 3 represents the current index into the
                    // buffer. It is continuously decremented until it hits 2, which means that we've run out of items to
                    // yield and should return false.
                    var index = _state - 3;
                    if (index != -1)
                    {
                        _current = source[index];
                        --_state;
                        return true;
                    }

                    break;
            }
            return false;
        }

        public void Reset()
        {
            _state = 1;
        }

        public readonly void Dispose()
        {
        }
    }
    
    /// <summary>
    /// An iterator that yields the items of an <see cref="IEnumerable{TSource}"/> in reverse.
    /// </summary>
    /// <typeparam name="TSource">The type of the source enumerable.</typeparam>
    public struct ReadOnlyListReverseIterator<TSource>(IReadOnlyList<TSource> source) : IEnumerator<TSource>, IEnumerable<TSource>
    {
        private int _state = 1;
        private TSource? _current;

        public readonly ReadOnlyListReverseIterator<TSource> GetEnumerator() => this;
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => this;
        readonly IEnumerator IEnumerable.GetEnumerator() => this;

        object IEnumerator.Current => Current!;

        public readonly TSource Current => _state > 1 ? _current! : throw new InvalidOperationException();

        public bool MoveNext()
        {
            if (_state - 2 <= -2)
            {
                // Either someone called a method and cast us to IEnumerable without calling GetEnumerator,
                // or we were already disposed. In either case, iteration has ended, so return false.
                // A comparison is made against -2 instead of _state <= 0 because we want to handle cases where
                // the source is really large and adding the bias causes _state to overflow.
                Debug.Assert(_state is -1 or 0);
                return false;
            }

            switch (_state)
            {
                case 1:
                    // Iteration has just started. Capture the source into an array and set _state to 2 + the count.
                    // Having an extra field for the count would be more readable, but we save it into _state with a
                    // bias instead to minimize field size of the iterator.
                    _state = source.Count + 2;
                    goto default;
                default:
                    // At this stage, _state starts from 2 + the count. _state - 3 represents the current index into the
                    // buffer. It is continuously decremented until it hits 2, which means that we've run out of items to
                    // yield and should return false.
                    var index = _state - 3;
                    if (index != -1)
                    {
                        _current = source[index];
                        --_state;
                        return true;
                    }

                    break;
            }
            return false;
        }

        public void Reset()
        {
            _state = 1;
        }

        public readonly void Dispose()
        {
        }
    }
}