using System.Windows;
using Microsoft.Extensions.Logging;

namespace Maxine.LogViewer.Sink.ViewModels;

public record LogViewModel : ILogViewModel
{
    public required uint Id { get; set; }
    public required LogLevel LevelValue { get; set; }
    public required LogViewModelElement Timestamp { get; set; }
    public required LogViewModelElement Level { get; set; }
    public required LogViewModelElement Category { get; set; }
    public required LogViewModelElement Message { get; set; }
    public required LogViewModelElement Exception { get; set; }
    public required Visibility ExceptionVisibility { get; set; }
}
