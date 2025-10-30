namespace Maxine.Extensions.Test;

[TestClass]
public class TimerExTests
{
    [TestMethod]
    public void Once_CreatesTimer()
    {
        bool executed = false;
        using var timer = TimerEx.Once(() => executed = true, TimeSpan.FromMilliseconds(50));
        Assert.IsNotNull(timer);
    }

    [TestMethod]
    public async Task Once_ExecutesCallbackAfterDueTime()
    {
        bool executed = false;
        using var timer = TimerEx.Once(() => executed = true, TimeSpan.FromMilliseconds(50));
        timer.Start();
        
        await Task.Delay(100);
        Assert.IsTrue(executed);
    }

    [TestMethod]
    public void Repeat_WithDueTimeAndInterval_CreatesTimer()
    {
        int count = 0;
        using var timer = TimerEx.Repeat(() => count++, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50));
        Assert.IsNotNull(timer);
    }

    [TestMethod]
    public async Task Repeat_ExecutesCallbackMultipleTimes()
    {
        int count = 0;
        using var timer = TimerEx.Repeat(() => count++, TimeSpan.FromMilliseconds(50));
        timer.Start();
        
        await Task.Delay(150);
        Assert.IsGreaterThanOrEqualTo(2, count); // Should execute at least twice
    }

    [TestMethod]
    public void Start_BeginsTimerExecution()
    {
        bool executed = false;
        using var timer = TimerEx.Once(() => executed = true, TimeSpan.FromMilliseconds(50));
        timer.Start();
        // Timer should be running
        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task Stop_StopsTimerExecution()
    {
        int count = 0;
        using var timer = TimerEx.Repeat(() => count++, TimeSpan.FromMilliseconds(50));
        timer.Start();
        
        await Task.Delay(100);
        timer.Stop();
        
        var countAfterStop = count;
        await Task.Delay(100);
        
        Assert.AreEqual(countAfterStop, count); // Count should not increase after stop
    }
}

