using System.Windows.Media;

namespace Maxine.LogViewer.Sink.ViewModels;

public readonly record struct LogViewModelElement(string Text, SolidColorBrush? Foreground) : ILogViewModelElement;
