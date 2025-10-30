using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Collections.Extensions;

namespace Poki.Utilities;

public class ArbitraryBaseConverter<T, TUnsigned>
    where T : unmanaged, IBinaryInteger<T>
    where TUnsigned : unmanaged, IBinaryInteger<TUnsigned>, IUnsignedNumber<TUnsigned>
{
    private readonly char[] _baseDigits;
    private readonly DictionarySlim<char, ushort> _baseDigitsReverse;

    public ArbitraryBaseConverter(string baseDigits, bool caseInsensitive = false)
    {
        _baseDigits = baseDigits.ToCharArray();
        _baseDigitsReverse = MakeInverseDictionary(baseDigits, caseInsensitive);
    }

    public ArbitraryBaseConverter(ReadOnlySpan<char> baseDigits, bool caseInsensitive = false)
    {
        _baseDigits = baseDigits.ToArray();
        _baseDigitsReverse = MakeInverseDictionary(baseDigits, caseInsensitive);
    }

    private static DictionarySlim<char, ushort> MakeInverseDictionary(ReadOnlySpan<char> digits, bool caseInsensitive = false)
    {
        var dict = new DictionarySlim<char, ushort>();
        
        if (caseInsensitive)
        {
            for (ushort i = 0; i < digits.Length; i++)
            {
                dict.GetOrAddValueRef(char.ToUpperInvariant(digits[i])) = i;
                dict.GetOrAddValueRef(char.ToLowerInvariant(digits[i])) = i;
            }
        }
        else
        {
            for (ushort i = 0; i < digits.Length; i++)
            {
                dict.GetOrAddValueRef(digits[i]) = i;
            }
        }

        return dict;
    }
    
    static ArbitraryBaseConverter()
    {
        if (typeof(T) != typeof(int) && typeof(T) != typeof(uint))
        {
            throw new InvalidOperationException($"{nameof(T)} must be int or uint");
        }
    }

    // Replace with Math.DivRem if it ever gets a generic
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static (TUnsigned Quotient, TUnsigned Remainder) DivRem(TUnsigned left, TUnsigned right)
    {
        var quotient = left / right;
        return (quotient, left - (quotient * right));
    }
    
    // Replace with: int.CreateTruncating(remainder);
    // once rider stops whining about it
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int UnsignedToInt(TUnsigned value)
    {
        if (typeof(TUnsigned) == typeof(byte)) return (byte)(object)value;
        if (typeof(TUnsigned) == typeof(ushort)) return (ushort)(object)value;
        if (typeof(TUnsigned) == typeof(uint)) return (int)(uint)(object)value;
        if (typeof(TUnsigned) == typeof(ulong)) return (int)(ulong)(object)value;
        if (typeof(TUnsigned) == typeof(UInt128)) return (int)(UInt128)(object)value;
        if (typeof(TUnsigned) == typeof(nuint)) return (int)(nuint)(object)value;

        throw new InvalidOperationException($"Unsupported type {typeof(TUnsigned)}");
    }

    // https://stackoverflow.com/a/10981113
    public string ToBase(T input1, int padLength = 0)
    {
        var digits = _baseDigits;
        
        if (input1 == T.Zero)
        {
            return new string(digits[0], 1);
        }

        var input = TUnsigned.CreateTruncating(input1);
        
        // List of characters allowed in the target string 
        var radix = TUnsigned.CreateTruncating(digits.Length);

        const int maxLength = sizeof(uint)*8;
        
        Span<char> charArray = stackalloc char[maxLength];
        var index = maxLength - 1;

        do
        {
            (input, var remainder) = DivRem(input, radix);

            charArray[index--] = digits[UnsignedToInt(remainder)];
        }
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse - wrong lol
        while (input > TUnsigned.Zero);

        var result = charArray[(index + 1)..maxLength];

        if (padLength - result.Length > 0)
        {
            Span<char> padding = stackalloc char[padLength - result.Length];
            padding.Fill(digits[0]);
            return string.Concat(padding, result);
        }

        return new string(result);
    }

    public bool TryFromBase(string number, out T result1)
    {
        var digits = _baseDigits;
        var digitsToValue = _baseDigitsReverse;

        result1 = T.Zero;

        if (string.IsNullOrEmpty(number))
            return false;

        var radix = TUnsigned.CreateTruncating(digits.Length);

        var result = TUnsigned.Zero;
        var multiplier = TUnsigned.One;
        for (var i = number.Length - 1; i >= 0; i--)
        {
            if (!digitsToValue.TryGetValue(number[i], out var digit))
                return false;

            result += TUnsigned.CreateTruncating(digit) * multiplier; // TODO if T/TUnsigned are sized < sizeof(ushort) can digit be truncated too small?
            multiplier *= radix;
        }

        result1 = T.CreateTruncating(result);
        return true;
    }

    public T FromBase(string number)
    {
        return TryFromBase(number, out var result)
            ? result
            : throw new ArgumentException($"Input string {number} was invalid", nameof(number));
    }
}

public class ArbitraryBaseConverter<T> : ArbitraryBaseConverter<T, T> where T : unmanaged, IBinaryInteger<T>, IUnsignedNumber<T>
{
    public ArbitraryBaseConverter(string baseDigits, bool caseInsensitive = false) : base(baseDigits, caseInsensitive)
    {
    }

    public ArbitraryBaseConverter(ReadOnlySpan<char> baseDigits, bool caseInsensitive = false) : base(baseDigits, caseInsensitive)
    {
    }
}

public static class ArbitraryBase
{
    public static readonly ArbitraryBaseConverter<int, uint> B93 = new("""
    0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!'"#$%&()*+-.\/:;<=>?@[]{}^_`|~
    """);

    public static readonly ArbitraryBaseConverter<uint> B31 = new("23456789abcdefghjkmnpqrstuvwxyz");
}