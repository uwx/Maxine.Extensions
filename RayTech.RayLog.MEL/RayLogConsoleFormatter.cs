using System.Runtime.CompilerServices;
using Maxine.Extensions.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace RayTech.RayLog.MEL;

public class RayLogConsoleFormatter : ConsoleFormatter
{
    public const string FormatterName = "raylog";

    public RayLogConsoleFormatter() : base(FormatterName)
    {
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        var exception = logEntry.Exception;
        var message = logEntry.Formatter(logEntry.State, exception);
        if (exception == null && message == null!)
        {
            return;
        }
        RayLogConsole.Write(textWriter, logEntry.LogLevel, logEntry.Category, message, exception);
    }
}

public static class RayLogConsole
{
    private static readonly EnumMap<LogLevel, ConsoleColors> LogLevelColors = new()
    {
        [LogLevel.Trace] = new ConsoleColors(ConsoleColor.DarkGray),
        [LogLevel.Debug] = new ConsoleColors(ConsoleColor.DarkMagenta),
        [LogLevel.Information] = new ConsoleColors(ConsoleColor.Blue),
        [LogLevel.Warning] = new ConsoleColors(ConsoleColor.Yellow),
        [LogLevel.Error] = new ConsoleColors(ConsoleColor.Red),
        [LogLevel.Critical] = new ConsoleColors(ConsoleColor.Black, ConsoleColor.DarkRed),
        [LogLevel.None] = new ConsoleColors(),
    };

    public static void Log(LogLevel logLevel, string message, Exception? exception = null, [CallerMemberName] string category = "NoCategory")
    {
        Write(logLevel >= LogLevel.Error ? Console.Error : Console.Out, logLevel, "DirectLogging." + category, message, exception);
    }

    public static void Write(TextWriter textWriter, LogLevel logLevel, string category, string message, Exception? exception = null)
    {
        var logLevelColors = ConsoleUtils.EmitAnsiColorCodes ? LogLevelColors[logLevel] : new ConsoleColors();
        var logLevelString = GetLogLevelString(logLevel);

        ConsoleColor? textColor = ConsoleUtils.EmitAnsiColorCodes ? ConsoleColor.DarkGray : null;

        textWriter.WriteColoredMessage(RayLogUtils.FormatNow(), foreground: textColor);
        textWriter.Write(' ');
        textWriter.WriteColoredMessage(logLevelString, logLevelColors.Background, logLevelColors.Foreground);
        textWriter.Write(' ');
        textWriter.WriteColoredMessage(category, foreground: textColor);
        textWriter.Write(' ');

        // TODO: scope information?

        WriteMessage(textWriter, message);

        if (exception != null)
        {
            // exception message
            textWriter.Write(Environment.NewLine);
            WriteMessage(textWriter, exception.ToString());
        }

        textWriter.Write(Environment.NewLine);
    }

    private static void WriteMessage(TextWriter textWriter, string? message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            WriteReplacing(textWriter, Environment.NewLine, $"    {Environment.NewLine}", message);
        }

        static void WriteReplacing(TextWriter writer, string oldValue, string newValue, string message)
        {
            var newMessage = message.Replace(oldValue, newValue);
            writer.Write(newMessage);
        }
    }
    
    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            LogLevel.None => "unkn",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    private readonly record struct ConsoleColors(ConsoleColor? Foreground = null, ConsoleColor? Background = null);
}

file static class TextWriterExtensions
{
    public static string GetForegroundColorEscapeCode(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => "\x1B[38;5;0m", // Black
            ConsoleColor.DarkBlue => "\x1B[38;5;4m", // Blue
            ConsoleColor.DarkGreen => "\x1B[38;5;2m", // Green
            ConsoleColor.DarkCyan => "\x1B[38;5;6m", // Cyan
            ConsoleColor.DarkRed => "\x1B[38;5;1m", // Red
            ConsoleColor.DarkMagenta => "\x1B[38;5;5m", // Purple
            ConsoleColor.DarkYellow => "\x1B[38;5;3m", // Brown
            ConsoleColor.Gray => "\x1B[38;5;7m", // Gray
            
            ConsoleColor.DarkGray => "\x1B[38;5;8m", // Dark Gray
            ConsoleColor.Blue => "\x1B[38;5;12m", // Light Blue
            ConsoleColor.Green => "\x1B[38;5;10m", // Light Green
            ConsoleColor.Cyan => "\x1B[38;5;14m", // Light Cyan
            ConsoleColor.Red => "\x1B[38;5;9m", // Light Red
            ConsoleColor.Magenta => "\x1B[38;5;13m", // Light Purple
            ConsoleColor.Yellow => "\x1B[38;5;11m", // Yellow
            ConsoleColor.White => "\x1B[38;5;15m", // White
            
            _ => "\x1B[39m" // Use default foreground color
        };
    }

    public static string GetBackgroundColorEscapeCode(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => "\x1B[48;5;0m", // Black
            ConsoleColor.DarkBlue => "\x1B[48;5;4m", // Blue
            ConsoleColor.DarkGreen => "\x1B[48;5;2m", // Green
            ConsoleColor.DarkCyan => "\x1B[48;5;6m", // Cyan
            ConsoleColor.DarkRed => "\x1B[48;5;1m", // Red
            ConsoleColor.DarkMagenta => "\x1B[48;5;5m", // Purple
            ConsoleColor.DarkYellow => "\x1B[48;5;3m", // Brown
            ConsoleColor.Gray => "\x1B[48;5;7m", // Gray
            
            ConsoleColor.DarkGray => "\x1B[48;5;8m", // Dark Gray
            ConsoleColor.Blue => "\x1B[48;5;12m", // Light Blue
            ConsoleColor.Green => "\x1B[48;5;10m", // Light Green
            ConsoleColor.Cyan => "\x1B[48;5;14m", // Light Cyan
            ConsoleColor.Red => "\x1B[48;5;9m", // Light Red
            ConsoleColor.Magenta => "\x1B[48;5;13m", // Light Purple
            ConsoleColor.Yellow => "\x1B[48;5;11m", // Yellow
            ConsoleColor.White => "\x1B[48;5;15m", // White
            
            _ => "\x1B[49m" // Use default background color
        };
    }
    
    private const string ResetColor = "\x1B[m";

    public static void WriteColoredMessage(this TextWriter textWriter, string message, ConsoleColor? background = null, ConsoleColor? foreground = null)
    {
        // Order: backgroundcolor, foregroundcolor, Message, reset foregroundcolor, reset backgroundcolor
        if (background is {} bgValue)
        {
            textWriter.Write(GetBackgroundColorEscapeCode(bgValue));
        }
        if (foreground is {} fgValue)
        {
            textWriter.Write(GetForegroundColorEscapeCode(fgValue));
        }
        textWriter.Write(message);
        if (foreground.HasValue || background.HasValue)
        {
            textWriter.Write(ResetColor); // reset to default foreground color
        }
    }
}

// From System.ConsoleUtils
file static class ConsoleUtils
{
    /// <summary>Get whether to emit ANSI color codes.</summary>
    public static readonly bool EmitAnsiColorCodes;

    static ConsoleUtils()
    {
        // By default, we emit ANSI color codes if output isn't redirected, and suppress them if output is redirected.
        var enabled = !Console.IsOutputRedirected;

        if (enabled)
        {
            // We subscribe to the informal standard from https://no-color.org/.  If we'd otherwise emit
            // ANSI color codes but the NO_COLOR environment variable is set, disable emitting them.
            enabled = Environment.GetEnvironmentVariable("NO_COLOR") is null;
        }
        else
        {
            // We also support overriding in the other direction.  If we'd otherwise avoid emitting color
            // codes but the DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION environment variable is
            // set to 1 or true, enable color.
            var envVar = Environment.GetEnvironmentVariable("DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION");
            enabled = envVar is not null && (envVar == "1" || envVar.Equals("true", StringComparison.OrdinalIgnoreCase));
        }

        // Store and return the computed answer.
        EmitAnsiColorCodes = enabled;
    }
}