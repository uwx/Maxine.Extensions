using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions;

public static class StackExtensions
{
    extension<T>(Stack<T> stack)
    {
        public ref T GetElementRef(int index)
        {
            if (index < 0 || index >= stack.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return ref CollectionsMarshal.AsSpan(stack)[index];
        }

        public ref T PeekRef()
        {
            if (stack.Count == 0)
                throw new InvalidOperationException("Stack is empty.");
            return ref stack.GetElementRef(stack.Count - 1);
        }
    }

    extension(CollectionsMarshal)
    {
        public static Span<T> AsSpan<T>(Stack<T> stack)
        {
            return StackAccessors<T>.GetArray(stack).AsSpan(0, StackAccessors<T>.GetCount(stack));
        }

        public static void SetCount<T>(Stack<T> stack, int count)
        {
            if (count < 0 || count > stack.Count)
                ThrowArgumentOutOfRangeException();
            StackAccessors<T>.GetCount(stack) = count;
            return;

            [DoesNotReturn]
            static void ThrowArgumentOutOfRangeException()
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
        }
    }

    private static class StackAccessors<T>
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_array")]
        public static extern ref T[] GetArray(Stack<T> c);
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_size")]
        public static extern ref int GetCount(Stack<T> c);
    }
}