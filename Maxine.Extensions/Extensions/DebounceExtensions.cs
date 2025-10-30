namespace Maxine.Extensions.Shared;

public delegate void Debounced<in T>(T argument);

public static class DebounceExtensions
{
    // Roughly based on https://stackoverflow.com/a/29491927
    public static Debounced<T> Debounce<T>(this Action<T> func, int milliseconds = 300)
    {
        var syncRoot = new object();
        TimerEx? timer = null;
        T? lastArg = default;

        void Debounced()
        {
            try
            {
                func(lastArg!);
            }
            finally
            {
                TimerEx oldTimer;

                lock (syncRoot)
                {
                    oldTimer = timer!.Value;
                    timer = null;
                }

                oldTimer.Dispose();
            }
        }

        return arg =>
        {
            lock (syncRoot) // Lock assuming ??= is not fully atomic
            {
                (timer ??= TimerEx.Once(Debounced, TimeSpan.FromMilliseconds(milliseconds))).Restart();
                lastArg = arg;
            }
        };
    }
}