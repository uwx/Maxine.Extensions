using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Maxine.LogViewer.Sink;
using Maxine.LogViewer.Sink.ViewModels;
using Microsoft.Extensions.Logging;

namespace Maxine.LogViewer.Desktop;

public class MainWindowViewModelMock : MainWindowViewModel
{
    public MainWindowViewModelMock() : base(Colors.Black)
    {
        var category = GetType().FullName!;

        LogBroker.AddLogEvent(new LogEntry(LogLevel.Critical, category, new(), null, "test message: critical", DateTimeOffset.Now));
        LogBroker.AddLogEvent(new LogEntry(LogLevel.Debug, category, new(), null, "test message: debug", DateTimeOffset.Now));
        LogBroker.AddLogEvent(new LogEntry(LogLevel.Error, category, new(), null, "test message: error", DateTimeOffset.Now));
        LogBroker.AddLogEvent(new LogEntry(LogLevel.Information, category, new(), null, "test message: info", DateTimeOffset.Now));
        LogBroker.AddLogEvent(new LogEntry(LogLevel.Trace, category, new(), null, "test message: trace", DateTimeOffset.Now));
        LogBroker.AddLogEvent(new LogEntry(LogLevel.Warning, category, new(), null, "test message: warn", DateTimeOffset.Now));
        
        LogBroker.AddLogEvent(new LogEntry(LogLevel.Warning, category, new(), null, "multiline\nmessage\nhi", DateTimeOffset.Now));
        
        LogBroker.AddLogEvent(new LogEntry(LogLevel.Warning, category, new(), new InvalidOperationException(), "exception", DateTimeOffset.Now));

        for (var i = 0; i < 500; i++)
        {
            LogBroker.AddLogEvent(new LogEntry(LogLevel.Warning, category, new(), null, "test message: " + i, DateTimeOffset.Now));
        }

    }
}