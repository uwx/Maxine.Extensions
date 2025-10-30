using System.Numerics;
using System.Runtime.CompilerServices;
using MI = System.Runtime.CompilerServices.MethodImplAttribute;

namespace Maxine.Extensions;

public static class MathExtensions
{
    private const MethodImplOptions I = MethodImplOptions.AggressiveInlining;

    [MI(I)] public static Angle<T> Degrees<T>(this T deg) where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> => Angle<T>.FromDegrees(deg);
    
    [MI(I)] public static Angle<T> Radians<T>(this T rad) where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> => Angle<T>.FromRadians(rad);
    
    [MI(I)] public static Length<T> Meters<T>(this T meters) where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> => Length<T>.FromMeters(meters);
    
    [MI(I)] public static Length<T> Km<T>(this T km) where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> => Length<T>.FromKilometers(km);
    
    [MI(I)] public static Length<T> Cm<T>(this T cm) where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> => Length<T>.FromCentimeters(cm);
    
    [MI(I)] public static Speed<T> MetersPerSecond<T>(this T mps) where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> => Speed<T>.FromMetersPerSecond(mps);
    
    [MI(I)] public static Speed<T> Kph<T>(this T kph) where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T> => Speed<T>.FromKilometersPerHour(kph);
    
    [MI(I)] public static TimeSpan Seconds(this int sec) => TimeSpan.FromSeconds(sec);
    [MI(I)] public static TimeSpan Seconds(this double sec) => TimeSpan.FromSeconds(sec);
    
    [MI(I)] public static TimeSpan Milliseconds(this int sec) => TimeSpan.FromMilliseconds(sec);
    [MI(I)] public static TimeSpan Milliseconds(this double sec) => TimeSpan.FromMilliseconds(sec);
}