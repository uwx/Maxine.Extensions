using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Maxine.Extensions;

public class AsciiComparer : IEqualityComparer<byte>
{
    public static AsciiComparer Instance { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAsciiCodePoint(uint value) => value <= 0x7Fu;
    
    public bool Equals(byte x, byte y)
    {
        var valueA = (uint)x;
        var valueB = (uint)y;

        if (!IsAsciiCodePoint(valueA | valueB))
        {
            return false;
        }

        if (valueA == valueB)
        {
            return true;
        }

        valueA |= 0x20u;
        if (valueA - 'a' > 'z' - 'a')
        {
            return false; // not exact match, and first input isn't in [A-Za-z]
        }

        return valueA == (valueB | 0x20u);
    }

    public int GetHashCode(byte obj)
    {
        var value = (uint)obj;

        if (!IsAsciiCodePoint(obj))
        {
            return obj;
        }

        value |= 0x20u;

        if (value - 'a' > 'z' - 'a')
        {
            return obj;
        }

        return unchecked((int)value);
    }
}

public static class SpanComparerExtensions
{
    public static int IndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> search, IEqualityComparer<T> comparer)
    {
        return IndexOf(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(search), search.Length, comparer);
    }
    public static int IndexOf<T>(this Span<T> span, ReadOnlySpan<T> search, IEqualityComparer<T> comparer)
    {
        return IndexOf(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(search), search.Length, comparer);
    }
    
    // from SpanHelpers
    private static int IndexOf<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength, IEqualityComparer<T> comparer)
    {
        Debug.Assert(searchSpaceLength >= 0);
        Debug.Assert(valueLength >= 0);

        if (valueLength == 0)
            return 0;  // A zero-length sequence is always treated as "found" at the start of the search space.

        T valueHead = value;
        ref T valueTail = ref Unsafe.Add(ref value, 1);
        int valueTailLength = valueLength - 1;

        int index = 0;
        while (true)
        {
            Debug.Assert(0 <= index && index <= searchSpaceLength); // Ensures no deceptive underflows in the computation of "remainingSearchSpaceLength".
            int remainingSearchSpaceLength = searchSpaceLength - index - valueTailLength;
            if (remainingSearchSpaceLength <= 0)
                break;  // The unsearched portion is now shorter than the sequence we're looking for. So it can't be there.

            // Do a quick search for the first element of "value".
            int relativeIndex = IndexOf(ref Unsafe.Add(ref searchSpace, index), valueHead, remainingSearchSpaceLength, comparer);
            if (relativeIndex < 0)
                break;
            index += relativeIndex;

            // Found the first element of "value". See if the tail matches.
            if (SequenceEqual(ref Unsafe.Add(ref searchSpace, index + 1), ref valueTail, valueTailLength, comparer))
                return index;  // The tail matched. Return a successful find.

            index++;
        }
        return -1;
    }

    private static unsafe int IndexOf<T>(ref T searchSpace, T value, int length, IEqualityComparer<T> comparer)
    {
        Debug.Assert(length >= 0);

        nint index = 0; // Use nint for arithmetic to avoid unnecessary 64->32->64 truncations
        if (default(T) != null || (object?)value != null)
        {
            Debug.Assert(value is not null);

            while (length >= 8)
            {
                length -= 8;

                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index)))
                    goto Found;
                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index + 1)))
                    goto Found1;
                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index + 2)))
                    goto Found2;
                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index + 3)))
                    goto Found3;
                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index + 4)))
                    goto Found4;
                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index + 5)))
                    goto Found5;
                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index + 6)))
                    goto Found6;
                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index + 7)))
                    goto Found7;

                index += 8;
            }

            if (length >= 4)
            {
                length -= 4;

                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index)))
                    goto Found;
                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index + 1)))
                    goto Found1;
                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index + 2)))
                    goto Found2;
                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index + 3)))
                    goto Found3;

                index += 4;
            }

            while (length > 0)
            {
                if (comparer.Equals(value, Unsafe.Add(ref searchSpace, index)))
                    goto Found;

                index += 1;
                length--;
            }
        }
        else
        {
            nint len = (nint)length;
            for (index = 0; index < len; index++)
            {
                if ((object?)Unsafe.Add(ref searchSpace, index) is null)
                {
                    goto Found;
                }
            }
        }
        return -1;

    Found: // Workaround for https://github.com/dotnet/runtime/issues/8795
        return (int)index;
    Found1:
        return (int)(index + 1);
    Found2:
        return (int)(index + 2);
    Found3:
        return (int)(index + 3);
    Found4:
        return (int)(index + 4);
    Found5:
        return (int)(index + 5);
    Found6:
        return (int)(index + 6);
    Found7:
        return (int)(index + 7);
    }

    public static bool SequenceEqual<T>(ref T first, ref T second, int length, IEqualityComparer<T> comparer)
    {
        Debug.Assert(length >= 0);

        if (Unsafe.AreSame(ref first, ref second))
            goto Equal;

        nint index = 0; // Use nint for arithmetic to avoid unnecessary 64->32->64 truncations
        T lookUp0;
        T lookUp1;
        while (length >= 8)
        {
            length -= 8;

            lookUp0 = Unsafe.Add(ref first, index);
            lookUp1 = Unsafe.Add(ref second, index);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;
            lookUp0 = Unsafe.Add(ref first, index + 1);
            lookUp1 = Unsafe.Add(ref second, index + 1);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;
            lookUp0 = Unsafe.Add(ref first, index + 2);
            lookUp1 = Unsafe.Add(ref second, index + 2);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;
            lookUp0 = Unsafe.Add(ref first, index + 3);
            lookUp1 = Unsafe.Add(ref second, index + 3);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;
            lookUp0 = Unsafe.Add(ref first, index + 4);
            lookUp1 = Unsafe.Add(ref second, index + 4);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;
            lookUp0 = Unsafe.Add(ref first, index + 5);
            lookUp1 = Unsafe.Add(ref second, index + 5);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;
            lookUp0 = Unsafe.Add(ref first, index + 6);
            lookUp1 = Unsafe.Add(ref second, index + 6);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;
            lookUp0 = Unsafe.Add(ref first, index + 7);
            lookUp1 = Unsafe.Add(ref second, index + 7);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;

            index += 8;
        }

        if (length >= 4)
        {
            length -= 4;

            lookUp0 = Unsafe.Add(ref first, index);
            lookUp1 = Unsafe.Add(ref second, index);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;
            lookUp0 = Unsafe.Add(ref first, index + 1);
            lookUp1 = Unsafe.Add(ref second, index + 1);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;
            lookUp0 = Unsafe.Add(ref first, index + 2);
            lookUp1 = Unsafe.Add(ref second, index + 2);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;
            lookUp0 = Unsafe.Add(ref first, index + 3);
            lookUp1 = Unsafe.Add(ref second, index + 3);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;

            index += 4;
        }

        while (length > 0)
        {
            lookUp0 = Unsafe.Add(ref first, index);
            lookUp1 = Unsafe.Add(ref second, index);
            if (!(comparer.Equals(lookUp0, lookUp1)))
                goto NotEqual;
            index += 1;
            length--;
        }

    Equal:
        return true;

    NotEqual: // Workaround for https://github.com/dotnet/runtime/issues/8795
        return false;
    }
}