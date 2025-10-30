using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace RayTech.RayLog.MEL;

internal static class EnableEscapes
{
    // https://www.jerriepelser.com/blog/using-ansi-color-codes-in-net-console-apps/
    private const int STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetStdHandle(int nStdHandle);

    internal static void EnableEscapesOnWindows()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }
        
        var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
        if (!GetConsoleMode(iStdOut, out var outConsoleMode))
        {
            Log.Warning($"failed to get output console mode, error: 0x{Marshal.GetLastPInvokeError():X4} {Marshal.GetLastPInvokeErrorMessage()}");
            return;
        }

        outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        if (!SetConsoleMode(iStdOut, outConsoleMode))
        {
            Log.Warning($"failed to set output console mode, error: 0x{Marshal.GetLastPInvokeError():X4} {Marshal.GetLastPInvokeErrorMessage()}");
            return;
        }
    }

}