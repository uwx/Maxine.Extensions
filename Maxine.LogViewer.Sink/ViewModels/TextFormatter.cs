namespace Maxine.LogViewer.Sink.ViewModels;

/// <summary>
/// Format the log entry into the output.
/// </summary>
/// <param name="logEntry">The log entry.</param>
public delegate string TextFormatter(in LogEntry logEntry);