namespace Maxine.LogViewer.Sink.ViewModels;

public record EmojiLogViewModel : LogViewModel
{
    public required string Emoji { get; set; }
}
