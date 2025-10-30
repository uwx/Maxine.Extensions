using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Threading;
using Maxine.Extensions.Shared;
using ObservableCollections;

namespace Maxine.LogViewer.Desktop;

public abstract class CircularBufferView<T> : DispatcherObject, ICollectionView, INotifyPropertyChanged
    where T : notnull
{
    private readonly ObservableCircularBuffer<T> _collection;
    private ObservableCircularBuffer<T>? _filteredView;

    protected abstract bool IsFiltered { get; }

    protected CircularBufferView(ObservableCircularBuffer<T> collection)
    {
        _collection = collection;
        _collection.BufferChanged += OnSourceCollectionUpdated;
    }

    private void OnSourceCollectionUpdated(in NotifyCircularBufferChangedEventArgs<T> e)
    {
        if (!IsFiltered)
        {
            DispatchCollectionChanged(e);
            return;
        }

        switch (e.Action)
        {
            case BufferChangedAction.PopFront:
                if (!_filteredView!.IsEmpty && ReferenceEquals(e.Operand, _filteredView!.Front()))
                {
                    _filteredView.PopFront();
                }
                break;
            case BufferChangedAction.PopBack:
                if (!_filteredView!.IsEmpty && ReferenceEquals(e.Operand, _filteredView!.Back()))
                {
                    _filteredView.PopBack();
                }
                break;
            case BufferChangedAction.PushFront:
                if (PassesFilter(e.Operand))
                {
                    DebugIsFilteredFullCheck();
                    _filteredView!.PushFront(e.Operand);
                }
                break;
            case BufferChangedAction.PushBack:
                if (PassesFilter(e.Operand))
                {
                    DebugIsFilteredFullCheck();
                    _filteredView!.PushBack(e.Operand);
                }
                break;
            case BufferChangedAction.Replace:
                throw new NotSupportedException("Too complicated");
            case BufferChangedAction.Clear:
                _filteredView!.Clear();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // [Conditional("DEBUG")]
    private void DebugIsFilteredFullCheck()
    {
        if (_filteredView!.IsFull)
        {
            throw new InvalidOperationException("The filtered collection had more entries added to it than the capacity.");
        }
    }

    private void OnFilteredCollectionChanged(in NotifyCircularBufferChangedEventArgs<T> e)
    {
        DispatchCollectionChanged(e);
    }

    protected abstract bool PassesFilter(T item);

    public IEnumerator GetEnumerator()
    {
        return (IsFiltered ? _filteredView! : _collection).GetEnumerator();
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public bool Contains(object item)
    {
        return ((IList)(IsFiltered ? _filteredView! : _collection)).Contains(item);
    }

    public IDisposable DeferRefresh()
    {
        return new RefreshDeferred(this);
    }

    private class RefreshDeferred : IDisposable
    {
        private readonly CircularBufferView<T> _view;

        public RefreshDeferred(CircularBufferView<T> view) => _view = view;

        public void Dispose() => _view.Refresh();
    }

    public bool MoveCurrentTo(object item) => throw new NotSupportedException();
    public bool MoveCurrentToFirst() => throw new NotSupportedException();
    public bool MoveCurrentToLast() => throw new NotSupportedException();
    public bool MoveCurrentToNext() => throw new NotSupportedException();
    public bool MoveCurrentToPosition(int position) => throw new NotSupportedException();
    public bool MoveCurrentToPrevious() => throw new NotSupportedException();

    public void Refresh()
    {
        if (IsFiltered)
        {
            _filteredView ??= NewFilteredView();
            
            using (_filteredView.SuppressUpdates())
            {
                _filteredView.Clear();

                foreach (var item in _collection)
                {
                    if ((Filter == null || Filter(item)) && PassesFilter(item))
                    {
                        _filteredView.PushBack(item);
                    }
                }
            }
        }
        
        PropertyChanged?.Invoke(this, EventCache.CountPropertyChangedEventArgs);
        PropertyChanged?.Invoke(this, EventCache.CountPropertyChangedEventArgs);
        CollectionChanged?.Invoke(this, EventCache.CollectionResetEventArgs);
    }

    private ObservableCircularBuffer<T> NewFilteredView()
    {
        var view = new ObservableCircularBuffer<T>(_collection.Capacity);

        view.BufferChanged += OnFilteredCollectionChanged;

        return view;
    }

    private void DispatchCollectionChanged(in NotifyCircularBufferChangedEventArgs<T> e)
    {
        NotifyCollectionChangedEventArgs<T> newArgs;
        
        switch (e.Action)
        {
            case BufferChangedAction.PopFront:
            case BufferChangedAction.PopBack:
                newArgs = NotifyCollectionChangedEventArgs<T>.Remove(e.Operand, e.Index);
                break;
            case BufferChangedAction.PushFront:
            case BufferChangedAction.PushBack:
                newArgs = NotifyCollectionChangedEventArgs<T>.Add(e.Operand, e.Index);
                break;
            case BufferChangedAction.Replace:
                newArgs = NotifyCollectionChangedEventArgs<T>.Replace(e.ReplacedWith!, e.Operand, e.Index);
                break;
            case BufferChangedAction.Clear:
                newArgs = NotifyCollectionChangedEventArgs<T>.Reset();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        CollectionChanged?.Invoke(this, newArgs.ToStandardEventArgs());

        switch (e.Action)
        {
            // add, remove, reset will change the count.
            case BufferChangedAction.PushBack:
            case BufferChangedAction.PushFront:
                PropertyChanged?.Invoke(this, EventCache.CountPropertyChangedEventArgs);
                PropertyChanged?.Invoke(this, EventCache.IsEmptyPropertyChangedEventArgs);
                break;
            case BufferChangedAction.PopBack:
            case BufferChangedAction.PopFront:
                PropertyChanged?.Invoke(this, EventCache.CountPropertyChangedEventArgs);
                PropertyChanged?.Invoke(this, EventCache.IsEmptyPropertyChangedEventArgs);
                break;
            case BufferChangedAction.Clear:
                PropertyChanged?.Invoke(this, EventCache.CountPropertyChangedEventArgs);
                PropertyChanged?.Invoke(this, EventCache.IsEmptyPropertyChangedEventArgs);
                break;
            case BufferChangedAction.Replace:
            default:
                break;
        }
    }

    public bool CanFilter => false;
    public bool CanGroup => false;
    public bool CanSort => false;
    public CultureInfo Culture { get; set; } = null!;
    public object CurrentItem => null!;
    public int CurrentPosition => -1;
    public Predicate<object> Filter { get; set; } = null!;
    public ObservableCollection<GroupDescription> GroupDescriptions => null!;
    public ReadOnlyObservableCollection<object> Groups => null!;
    public bool IsCurrentAfterLast => false;
    public bool IsCurrentBeforeFirst => false;
    public int Count => (IsFiltered ? _filteredView! : _collection).Count;
    public bool IsEmpty => (IsFiltered ? _filteredView! : _collection).IsEmpty;
    public SortDescriptionCollection SortDescriptions => SortDescriptionCollection.Empty;
    public IEnumerable SourceCollection => _collection;

    // No-ops
    event EventHandler? ICollectionView.CurrentChanged
    {
        add { }
        remove { }
    }

    event CurrentChangingEventHandler? ICollectionView.CurrentChanging
    {
        add { }
        remove { }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
}

internal static class EventCache
{
    public static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new("Count");
    public static readonly PropertyChangedEventArgs IsEmptyPropertyChangedEventArgs = new("IsEmpty");
    public static readonly NotifyCollectionChangedEventArgs CollectionResetEventArgs = new(NotifyCollectionChangedAction.Reset);
}