using Microsoft.Extensions.Logging;
using RayTech.RayLog.MEL;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Maxine.Extensions.Test;

[TestClass]
public class LoggerTests
{
    [TestMethod]
    public void TestInterpolatedStringHandler()
    {
        var logger = new DummyLogger<LoggerTests>();

        var dummy1 = new DummyObject("foobar");
        var dummy2 = new DummyObject("foobaz");
        
        logger.LogInformation($"hi: {dummy1}, {dummy2:@}");
        
        Console.WriteLine("state: " + logger.State?.GetType() + ", " + logger.State);

        Assert.AreEqual("hi: DummyObject { Value = foobar }, DummyObject { Value = foobaz }", logger.State?.ToString());
    }

    [TestMethod]
    public void TestInterpolatedStringHandlerRemoveProblematicChars()
    {
        const string input = "(pokemonIcons.KeyToFileMap.Count / 50) + (pokemonIcons.KeyToFileMap.Count % 50 != 0 ? 1 : 0)";
        const string expct = "(pokemonIcons.KeyToFileMap.Count / 50) + (pokemonIcons.KeyToFileMap.Count % 50 != 0 ? 1  0)";
        
        var v = StructuredLoggingInterpolatedStringHandler.RemoveProblematicChars(input, input.IndexOfAny(StructuredLoggingInterpolatedStringHandler.CharsToRemove));
        Console.WriteLine(v);
        
        // same string as above, no ":" symbol
        Assert.AreEqual(expct, v);
    }

    [TestMethod]
    public void TestInterpolatedStringHandlerRemoveProblematicCharsProducesNoName()
    {
        const string input = "";
        const string expct = "noName";
        
        var v = StructuredLoggingInterpolatedStringHandler.RemoveProblematicChars(input, input.IndexOfAny(StructuredLoggingInterpolatedStringHandler.CharsToRemove));
        Console.WriteLine(v);
        
        // same string as above, no ":" symbol
        Assert.AreEqual(expct, v);
    }
}

public record DummyObject(string Value);

public class DummyLogger<T> : ILogger<T>
{
    public LogLevel LogLevel { get; private set; }
    public EventId EventId { get; private set; }
    public object? State { get; private set; }
    public Exception? Exception { get; private set; }
    public object? Formatter { get; private set; }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Formatter = formatter;
        Exception = exception;
        State = state;
        EventId = eventId;
        LogLevel = logLevel;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}