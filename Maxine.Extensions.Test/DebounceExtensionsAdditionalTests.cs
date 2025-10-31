using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class DebounceExtensionsAdditionalTests
{
    [TestMethod]
    public void Debounce_DelaysExecution()
    {
        var executionCount = 0;
        var lastValue = 0;
        Action<int> action = x =>
        {
            executionCount++;
            lastValue = x;
        };

        var debounced = action.Debounce(100);

        debounced(1);
        debounced(2);
        debounced(3);

        // Should not have executed yet
        Assert.AreEqual(0, executionCount);

        Thread.Sleep(150);

        // Should have executed once with last value
        Assert.AreEqual(1, executionCount);
        Assert.AreEqual(3, lastValue);
    }

    [TestMethod]
    public void Debounce_MultipleCallsOnlyExecutesLast()
    {
        var values = new List<int>();
        Action<int> action = x => values.Add(x);

        var debounced = action.Debounce(50);

        debounced(1);
        Thread.Sleep(10);
        debounced(2);
        Thread.Sleep(10);
        debounced(3);
        Thread.Sleep(10);
        debounced(4);

        Thread.Sleep(100);

        Assert.AreEqual(1, values.Count);
        Assert.AreEqual(4, values[0]);
    }

    [TestMethod]
    public void Debounce_RespectsDelay()
    {
        var executionCount = 0;
        Action<int> action = x => executionCount++;

        var debounced = action.Debounce(200);

        debounced(1);
        Thread.Sleep(100);
        
        Assert.AreEqual(0, executionCount);
        
        Thread.Sleep(150);
        
        Assert.AreEqual(1, executionCount);
    }

    [TestMethod]
    public void Debounce_RestartTimer_OnNewCall()
    {
        var executionCount = 0;
        Action<int> action = x => executionCount++;

        var debounced = action.Debounce(100);

        debounced(1);
        Thread.Sleep(60);
        debounced(2); // This should restart the timer
        Thread.Sleep(60);
        
        // Should not have executed yet (only 120ms total, but timer restarted at 60ms)
        Assert.AreEqual(0, executionCount);
        
        Thread.Sleep(60);
        
        // Now it should have executed
        Assert.AreEqual(1, executionCount);
    }

    [TestMethod]
    public void Debounce_WithString_Works()
    {
        var lastValue = "";
        Action<string> action = x => lastValue = x;

        var debounced = action.Debounce(50);

        debounced("hello");
        debounced("world");
        debounced("test");

        Thread.Sleep(100);

        Assert.AreEqual("test", lastValue);
    }

    [TestMethod]
    public void Debounce_ZeroDelay_ExecutesQuickly()
    {
        var executionCount = 0;
        Action<int> action = x => executionCount++;

        var debounced = action.Debounce(0);

        debounced(1);
        Thread.Sleep(50);

        Assert.AreEqual(1, executionCount);
    }

    [TestMethod]
    public void Debounce_MultipleInvocations_OnlyLastValueUsed()
    {
        var receivedValues = new List<string>();
        Action<string> action = x => receivedValues.Add(x);

        var debounced = action.Debounce(50);

        debounced("first");
        debounced("second");
        debounced("third");
        debounced("fourth");

        Thread.Sleep(100);

        Assert.AreEqual(1, receivedValues.Count);
        Assert.AreEqual("fourth", receivedValues[0]);
    }

    [TestMethod]
    public void Debounce_CustomDelay_Works()
    {
        var executionCount = 0;
        Action<int> action = x => executionCount++;

        var debounced = action.Debounce(300);

        debounced(1);
        Thread.Sleep(200);
        Assert.AreEqual(0, executionCount);

        Thread.Sleep(150);
        Assert.AreEqual(1, executionCount);
    }
}
