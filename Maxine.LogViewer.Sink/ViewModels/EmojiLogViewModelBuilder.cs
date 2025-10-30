using System.Windows;
using System.Windows.Media;
using Maxine.Extensions.Shared;
using Microsoft.Extensions.Logging;
using RayTech.RayLog.MEL;

namespace Maxine.LogViewer.Sink.ViewModels;

public class EmojiLogViewModelBuilder : LogViewModelBuilder
{
    protected readonly EnumMap<LogLevel, string> LevelEmojis = new();

    public EmojiLogViewModelBuilder(Color? defaultForeground) : base(defaultForeground)
    {
        LevelEmojis[LogLevel.Trace] = "💬";
        LevelEmojis[LogLevel.Debug] = "👩‍💻";
        LevelEmojis[LogLevel.Information] = "ℹ";
        LevelEmojis[LogLevel.Warning] = "⚠";
        LevelEmojis[LogLevel.Error] = "❗";
        LevelEmojis[LogLevel.Critical] = "🚨";
    }

    public override ILogViewModel Build(in LogEntry logEvent, ILogViewModel? existing = null)
    {
        if (existing is EmojiLogViewModel logViewModel)
        {
            logViewModel.Emoji = GetEmoji(logEvent.LogLevel);
            
            logViewModel.Id = GetNextId();
            logViewModel.LevelValue = logEvent.LogLevel;
            logViewModel.Timestamp = BuildTimestampElement(logEvent);
            logViewModel.Level = BuildLevelElement(logEvent);
            logViewModel.Category = BuildSourceContextElement(logEvent);
            logViewModel.Message = BuildMessageElement(logEvent);
            logViewModel.Exception = BuildExceptionElement(logEvent);
            logViewModel.ExceptionVisibility = logEvent.Exception != null ? Visibility.Visible : Visibility.Collapsed;
            return logViewModel;
        }

        return new EmojiLogViewModel
        {
            Emoji = GetEmoji(logEvent.LogLevel),

            Id = GetNextId(),
            LevelValue = logEvent.LogLevel,
            Timestamp = BuildTimestampElement(logEvent),
            Level = BuildLevelElement(logEvent),
            Category = BuildSourceContextElement(logEvent),
            Message = BuildMessageElement(logEvent),
            Exception = BuildExceptionElement(logEvent),
            ExceptionVisibility = logEvent.Exception != null ? Visibility.Visible : Visibility.Collapsed
        };
    }

    public virtual EmojiLogViewModelBuilder SetLevelEmoji(LogLevel level, string emoji)
    {
        LevelEmojis[level] = emoji;
        return this;
    }

    protected virtual string GetEmoji(LogLevel level)
    {
        return LevelEmojis.TryGetValue(level, out var value) ? value : string.Empty;
    }
}
