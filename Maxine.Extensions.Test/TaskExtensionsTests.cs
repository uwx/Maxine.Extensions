using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class TaskExtensionsTests
{
    [TestMethod]
    public async Task ContinueWithRelativelySafely_TaskWithResult_ExecutesContinuation()
    {
        var task = Task.FromResult(42);
        
        var result = await task.ContinueWithRelativelySafely(t => t.Result * 2);
        
        Assert.AreEqual(84, result);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_TaskWithResult_AccessesPreviousResult()
    {
        var task = Task.FromResult("Hello");
        
        var result = await task.ContinueWithRelativelySafely(t => t.Result + " World");
        
        Assert.AreEqual("Hello World", result);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_Task_ExecutesContinuation()
    {
        var task = Task.CompletedTask;
        bool executed = false;
        
        await task.ContinueWithRelativelySafely(t => 
        {
            executed = true;
        });
        
        Assert.IsTrue(executed);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_DelayedTask_WaitsForCompletion()
    {
        var task = Task.Delay(50).ContinueWith(_ => 100);
        
        var result = await task.ContinueWithRelativelySafely(t => t.Result + 1);
        
        Assert.AreEqual(101, result);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_WithCancellationToken_Cancels()
    {
        using var cts = new CancellationTokenSource();
        var task = Task.Delay(5000);
        
        cts.Cancel();
        
        try
        {
            await task.ContinueWithRelativelySafely(
                t => { },
                cts.Token);
            
            Assert.Fail("Should have thrown TaskCanceledException");
        }
        catch (TaskCanceledException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_TaskResult_WithCancellationToken_Cancels()
    {
        using var cts = new CancellationTokenSource();
        var task = Task.Delay(5000).ContinueWith(_ => 42);
        
        cts.Cancel();
        
        try
        {
            await task.ContinueWithRelativelySafely(
                t => t.Result,
                cts.Token);
            
            Assert.Fail("Should have thrown TaskCanceledException");
        }
        catch (TaskCanceledException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_ChainedContinuations_ExecutesInOrder()
    {
        var task = Task.FromResult(1);
        
        var result = await task
            .ContinueWithRelativelySafely(t => t.Result + 1)
            .ContinueWithRelativelySafely(t => t.Result * 2)
            .ContinueWithRelativelySafely(t => t.Result + 10);
        
        Assert.AreEqual(14, result); // (1 + 1) * 2 + 10 = 14
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_FaultedTask_PassesFaultToSafeContinuation()
    {
        var task = Task.FromException<int>(new InvalidOperationException("Test error"));
        
        Exception? caughtException = null;
        await task.ContinueWithRelativelySafely(t =>
        {
            try
            {
                var _ = t.Result;
                return 0;
            }
            catch (Exception ex)
            {
                caughtException = ex;
                return -1;
            }
        });
        
        Assert.IsNotNull(caughtException);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_Task_CanAccessTaskStatus()
    {
        var task = Task.CompletedTask;
        TaskStatus? status = null;
        
        await task.ContinueWithRelativelySafely(t =>
        {
            status = t.Status;
        });
        
        Assert.AreEqual(TaskStatus.RanToCompletion, status);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_MultipleResults_AllExecute()
    {
        var tasks = Enumerable.Range(1, 5)
            .Select(i => Task.FromResult(i))
            .Select(t => t.ContinueWithRelativelySafely(task => task.Result * 2));
        
        var results = await Task.WhenAll(tasks);
        
        CollectionAssert.AreEqual(new[] { 2, 4, 6, 8, 10 }, results);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_TransformType_WorksCorrectly()
    {
        var task = Task.FromResult(123);
        
        var result = await task.ContinueWithRelativelySafely(t => t.Result.ToString());
        
        Assert.AreEqual("123", result);
    }
}
