using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class TaskExtensionsAdditionalTests
{
    [TestMethod]
    public async Task ContinueWithRelativelySafely_TaskT_ExecutesContinuation()
    {
        var task = Task.FromResult(42);
        var continuationExecuted = false;

        var result = await task.ContinueWithRelativelySafely(t =>
        {
            continuationExecuted = true;
            return t.Result * 2;
        });

        Assert.IsTrue(continuationExecuted);
        Assert.AreEqual(84, result);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_TaskT_ReceivesCompletedTask()
    {
        var task = Task.FromResult("Hello");
        Task<string>? receivedTask = null;

        await task.ContinueWithRelativelySafely(t =>
        {
            receivedTask = t;
            return t.Result.Length;
        });

        Assert.IsNotNull(receivedTask);
        Assert.IsTrue(receivedTask.IsCompleted);
        Assert.AreEqual("Hello", receivedTask.Result);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_Task_ExecutesContinuation()
    {
        var task = Task.CompletedTask;
        var continuationExecuted = false;

        await task.ContinueWithRelativelySafely(t =>
        {
            continuationExecuted = true;
        });

        Assert.IsTrue(continuationExecuted);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_Task_ReceivesCompletedTask()
    {
        var task = Task.CompletedTask;
        Task? receivedTask = null;

        await task.ContinueWithRelativelySafely(t =>
        {
            receivedTask = t;
        });

        Assert.IsNotNull(receivedTask);
        Assert.IsTrue(receivedTask.IsCompleted);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_WithDelay_WaitsForCompletion()
    {
        var task = Task.Delay(50).ContinueWith(_ => 100);
        var startTime = DateTime.UtcNow;

        var result = await task.ContinueWithRelativelySafely(t => t.Result * 2);

        var elapsed = DateTime.UtcNow - startTime;
        Assert.IsTrue(elapsed.TotalMilliseconds >= 40); // Some tolerance
        Assert.AreEqual(200, result);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_TaskT_ChainedContinuations()
    {
        var task = Task.FromResult(1);

        var result = await task
            .ContinueWithRelativelySafely(t => t.Result + 1)
            .ContinueWithRelativelySafely(t => t.Result * 2)
            .ContinueWithRelativelySafely(t => t.Result + 10);

        Assert.AreEqual(14, result); // ((1 + 1) * 2) + 10
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_Task_ChainedContinuations()
    {
        var counter = 0;
        var task = Task.CompletedTask;

        await task
            .ContinueWithRelativelySafely(t => counter++)
            .ContinueWithRelativelySafely(t => counter++)
            .ContinueWithRelativelySafely(t => counter++);

        Assert.AreEqual(3, counter);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_WithCancellationToken_Uncanceled()
    {
        var cts = new CancellationTokenSource();
        var task = Task.FromResult(42);

        var result = await task.ContinueWithRelativelySafely(
            t => t.Result,
            cts.Token
        );

        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_TaskT_DifferentReturnType()
    {
        var task = Task.FromResult(123);

        var result = await task.ContinueWithRelativelySafely(t => t.Result.ToString());

        Assert.AreEqual("123", result);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_Task_WithAsyncDelay()
    {
        var executed = false;
        var task = Task.Delay(30);

        await task.ContinueWithRelativelySafely(t =>
        {
            executed = true;
        });

        Assert.IsTrue(executed);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_TaskT_ReturnsTaskOfNewType()
    {
        var task = Task.FromResult(5.5);

        var resultTask = task.ContinueWithRelativelySafely(t => (int)t.Result);

        Assert.IsInstanceOfType<Task<int>>(resultTask);
        var result = await resultTask;
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_MultipleParallelContinuations()
    {
        var task = Task.FromResult(10);
        var count1 = 0;
        var count2 = 0;

        var continuation1 = task.ContinueWithRelativelySafely(t => { count1++; return t.Result; });
        var continuation2 = task.ContinueWithRelativelySafely(t => { count2++; return t.Result; });

        await Task.WhenAll(continuation1, continuation2);

        Assert.AreEqual(1, count1);
        Assert.AreEqual(1, count2);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_Task_ReturnsCompletedTask()
    {
        var task = Task.CompletedTask;
        var executed = false;

        var resultTask = task.ContinueWithRelativelySafely(t => executed = true);

        await resultTask;
        Assert.IsTrue(executed);
        Assert.IsTrue(resultTask.IsCompleted);
    }

    [TestMethod]
    public async Task ContinueWithRelativelySafely_TaskT_WithComplexObject()
    {
        var task = Task.FromResult(new { Name = "Test", Value = 42 });

        var result = await task.ContinueWithRelativelySafely(t => 
            $"{t.Result.Name}: {t.Result.Value}"
        );

        Assert.AreEqual("Test: 42", result);
    }
}
