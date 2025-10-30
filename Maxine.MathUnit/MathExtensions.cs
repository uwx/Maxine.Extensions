using System;
using System.Runtime.CompilerServices;
using System.Windows;
using MI = System.Runtime.CompilerServices.MethodImplAttribute;

namespace MathUnit;

public static class MathExtensions
{
    private const MethodImplOptions I = MethodImplOptions.AggressiveInlining;

    [MI(I)] public static Angle Degrees(this int deg) => Angle.FromDegrees(deg);
    [MI(I)] public static Angle Degrees(this double deg) => Angle.FromDegrees(deg);
    
    [MI(I)] public static Angle Radians(this int rad) => Angle.FromRadians(rad);
    [MI(I)] public static Angle Radians(this double rad) => Angle.FromRadians(rad);
    
    [MI(I)] public static Length Meters(this int meters) => Length.FromMeters(meters);
    [MI(I)] public static Length Meters(this double meters) => Length.FromMeters(meters);
    
    [MI(I)] public static Length Km(this int km) => Length.FromKilometers(km);
    [MI(I)] public static Length Km(this double km) => Length.FromKilometers(km);
    
    [MI(I)] public static Length Cm(this int cm) => Length.FromCentimeters(cm);
    [MI(I)] public static Length Cm(this double cm) => Length.FromCentimeters(cm);
    
    [MI(I)] public static Speed MetersPerSecond(this int mps) => Speed.FromMetersPerSecond(mps);
    [MI(I)] public static Speed MetersPerSecond(this double mps) => Speed.FromMetersPerSecond(mps);
    
    [MI(I)] public static Speed Kph(this int kph) => Speed.FromKilometersPerHour(kph);
    [MI(I)] public static Speed Kph(this double kph) => Speed.FromKilometersPerHour(kph);
    
    [MI(I)] public static TimeSpan Seconds(this int sec) => TimeSpan.FromSeconds(sec);
    [MI(I)] public static TimeSpan Seconds(this double sec) => TimeSpan.FromSeconds(sec);
    
    [MI(I)] public static TimeSpan Milliseconds(this int sec) => TimeSpan.FromMilliseconds(sec);
    [MI(I)] public static TimeSpan Milliseconds(this double sec) => TimeSpan.FromMilliseconds(sec);
}