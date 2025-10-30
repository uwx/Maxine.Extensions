namespace Maxine.Extensions.Test;

[TestClass]
public class BenchmarkTimerTests
{
    [TestMethod]
    public void Timer_CanBeCreatedWithMessage()
    {
        using var timer = new BenchmarkTimer("Test message");
        Thread.Sleep(10);
        // Timer will print elapsed time on dispose
        Assert.IsTrue(true); // Test that it doesn't throw
    }

    [TestMethod]
    public void Timer_CanBeCreatedWithoutMessage()
    {
        using var timer = new BenchmarkTimer();
        Thread.Sleep(10);
        // Timer will print elapsed time on dispose
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Timer_DisposePrintsElapsedTime()
    {
        // Capture console output
        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            using (var timer = new BenchmarkTimer("Test"))
            {
                Thread.Sleep(10);
            }

            var output = stringWriter.ToString();
            Assert.Contains("Test", output);
            Assert.Contains("elapsed", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}

