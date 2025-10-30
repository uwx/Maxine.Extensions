namespace Maxine.LogViewer.Sink.ViewModels;

/// <summary>
/// An interface to construct an <see cref="ILogViewModel"/> from a log entry.
/// </summary>
public interface ILogViewModelBuilder
{
    /// <summary>
    /// Construct an <see cref="ILogViewModel"/> from a log entry.
    /// </summary>
    /// <param name="logEvent">The log event.</param>
    /// <param name="existing">An optional existing object previously returned by a call to this method, for reuse.</param>
    /// <returns>The newly created log view model.</returns>
    ILogViewModel Build(in LogEntry logEvent, ILogViewModel? existing = null);
}
