namespace Maxine.LogViewer.Sink.ViewModels;

/// <summary>
/// Represents an element in a log view model.
/// </summary>
public interface ILogViewModelElement
{
    /// <summary>
    /// The text for the element.
    /// </summary>
    string Text { get; }
}
