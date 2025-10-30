using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RayTech.RayLog.MEL;

public static class RayLogUtils
{
    public static readonly string[] LevelNames =
    [
        "Debug",
        "Info",
        "Warning",
        "Severe",
        "Fatal"
    ];

    internal static readonly long StartTime;

    static RayLogUtils()
    {
        StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        EnableEscapes.EnableEscapesOnWindows();
    }

    internal static string FormatTime(long timeStamp)
    {
        var span = new TimeSpan((timeStamp - StartTime) * TimeSpan.TicksPerMillisecond);

        var ms = span.Milliseconds;
        var secs = span.Seconds;
        var mins = span.Minutes;
        var hours = span.Hours;
        var days = span.Days;
            
        if (days > 0) return FormatDays(days, hours, mins, secs);
            
        if (hours > 0) return FormatHours(hours, mins, secs);
            
        if (mins > 0) return FormatMins(mins, secs);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (secs > 0) return FormatSecs(secs, ms);
            
        // else
        return FormatSecs(0, ms);
    }

    private static string FormatSecs(int secs, int ms)
    {
        Span<char> chars = stackalloc char[
            1 + // +
            2 + // Pad0(secs)
            1 + // .
            1 + // Digit(ms / 100)
            1 //   s
        ];
            
        chars[0] = '+';
        Pad0(secs, ref chars, 1);
        chars[3] = '.';
        chars[4] = Digit(ms / 100);
        chars[5] = 's';

        return new string(chars);
    }

    private static string FormatMins(int mins, int secs)
    {
        Span<char> chars = stackalloc char[
            1 + // +
            2 + // Pad0(mins)
            1 + // :
            2 //   Pad0(secs)
        ];
            
        chars[0] = '+';
        Pad0(mins, ref chars, 1);
        chars[3] = ':';
        Pad0(secs, ref chars, 4);

        return new string(chars);
    }

    private static string FormatHours(int hours, int mins, int secs)
    {
        Span<char> chars = stackalloc char[
            1 + // +
            2 + // Pad0(hours)
            1 + // :
            2 + // Pad0(mins)
            1 + // :
            2 //   Pad0(secs)
        ];
                
        chars[0] = '+';
        Pad0(hours, ref chars, 1);
        chars[3] = ':';
        Pad0(mins, ref chars, 4);
        chars[6] = ':';
        Pad0(secs, ref chars, 7);

        return new string(chars);
    }

    private static string FormatDays(int days, int hours, int mins, int secs)
    {
        var dayString = days.ToString();
        var dayLen = dayString.Length;

        Span<char> chars = stackalloc char[
            1 + // +
            dayLen + // dayString
            1 + // :
            2 + // Pad0(hours)
            1 + // :
            2 + // Pad0(mins)
            1 + // :
            2 //   Pad0(secs)
        ];

        chars[0] = '+';
            
        dayString.AsSpan().CopyTo(chars[1..dayLen]);
        chars[1 + dayLen] = ':';
        Pad0(hours, ref chars, 2 + dayLen);
        chars[4 + dayLen] = ':';
        Pad0(mins, ref chars, 5 + dayLen);
        chars[7 + dayLen] = ':';
        Pad0(secs, ref chars, 8 + dayLen);

        return new string(chars);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Pad0(int num, ref Span<char> span, int start)
    {
        if (num < 10)
        {
            span[start] = '0';
            span[start + 1] = (char) (num + 48);
        }
        else
        {
            span[start] = (char) (num / 10 + 48);
            span[start + 1] = (char) (num % 10 + 48);
        }
    }
        
    // unused:
    //private static void Pad0n3(int i, out char i1, out char i2, out char i3)
    //{
    //    if (i < 10)
    //    {
    //        i1 = '0';
    //        i2 = '0';
    //        i3 = (char) (i + 48);
    //    }
    //    else if (i < 100)
    //    {
    //        i1 = '0';
    //        i2 = (char) (i / 10 + 48);
    //        i3 = (char) (i % 10 + 48);
    //    }
    //    else
    //    {
    //        i1 = (char) (i / 100 + 48);
    //        i2 = (char) (i / 10 % 10 + 48);
    //        i3 = (char) (i % 10 + 48);
    //    }
    //}

    // for 0-9
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char Digit(int i) => (char) (i + 48);

    private static long Now
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FormatNow() => FormatTime(Now);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FormatTime(DateTimeOffset time) => FormatTime(time.ToUnixTimeMilliseconds());

    /// <summary>
    /// Partition a string into a group of substrings
    /// </summary>
    /// <param name="input"></param>
    /// <param name="totalPartitions"></param>
    /// <returns></returns>
    public static IEnumerable<string> Partition(string input, int totalPartitions)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        if (totalPartitions < 1)
            throw new ArgumentOutOfRangeException(nameof(totalPartitions));

        var maxSize = (int)Math.Ceiling(input.Length / (double)totalPartitions);
        var k = 0;

        for (var i = 0; i < totalPartitions; i++)
        {
            yield return input.Substring(k, Math.Min(input.Length - k, maxSize));
            k += maxSize;
            if (k >= input.Length) yield break;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Initialize()
    {
        // empty
    }
}