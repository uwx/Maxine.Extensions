namespace Maxine.Extensions.Test;

[TestClass]
public class ValueTaskAsyncLockTests
{
    [TestMethod]
    public async Task LockAsync_CanBeAcquiredAndReleased()
    {
        var asyncLock = new ValueTaskAsyncLock();
        using (await asyncLock.LockAsync())
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public async Task LockAsync_WithCancellationToken_CanBeAcquired()
    {
        var asyncLock = new ValueTaskAsyncLock();
        var cts = new CancellationTokenSource();
        
        using (await asyncLock.LockAsync(cts.Token))
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public async Task LockAsync_SerializesAccess()
    {
        var asyncLock = new ValueTaskAsyncLock();
        int counter = 0;
        bool concurrentAccess = false;

        var tasks = Enumerable.Range(0, 5).Select(async _ =>
        {
            using (await asyncLock.LockAsync())
            {
                var currentValue = counter;
                await Task.Delay(10);
                counter = currentValue + 1;
                
                // If there's concurrent access, counter would be modified during our delay
                if (counter != currentValue + 1)
                    concurrentAccess = true;
            }
        });

        await Task.WhenAll(tasks);
        
        Assert.AreEqual(5, counter);
        Assert.IsFalse(concurrentAccess);
    }

    [TestMethod]
    public async Task LockAsync_Dispose_ReleasesLock()
    {
        var asyncLock = new ValueTaskAsyncLock();
        
        var lock1 = await asyncLock.LockAsync();
        lock1.Dispose();
        
        // Should be able to acquire lock again
        using var lock2 = await asyncLock.LockAsync();
        Assert.IsTrue(true);
    }
}
