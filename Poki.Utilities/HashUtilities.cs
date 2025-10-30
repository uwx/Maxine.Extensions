using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Poki.Utilities;

/// <summary>
/// Utility classes for converting between an arbitrary numeral system.
/// </summary>
public static class HashUtilities
{
    private const uint KeyMask = 114926711;

    private static readonly Skip32Cipher Cipher;

    static HashUtilities()
    {
        var bytes = new byte[10];
        new Random((int) KeyMask).NextBytes(bytes);
        Cipher = new Skip32Cipher(bytes);
    }

    public static string GetDisplayString(int index)
    {
        return ArbitraryBase.B31.ToBase(Cipher.Encrypt((uint) index), 4);
    }

    public static int GetRealIndex(string displayIndex)
    {
        // ReSharper disable once ArrangeLocalFunctionBody
        [MethodImpl(MethodImplOptions.NoInlining)]
        static int ThrowInvalidCharacter()
            => throw new ArgumentException("Invalid character in the arbitrary numeral system number");

        return TryGetRealIndex(displayIndex, out var index) ? index : ThrowInvalidCharacter();
    }

    public static bool TryGetRealIndex(string displayIndex, out int value)
    {
        if (ArbitraryBase.B31.TryFromBase(displayIndex, out var stoi))
        {
            value = (int) Cipher.Decrypt(stoi);
            return true;
        }

        value = 0;
        return false;
    }
}