using System.Numerics;
using System.Runtime.CompilerServices;

namespace Maxine.Extensions;

internal static class Math2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Mod(double x, double y)
    {
        // C# does not have true modulo operator, only remainder, so we have to take care of negative numbers
        x %= y; 
        return x < 0 ? x + y : x;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Mod<T>(T x, T y) where T : INumber<T>
    {
        // C# does not have true modulo operator, only remainder, so we have to take care of negative numbers
        x %= y; 
        return x < T.Zero ? x + y : x;
    }
}