namespace Maxine.Extensions.Shared;

public readonly struct TimerEx : IDisposable, IAsyncDisposable
{
    private readonly long _dueTime;
    private readonly long _period;
    private readonly Timer _timer;
    
    private TimerEx(Action callback, long dueTime, long period)
    {
        _dueTime = dueTime;
        _period = period;

        _timer = new Timer(
            static callback => ((Action)callback!)(),
            callback,
            Timeout.Infinite,
            Timeout.Infinite
        );
    }

    public static TimerEx Once(Action callback, TimeSpan dueTime)
    {
        return new TimerEx(
            callback,
            (long)Math.Ceiling(dueTime.TotalMilliseconds),
            Timeout.Infinite
        );
    }

    public static TimerEx Repeat(Action callback, TimeSpan dueTime, TimeSpan interval)
    {
        return new TimerEx(
            callback,
            (long)Math.Ceiling(dueTime.TotalMilliseconds),
            (long)Math.Ceiling(interval.TotalMilliseconds)
        );
    }

    public static TimerEx Repeat(Action callback, TimeSpan interval)
    {
        return new TimerEx(
            callback,
            (long)Math.Ceiling(interval.TotalMilliseconds),
            (long)Math.Ceiling(interval.TotalMilliseconds)
        );
    }

    public void Start()
    {
        _timer.Change(_dueTime, _period);
    }

    public void Restart()
    {
        _timer.Change(_dueTime, _period);
    }

    public void Stop()
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _timer.DisposeAsync();
    }
}