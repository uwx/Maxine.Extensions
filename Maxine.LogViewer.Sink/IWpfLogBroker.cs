using System.Windows.Threading;

namespace Maxine.LogViewer.Sink;

public interface IWpfLogBroker
{
    void AddLogEvent(in LogEntry logEntry);
    Dispatcher DispatcherQueue { get; }
}
