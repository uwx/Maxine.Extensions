using System.Windows.Threading;
using Maxine.Extensions.Collections;
using Maxine.LogViewer.Sink;
using Maxine.LogViewer.Sink.ViewModels;

namespace Maxine.LogViewer.Desktop;

public class ItemsControlLogBroker : IWpfLogBroker
{
    private readonly ILogViewModelBuilder _logViewModelBuilder;

    private ILogViewModel? _spareViewModel;

    public ItemsControlLogBroker(ILogViewModelBuilder logViewModelBuilder, Dispatcher dispatcher)
    {
        _logViewModelBuilder = logViewModelBuilder;

        DispatcherQueue = dispatcher;

        Logs.BufferChanged += OnLogsCollectionChanged;
    }

    private void OnLogsCollectionChanged(in NotifyCircularBufferChangedEventArgs<ILogViewModel> e)
    {
        if (e.Action is BufferChangedAction.PopBack or BufferChangedAction.PopFront or BufferChangedAction.Replace)
        {
            _spareViewModel = (ILogViewModel?)e.Operand;
        }
    }

    public void AddLogEvent(in LogEntry logEvent)
    {
        Logs.PushBack(_logViewModelBuilder.Build(logEvent, _spareViewModel));
    }

    public Dispatcher DispatcherQueue { get; }
    public ObservableCircularBuffer<ILogViewModel> Logs { get; } = new(1000);
}
