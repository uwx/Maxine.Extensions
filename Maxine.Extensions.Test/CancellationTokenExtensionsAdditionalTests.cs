using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class CancellationTokenExtensionsAdditionalTests
{
    [TestMethod]
    public void Register_WithTypedState_InvokesCallbackWithCorrectState()
    {
        var cts = new CancellationTokenSource();
        var state = "test state";
        string? receivedState = null;

        cts.Token.Register<string>(s => receivedState = s, state);
        cts.Cancel();

        Assert.AreEqual("test state", receivedState);
    }

    [TestMethod]
    public void Register_WithTypedState_AlreadyCanceled_RunsImmediately()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var executed = false;
        cts.Token.Register<int>(_ => executed = true, 42);

        Assert.IsTrue(executed);
    }

    [TestMethod]
    public void Register_WithCancellationToken_ReceivesBothStateAndToken()
    {
        var cts = new CancellationTokenSource();
        var state = 123;
        int? receivedState = null;
        CancellationToken? receivedToken = null;

        cts.Token.Register<int>((s, ct) =>
        {
            receivedState = s;
            receivedToken = ct;
        }, state);

        cts.Cancel();

        Assert.AreEqual(123, receivedState);
        Assert.IsTrue(receivedToken.HasValue);
        Assert.IsTrue(receivedToken.Value.IsCancellationRequested);
    }

    [TestMethod]
    public void Register_WithSyncContext_InvokesCallback()
    {
        var cts = new CancellationTokenSource();
        var executed = false;

        cts.Token.Register<bool>(s => executed = s, true, useSynchronizationContext: false);
        cts.Cancel();

        Assert.IsTrue(executed);
    }

    [TestMethod]
    public void Register_WithSyncContext_PassesState()
    {
        var cts = new CancellationTokenSource();
        var state = "sync context test";
        string? receivedState = null;

        cts.Token.Register<string>(s => receivedState = s, state, useSynchronizationContext: false);
        cts.Cancel();

        Assert.AreEqual("sync context test", receivedState);
    }

    [TestMethod]
    public void UnsafeRegister_WithTypedState_InvokesCallback()
    {
        var cts = new CancellationTokenSource();
        var state = 999;
        int? receivedState = null;

        cts.Token.UnsafeRegister<int>(s => receivedState = s, state);
        cts.Cancel();

        Assert.AreEqual(999, receivedState);
    }

    [TestMethod]
    public void UnsafeRegister_WithCancellationToken_ReceivesBoth()
    {
        var cts = new CancellationTokenSource();
        var state = "unsafe";
        string? receivedState = null;
        bool tokenWasCancelled = false;

        cts.Token.UnsafeRegister<string>((s, ct) =>
        {
            receivedState = s;
            tokenWasCancelled = ct.IsCancellationRequested;
        }, state);

        cts.Cancel();

        Assert.AreEqual("unsafe", receivedState);
        Assert.IsTrue(tokenWasCancelled);
    }

    [TestMethod]
    public void UnsafeRegister_NoState_InvokesAction()
    {
        var cts = new CancellationTokenSource();
        var executed = false;

        cts.Token.UnsafeRegister(() => executed = true);
        cts.Cancel();

        Assert.IsTrue(executed);
    }

    [TestMethod]
    public void UnsafeRegister_NoState_AlreadyCanceled_RunsImmediately()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var executed = false;
        cts.Token.UnsafeRegister(() => executed = true);

        Assert.IsTrue(executed);
    }

    [TestMethod]
    public void WithCancelAfter_SchedulesCancellation()
    {
        var cts = new CancellationTokenSource();
        
        cts.WithCancelAfter(TimeSpan.FromMilliseconds(50));

        Assert.IsFalse(cts.Token.IsCancellationRequested);
        Thread.Sleep(100);
        Assert.IsTrue(cts.Token.IsCancellationRequested);
    }

    [TestMethod]
    public void WithCancelAfter_ReturnsOriginalCts()
    {
        var cts = new CancellationTokenSource();
        
        var result = cts.WithCancelAfter(TimeSpan.FromSeconds(10));

        Assert.AreSame(cts, result);
    }

    [TestMethod]
    public void WithCancelAfter_CanBeChained()
    {
        var cts = new CancellationTokenSource()
            .WithCancelAfter(TimeSpan.FromMilliseconds(50));

        Assert.IsFalse(cts.Token.IsCancellationRequested);
        Thread.Sleep(100);
        Assert.IsTrue(cts.Token.IsCancellationRequested);
    }

    [TestMethod]
    public void Register_MultipleCallbacks_AllInvoked()
    {
        var cts = new CancellationTokenSource();
        var count1 = 0;
        var count2 = 0;

        cts.Token.Register<int>(_ => count1++, 1);
        cts.Token.Register<int>(_ => count2++, 2);

        cts.Cancel();

        Assert.AreEqual(1, count1);
        Assert.AreEqual(1, count2);
    }

    [TestMethod]
    public void UnsafeRegister_Dispose_UnregistersCallback()
    {
        var cts = new CancellationTokenSource();
        var executed = false;

        var registration = cts.Token.UnsafeRegister(() => executed = true);
        registration.Dispose();

        cts.Cancel();

        Assert.IsFalse(executed);
    }

    [TestMethod]
    public void Register_ComplexObjectState_PreservesObject()
    {
        var cts = new CancellationTokenSource();
        var state = new { Name = "Test", Value = 42 };
        object? receivedState = null;

        cts.Token.Register(s => receivedState = s, state);
        cts.Cancel();

        Assert.IsNotNull(receivedState);
        Assert.AreSame(state, receivedState);
    }

    [TestMethod]
    public void UnsafeRegister_WithCancellationToken_TokenMatchesSource()
    {
        var cts = new CancellationTokenSource();
        CancellationToken? receivedToken = null;

        cts.Token.UnsafeRegister<int>((_, ct) => receivedToken = ct, 0);
        cts.Cancel();

        Assert.IsTrue(receivedToken.HasValue);
        Assert.AreEqual(cts.Token, receivedToken.Value);
    }
}
