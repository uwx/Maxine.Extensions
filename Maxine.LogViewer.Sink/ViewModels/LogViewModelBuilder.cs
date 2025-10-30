using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Maxine.Extensions.Shared;
using Microsoft.Extensions.Logging;
using RayTech.RayLog.MEL;

namespace Maxine.LogViewer.Sink.ViewModels;

public class LogViewModelBuilder : ILogViewModelBuilder
{
    private readonly Dictionary<Color, SolidColorBrush> _colorCache = new();
    
    protected readonly SolidColorBrush? DefaultForeground;

    protected SolidColorBrush? ExceptionForeground;
    protected TextFormatter? ExceptionTextFormatter;

    protected readonly EnumMap<LogLevel, SolidColorBrush> LevelForegrounds = new();
    protected TextFormatter? LevelTextFormatter;

    protected readonly EnumMap<LogLevel, SolidColorBrush> MessageForegrounds = new();
    protected TextFormatter? MessageTextFormatter;
    protected SolidColorBrush? SourceContextForeground;
    protected TextFormatter? SourceContextTextFormatter;
    protected SolidColorBrush? TimestampForeground;
    protected TextFormatter? TimestampTextFormatter;

    public LogViewModelBuilder(Color? defaultForeground)
    {
        if (defaultForeground is { } color)
            DefaultForeground = new SolidColorBrush(color);
    }

    private uint _nextId = 1;

    public virtual ILogViewModel Build(in LogEntry logEvent, ILogViewModel? existing = null)
    {
        if (existing is LogViewModel logViewModel)
        {
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
        
        return new LogViewModel
        {
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

    protected uint GetNextId() => _nextId++;

    public virtual LogViewModelBuilder SetExceptionForeground(Color color)
    {
        ExceptionForeground = GetSolidColorBrush(color);
        return this;
    }

    public virtual LogViewModelBuilder SetExceptionFormat(TextFormatter textFormatter)
    {
        ExceptionTextFormatter = textFormatter;
        return this;
    }

    public virtual LogViewModelBuilder SetLevelForeground(LogLevel level, Color color)
    {
        LevelForegrounds[level] = GetSolidColorBrush(color);
        return this;
    }

    public virtual LogViewModelBuilder SetLevelsFormat(TextFormatter textFormatter)
    {
        LevelTextFormatter = textFormatter;
        return this;
    }

    public virtual LogViewModelBuilder SetMessageForeground(LogLevel level, Color color)
    {
        MessageForegrounds[level] = GetSolidColorBrush(color);
        return this;
    }

    public virtual LogViewModelBuilder SetMessageFormat(TextFormatter textFormatter)
    {
        MessageTextFormatter = textFormatter;
        return this;
    }

    public virtual LogViewModelBuilder SetCategoryForeground(Color color)
    {
        SourceContextForeground = GetSolidColorBrush(color);
        return this;
    }

    public virtual LogViewModelBuilder SetCategoryFormat(TextFormatter textFormatter)
    {
        SourceContextTextFormatter = textFormatter;
        return this;
    }

    public virtual LogViewModelBuilder SetTimestampForeground(Color color)
    {
        TimestampForeground = GetSolidColorBrush(color);
        return this;
    }

    public virtual LogViewModelBuilder SetTimestampFormat(TextFormatter textFormatter)
    {
        TimestampTextFormatter = textFormatter;
        return this;
    }

    protected static LogViewModelElement BuildLogViewModelElement(LogEntry logEvent, TextFormatter? textFormatter, SolidColorBrush? foreground)
    {
        var baseString = textFormatter?.Invoke(logEvent) ?? string.Empty;

        return new LogViewModelElement(baseString, foreground);
    }

    protected virtual LogViewModelElement BuildExceptionElement(LogEntry logEvent)
    {
        ExceptionForeground ??= DefaultForeground;
        return BuildLogViewModelElement(logEvent, ExceptionTextFormatter, ExceptionForeground);
    }

    protected virtual LogViewModelElement BuildLevelElement(LogEntry logEvent)
    {
        LevelForegrounds.TryGetValue(logEvent.LogLevel, out var foregound);
        return BuildLogViewModelElement(logEvent, LevelTextFormatter, foregound ?? DefaultForeground);
    }

    protected virtual LogViewModelElement BuildMessageElement(LogEntry logEvent)
    {
        MessageForegrounds.TryGetValue(logEvent.LogLevel, out var foregound);
        return BuildLogViewModelElement(logEvent, MessageTextFormatter, foregound ?? DefaultForeground);
    }

    protected virtual LogViewModelElement BuildSourceContextElement(LogEntry logEvent)
    {
        SourceContextForeground ??= DefaultForeground;
        return BuildLogViewModelElement(logEvent, SourceContextTextFormatter, SourceContextForeground);
    }

    protected virtual LogViewModelElement BuildTimestampElement(LogEntry logEvent)
    {
        TimestampForeground ??= DefaultForeground;
        return BuildLogViewModelElement(logEvent, TimestampTextFormatter, TimestampForeground);
    }

    protected SolidColorBrush GetSolidColorBrush(Color color)
    {
        if (!_colorCache.TryGetValue(color, out var brush))
        {
            _colorCache[color] = brush = new SolidColorBrush(color);
        }
        return brush;
    }
}
