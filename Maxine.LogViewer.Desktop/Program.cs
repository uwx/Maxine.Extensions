using System;
using System.Threading.Tasks;
using Maxine.LogViewer.Sink;
using Microsoft.Extensions.Logging;

namespace Maxine.LogViewer.Desktop;

public class Program
{
    [STAThread]
    public static void Main()
    {
        var logViewer = new LogViewerApp(a =>
        {
            var category = typeof(Program).FullName!;
            a.AddLogEvent(new LogEntry(LogLevel.Critical, category, new(), null, "test message: critical", DateTimeOffset.Now));
            a.AddLogEvent(new LogEntry(LogLevel.Debug, category, new(), null, "test message: debug", DateTimeOffset.Now));
            a.AddLogEvent(new LogEntry(LogLevel.Error, category, new(), null, "test message: error", DateTimeOffset.Now));
            a.AddLogEvent(new LogEntry(LogLevel.Information, category, new(), null, "test message: info", DateTimeOffset.Now));
            a.AddLogEvent(new LogEntry(LogLevel.Trace, category, new(), null, "test message: trace", DateTimeOffset.Now));
            a.AddLogEvent(new LogEntry(LogLevel.Warning, category, new(), null, "test message: warn", DateTimeOffset.Now));
            
            a.AddLogEvent(new LogEntry(LogLevel.Warning, category, new(), null, "multiline\nmessage\nhi", DateTimeOffset.Now));

            for (var i = 0; i < 10000; i++)
            {
                a.AddLogEvent(new LogEntry(LogLevel.Warning, category, new(), null, "test message: " + i, DateTimeOffset.Now));
            }

            _ = a.DispatcherQueue.InvokeAsync(async () =>
            {
                var i = 0;
                while (true)
                {
                    a.AddLogEvent(new LogEntry((LogLevel)(i % (int)LogLevel.None), category, new(), null, $"newtest message: {i++} {DateTimeOffset.Now}", DateTimeOffset.Now));
                    await Task.Delay(
                        100 // set to 5 to bring your system to its knees
                    );
                }
            });
        });
        logViewer.InitializeComponent();
        logViewer.Run();
    }
}