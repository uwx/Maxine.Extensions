using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Maxine.Extensions;
using Maxine.Extensions.Collections;
using Maxine.LogViewer.Sink;
using Maxine.LogViewer.Sink.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModernWpf.Controls;

namespace Maxine.LogViewer.Desktop;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public LogBufferView LogsView { get; internal set; }

    public ItemsControlLogBroker LogBroker { get; }
    public bool IsAutoScrollOn { get; set; }

    public MainWindowViewModel(Dispatcher? dispatcher = null) : this(Colors.White, dispatcher)
    {
    }

    protected MainWindowViewModel(Color foregroundColor, Dispatcher? dispatcher = null)
    {
        LogBroker = new ItemsControlLogBroker(LogFormatter.GetLogViewModelBuilder(foregroundColor), dispatcher ?? Dispatcher.CurrentDispatcher);
        LogsView = new LogBufferView(LogBroker.Logs);
    }

    public bool LogVisible
    {
        get => _logVisible;
        set => SetField(ref _logVisible, value);
    }
    private bool _logVisible;

    #region OnPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    #endregion
}

public sealed class LogBufferView : CircularBufferView<ILogViewModel>
{
    public LogLevel? MinimumLogLevel { set; private get; }
    public CompiledFilters? TextFilter { set; private get; }

    protected override bool IsFiltered => MinimumLogLevel != null || TextFilter != null;
    
    public LogBufferView(ObservableCircularBuffer<ILogViewModel> collection) : base(collection)
    {
    }

    protected override bool PassesFilter(ILogViewModel item)
    {
        if (MinimumLogLevel is { } minimumLogLevel)
        {
            if (((EmojiLogViewModel)item).LevelValue < minimumLogLevel)
            {
                return false;
            }
        }

        if (TextFilter is { } textFilter)
        {
            if (!FilterText(item, textFilter))
                return false;
        }

        return true;
    }

    private static bool FilterText(ILogViewModel item, CompiledFilters textFilter)
    {
        var logViewModel = (EmojiLogViewModel)item;

        var category = string.IsNullOrWhiteSpace(logViewModel.Category.Text) ? null : logViewModel.Category.Text;
        var message = string.IsNullOrWhiteSpace(logViewModel.Message.Text) ? null : logViewModel.Message.Text;
        var exception = string.IsNullOrWhiteSpace(logViewModel.Exception.Text) ? null : logViewModel.Exception.Text;

        if (textFilter.HasPositiveFilters)
        {
            // if none of the elements match a positive filter
            if (
                (category == null || !textFilter.TestPositive(category)) &&
                (message == null || !textFilter.TestPositive(message)) &&
                (exception == null || !textFilter.TestPositive(exception))
            )
            {
                return false;
            }
        }

        // if any of the elements don't match a negative filter
        if (
            (category != null && !textFilter.TestNegative(category)) ||
            (message != null && !textFilter.TestNegative(message)) ||
            (exception != null && !textFilter.TestNegative(exception))
        )
        {
            return false;
        }

        return true;
    }
}

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindowViewModel ViewModel { get; }
    
    private ScrollViewer? _logScrollViewer;

    public MainWindow()
    {
        InitializeComponent();
        
        // PresentationTraceSources.SetTraceLevel(LogViewer.ItemContainerGenerator, PresentationTraceLevel.High);

        DataContext = ViewModel = new MainWindowViewModel(Dispatcher);

        ViewModel.LogsView.CollectionChanged += (_, e) =>
        {
            if (ViewModel.IsAutoScrollOn && e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Reset)
            {
                _logScrollViewer?.ScrollToBottom();
            }
        };

        var levels = Enum.GetValues<LogLevel>().Except(LogLevel.None).ToArray();
        foreach (var level in levels) LevelSwitcher.Items.Add(level.ToString());

        LevelSwitcher.SelectedIndex = 0;
        LevelSwitcher.SelectionChanged += (_, _) =>
        {
            ViewModel.LogsView.MinimumLogLevel = levels[LevelSwitcher.SelectedIndex];
            ViewModel.LogsView.Refresh();
        };

        try
        {
            var app = Application.Current as LogViewerApp;

            app?.OnLoad(ViewModel.LogBroker);
        }
        catch
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                throw;
            }
        }
        
        _debounced = DebounceExtensions.Debounce<string>(text =>
        {
            Dispatcher.Invoke(() =>
            {
                if (text == string.Empty)
                {
                    ViewModel.LogsView.TextFilter = null;
                }
                else
                {
                    ViewModel.LogsView.TextFilter = StringMatching.CompileFilters(text);
                }
        
                ViewModel.LogsView.Refresh();
            });
        }, 250);
    }

    private void LogScrollViewer_OnLoaded(object sender, RoutedEventArgs e)
    {
        _logScrollViewer = (ScrollViewer)sender;
    }

    private readonly Debounced<string> _debounced;

    private void SearchTermTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;

        _debounced(text);
    }
}