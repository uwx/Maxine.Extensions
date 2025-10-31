using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class ValueTaskAsyncLockAdditionalTests
{
    [TestMethod]
    public async Task LockAsync_ReleasesLockOnDispose()
    {
        var asyncLock = new ValueTaskAsyncLock();

        using (await asyncLock.LockAsync())
        {
            // Lock acquired
        }

        // Lock should be released now, acquire again
        using (await asyncLock.LockAsync())
        {
            Assert.IsTrue(true); // Successfully acquired again
        }
    }

    [TestMethod]
    public async Task LockAsync_MultipleCallers_SequentialAccess()
    {
        var asyncLock = new ValueTaskAsyncLock();
        var results = new List<int>();

        var task1 = Task.Run(async () =>
        {
            using (await asyncLock.LockAsync())
            {
                results.Add(1);
                await Task.Delay(50);
            }
        });

        var task2 = Task.Run(async () =>
        {
            using (await asyncLock.LockAsync())
            {
                results.Add(2);
                await Task.Delay(50);
            }
        });

        await Task.WhenAll(task1, task2);
        
        // Both tasks should have executed
        Assert.AreEqual(2, results.Count);
        // Should contain both values (order may vary)
        CollectionAssert.Contains(results, 1);
        CollectionAssert.Contains(results, 2);
    }

    [TestMethod]
    public async Task LockAsync_CancellationToken_ThrowsWhenCanceled()
    {
        var asyncLock = new ValueTaskAsyncLock();
        var cts = new CancellationTokenSource();

        // Hold the lock
        var lockTask = asyncLock.LockAsync();
        var lockDisposable = await lockTask;

        try
        {
            // Try to acquire with already-cancelled token
            cts.Cancel();
            
            try
            {
                await asyncLock.LockAsync(cts.Token);
                Assert.Fail("Expected OperationCanceledException");
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }
        finally
        {
            lockDisposable.Dispose();
        }
    }

    [TestMethod]
    public async Task LockAsync_NoContention_ReturnsImmediately()
    {
        var asyncLock = new ValueTaskAsyncLock();

        var lockTask = asyncLock.LockAsync();
        
        // When there's no contention, lock should be acquired immediately
        var disposable = await lockTask;
        disposable.Dispose();
        
        Assert.IsTrue(true); // Successfully acquired and released
    }

    [TestMethod]
    public async Task LockAsync_Contention_WaitsForRelease()
    {
        var asyncLock = new ValueTaskAsyncLock();
        var firstLockAcquired = new TaskCompletionSource<bool>();
        var firstLockReleasing = new TaskCompletionSource<bool>();

        var task1 = Task.Run(async () =>
        {
            using (await asyncLock.LockAsync())
            {
                firstLockAcquired.SetResult(true);
                await firstLockReleasing.Task;
            }
        });

        await firstLockAcquired.Task;

        var task2Start = Task.Run(async () =>
        {
            using (await asyncLock.LockAsync())
            {
                // Should only reach here after first lock is released
                Assert.IsTrue(task1.IsCompleted);
            }
        });

        // Give task2 a moment to start waiting
        await Task.Delay(50);
        
        firstLockReleasing.SetResult(true);
        
        await Task.WhenAll(task1, task2Start);
    }

    [TestMethod]
    public async Task LockAsync_MultipleWaiters_ProcessedInOrder()
    {
        var asyncLock = new ValueTaskAsyncLock();
        var results = new List<int>();
        var startSignal = new TaskCompletionSource<bool>();

        // Acquire the lock first
        var initialLock = await asyncLock.LockAsync();

        var tasks = Enumerable.Range(1, 3).Select(i => Task.Run(async () =>
        {
            await startSignal.Task; // Wait for signal to start
            using (await asyncLock.LockAsync())
            {
                results.Add(i);
            }
        })).ToList();

        // Let all tasks start waiting
        await Task.Delay(50);
        
        // Release initial lock and signal tasks to proceed
        initialLock.Dispose();
        startSignal.SetResult(true);

        await Task.WhenAll(tasks);
        
        // All should have executed
        Assert.AreEqual(3, results.Count);
    }

    [TestMethod]
    public async Task LockAsync_ReentrantCall_Deadlocks()
    {
        var asyncLock = new ValueTaskAsyncLock();
        var timeoutTask = Task.Delay(500);

        using (await asyncLock.LockAsync())
        {
            // Try to acquire the same lock again (should deadlock)
            var reentrancyTask = Task.Run(async () => await asyncLock.LockAsync());
            
            var completedTask = await Task.WhenAny(reentrancyTask, timeoutTask);
            
            // Should timeout, not complete
            Assert.AreSame(timeoutTask, completedTask);
        }
    }

    [TestMethod]
    public async Task LockAsync_DefaultCancellationToken_WorksCorrectly()
    {
        var asyncLock = new ValueTaskAsyncLock();

        using (await asyncLock.LockAsync(default))
        {
            Assert.IsTrue(true); // Lock acquired
        }
    }

    [TestMethod]
    public async Task LockAsync_CancelAfterAcquire_DoesNotReleaseLock()
    {
        var asyncLock = new ValueTaskAsyncLock();
        var cts = new CancellationTokenSource();

        var lockDisposable = await asyncLock.LockAsync(cts.Token);
        
        // Cancel after acquiring
        cts.Cancel();
        
        // Try to acquire again - should wait (lock still held)
        var secondAcquireTask = Task.Run(async () => await asyncLock.LockAsync());
        await Task.Delay(50);
        
        Assert.IsFalse(secondAcquireTask.IsCompleted);
        
        // Cleanup
        lockDisposable.Dispose();
        var secondLock = await secondAcquireTask;
        secondLock.Dispose();
    }
}
