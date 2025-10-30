using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Maxine.LogViewer.Sink;

/// <summary>
/// Holds the information for a single log entry.
/// </summary>
/// <param name="LogLevel">The log level.</param>
/// <param name="Category">The category name for the log.</param>
/// <param name="EventId">The log event Id.</param>
/// <param name="Exception">The log exception.</param>
/// <param name="Message">The formatted log message.</param>
public readonly record struct LogEntry(LogLevel LogLevel, string Category, EventId EventId, Exception? Exception, string Message, DateTimeOffset Timestamp);

internal class WinUi3Logger : ILogger
{
    private readonly WpfLoggerProvider _sink;
    private readonly string _categoryName;

    public WinUi3Logger(WpfLoggerProvider sink, string categoryName)
    {
        _sink = sink;
        _categoryName = categoryName;
    }
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _sink.Emit(new LogEntry<TState>(logLevel, _categoryName, eventId, state, exception, formatter));
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => default!;
}

internal class WpfLoggerProvider : ILoggerProvider, IDisposable
{
    private readonly IWpfLogBroker _logBroker;
    private readonly Channel<LogEntry> _channel = Channel.CreateUnbounded<LogEntry>();
    private readonly Dispatcher _dispatcherQueue;
    private readonly DispatcherPriority _dispatcherQueuePriority;
    private readonly TimeSpan _loggingInterval;
    private CancellationTokenSource? _cancellationTokenSource;

    private int _started;
    private readonly ConcurrentDictionary<string, WinUi3Logger> _loggers = new(StringComparer.OrdinalIgnoreCase);

    public WpfLoggerProvider(
        IWpfLogBroker logBroker,
        DispatcherPriority dispatcherQueuePriority,
        TimeSpan loggingInterval
    )
    {
        _dispatcherQueue = logBroker.DispatcherQueue;
        _dispatcherQueuePriority = dispatcherQueuePriority;
        _logBroker = logBroker;
        _loggingInterval = loggingInterval;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _channel.Writer.TryComplete();
        _loggers.Clear();
    }

    public ILogger CreateLogger(string categoryName)
    {
        if (Interlocked.Exchange(ref _started, 1) == 0)
        {
            StartUpdate();
        }

        return _loggers.GetOrAdd(categoryName, _ => new WinUi3Logger(this, categoryName));
    }

    public void Emit<T>(LogEntry<T> logEvent)
    {
        var message = logEvent.Formatter!(logEvent.State, logEvent.Exception);
        _ = _channel.Writer.TryWrite(new LogEntry(logEvent.LogLevel, logEvent.Category, logEvent.EventId, logEvent.Exception, message, DateTimeOffset.Now));
    }

    public DispatcherOperation StartUpdate()
    {
        _cancellationTokenSource ??= new();
        var cancellationToken = _cancellationTokenSource.Token;

        return _dispatcherQueue.BeginInvoke(
            _dispatcherQueuePriority,
            async () =>
            {
                if (_loggingInterval != TimeSpan.Zero)
                {
                    var cache = new List<LogEntry>();
                    var stopwatch = new Stopwatch();
                    using PeriodicTimer timer = new(_loggingInterval);
                    try
                    {
                        while (await timer.WaitForNextTickAsync(cancellationToken))
                        {
                            stopwatch.Restart();
                            while (_channel.Reader.TryRead(out var logEvent))
                            {
                                cache.Add(logEvent);

                                if (stopwatch.ElapsedMilliseconds > _loggingInterval.Milliseconds)
                                    break;
                            }

                            foreach (var logEvent in cache)
                            {
                                _logBroker.AddLogEvent(logEvent);
                            }

                            cache.Clear();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
                else
                {
                    try
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        while (await _channel.Reader.WaitToReadAsync(cancellationToken))
                        while (_channel.Reader.TryRead(out var logEvent))
                        {
                            _logBroker.AddLogEvent(logEvent);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            });
    }

    public void StopUpdate()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
}
