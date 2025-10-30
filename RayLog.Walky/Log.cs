using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace RayTech.RayLog.MEL;

[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
public static class Log
{
    private static readonly EnumMap<LogLevel, ConsoleColors> LogLevelColors = new(
        KeyValuePair.Create(LogLevel.Trace, new ConsoleColors(ConsoleColor.DarkGray)),
        KeyValuePair.Create(LogLevel.Debug, new ConsoleColors(ConsoleColor.DarkMagenta)),
        KeyValuePair.Create(LogLevel.Information, new ConsoleColors(ConsoleColor.Blue)),
        KeyValuePair.Create(LogLevel.Warning, new ConsoleColors(ConsoleColor.Yellow)),
        KeyValuePair.Create(LogLevel.Error, new ConsoleColors(ConsoleColor.Red)),
        KeyValuePair.Create(LogLevel.Critical, new ConsoleColors(ConsoleColor.Black, ConsoleColor.DarkRed)),
        KeyValuePair.Create(LogLevel.None, new ConsoleColors(null))
    );

    public static void LogLog(
        LogLevel logLevel,
        string message,
        Exception? exception = null,
        int lineNumber = -1,
        string path = "",
        string member = ""
    )
    {
        Write(
            logLevel >= LogLevel.Error ? Console.Error : Console.Out,
            logLevel,
            $"{Path.GetFileNameWithoutExtension(path)} {member}:{lineNumber}",
            message,
            exception
        );
    }
    
    public static void Trace(string message, Exception? exception = null, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string path = "", [CallerMemberName] string category = "")
        => LogLog(LogLevel.Trace, message, exception, lineNumber, path, category);
    public static void Debug(string message, Exception? exception = null, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string path = "", [CallerMemberName] string category = "")
        => LogLog(LogLevel.Debug, message, exception, lineNumber, path, category);
    public static void Information(string message, Exception? exception = null, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string path = "", [CallerMemberName] string category = "")
        => LogLog(LogLevel.Information, message, exception, lineNumber, path, category);
    public static void Warning(string message, Exception? exception = null, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string path = "", [CallerMemberName] string category = "")
        => LogLog(LogLevel.Warning, message, exception, lineNumber, path, category);
    public static void Error(string message, Exception? exception = null, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string path = "", [CallerMemberName] string category = "")
        => LogLog(LogLevel.Error, message, exception, lineNumber, path, category);
    public static void Critical(string message, Exception? exception = null, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string path = "", [CallerMemberName] string category = "")
        => LogLog(LogLevel.Critical, message, exception, lineNumber, path, category);
    public static void Error(Exception exception, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string path = "", [CallerMemberName] string category = "")
        => LogLog(LogLevel.Error, "An error occurred.", exception, lineNumber, path, category);
    public static void Critical(Exception exception, [CallerLineNumber] int lineNumber = -1, [CallerFilePath] string path = "", [CallerMemberName] string category = "")
        => LogLog(LogLevel.Critical, "An error occurred.", exception, lineNumber, path, category);

    private static void Write(TextWriter textWriter, LogLevel logLevel, string category, string message, Exception? exception = null)
    {
        var logLevelColors = LogLevelColors[logLevel];
        var logLevelString = GetLogLevelString(logLevel);

        textWriter.WriteColoredMessage(RayLogUtils.FormatNow(), foreground: ConsoleColor.DarkGray);
        textWriter.Write(' ');
        textWriter.WriteColoredMessage(logLevelString, logLevelColors.Background, logLevelColors.Foreground);
        textWriter.Write(' ');
        textWriter.WriteColoredMessage(category, foreground: ConsoleColor.DarkGray);
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

    private readonly record struct ConsoleColors(ConsoleColor? Foreground, ConsoleColor? Background = null);

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