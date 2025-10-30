using System;
using System.Windows.Media;
using Maxine.LogViewer.Sink;
using Maxine.LogViewer.Sink.ViewModels;
using Microsoft.Extensions.Logging;
using RayTech.RayLog.MEL;

namespace Maxine.LogViewer.Desktop;

public static class LogFormatter
{
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

    internal static LogViewModelBuilder GetLogViewModelBuilder(Color color)
        => new EmojiLogViewModelBuilder(color)
            .SetTimestampFormat(static delegate(in LogEntry entry) { return RayLogUtils.FormatTime(entry.Timestamp); })
            .SetTimestampForeground(Color.FromRgb(89, 89, 89))

            .SetLevelsFormat(static delegate(in LogEntry entry) { return GetLogLevelString(entry.LogLevel); })
            .SetLevelForeground(LogLevel.Trace, Color.FromRgb(89, 89, 89))
            .SetLevelForeground(LogLevel.Debug, Color.FromRgb(131, 74, 123))
            .SetLevelForeground(LogLevel.Information, Color.FromRgb(47, 104, 255))
            .SetLevelForeground(LogLevel.Warning, Color.FromRgb(0xCC, 0xEA, 0x16))
            .SetLevelForeground(LogLevel.Error, Color.FromRgb(228, 40, 29))
            .SetLevelForeground(LogLevel.Critical, Colors.OrangeRed)

            .SetCategoryFormat(static delegate(in LogEntry entry) { return entry.Category; })
            .SetCategoryForeground(Color.FromRgb(89, 89, 89))

            .SetMessageFormat(static delegate(in LogEntry entry) { return entry.Message; })

            .SetExceptionFormat(static delegate(in LogEntry entry) { return entry.Exception?.ToString() ?? string.Empty; })
            .SetExceptionForeground(Colors.Red);
}