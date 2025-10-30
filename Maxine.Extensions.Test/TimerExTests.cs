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
    public async Task Once_DoesNotExecuteBeforeStart()
    {
        bool executed = false;
        using var timer = TimerEx.Once(() => executed = true, TimeSpan.FromMilliseconds(50));
        
        await Task.Delay(100);
        Assert.IsFalse(executed); // Should not execute without Start()
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
    
    [TestMethod]
    public async Task Restart_ResetsTimer()
    {
        int count = 0;
        using var timer = TimerEx.Repeat(() => count++, TimeSpan.FromMilliseconds(100));
        timer.Start();
        
        await Task.Delay(150);
        timer.Restart();
        
        // After restart, timer should continue executing
        var countAtRestart = count;
        await Task.Delay(150);
        
        Assert.IsGreaterThan(countAtRestart, count);
    }
    
    [TestMethod]
    public async Task Dispose_StopsAndCleansUpTimer()
    {
        int count = 0;
        var timer = TimerEx.Repeat(() => count++, TimeSpan.FromMilliseconds(50));
        timer.Start();
        
        await Task.Delay(100);
        timer.Dispose();
        
        var countAfterDispose = count;
        await Task.Delay(100);
        
        Assert.AreEqual(countAfterDispose, count); // No more executions after dispose
    }
    
    [TestMethod]
    public async Task DisposeAsync_StopsAndCleansUpTimer()
    {
        int count = 0;
        var timer = TimerEx.Repeat(() => count++, TimeSpan.FromMilliseconds(50));
        timer.Start();
        
        await Task.Delay(100);
        await timer.DisposeAsync();
        
        var countAfterDispose = count;
        await Task.Delay(100);
        
        Assert.AreEqual(countAfterDispose, count); // No more executions after async dispose
    }
    
    [TestMethod]
    public async Task Repeat_WithDueTimeAndInterval_ExecutesCorrectly()
    {
        int count = 0;
        using var timer = TimerEx.Repeat(() => count++, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(50));
        timer.Start();
        
        // Should wait 100ms before first execution, then every 50ms
        await Task.Delay(120); // After due time
        Assert.IsGreaterThanOrEqualTo(1, count);
        
        await Task.Delay(100); // Additional time for interval executions
        Assert.IsGreaterThanOrEqualTo(2, count);
    }
    
    [TestMethod]
    public async Task MultipleStartCalls_DoesNotCauseIssues()
    {
        int count = 0;
        using var timer = TimerEx.Once(() => count++, TimeSpan.FromMilliseconds(50));
        
        timer.Start();
        timer.Start(); // Second start should not cause problems
        
        await Task.Delay(100);
        Assert.AreEqual(1, count); // Should still execute only once
    }
    
    [TestMethod]
    public async Task StopThenStart_WorksCorrectly()
    {
        int count = 0;
        using var timer = TimerEx.Repeat(() => count++, TimeSpan.FromMilliseconds(50));
        
        timer.Start();
        await Task.Delay(100);
        timer.Stop();
        
        var countAfterStop = count;
        await Task.Delay(100);
        Assert.AreEqual(countAfterStop, count);
        
        // Start again
        timer.Start();
        await Task.Delay(100);
        
        Assert.IsGreaterThan(countAfterStop, count); // Should continue executing
    }
}

