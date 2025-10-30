using System.Diagnostics;

namespace Maxine.Extensions;

public readonly struct BenchmarkTimer(string message = "") : IDisposable
{
    private readonly long _stopwatch = Stopwatch.GetTimestamp();

    public void Dispose()
    {
        var ticksDifference = Stopwatch.GetTimestamp() - _stopwatch;
        var elapsed = TimeSpan.FromTicks(ticksDifference);
        
        Console.WriteLine(message + " elapsed: " + elapsed);
    }
}