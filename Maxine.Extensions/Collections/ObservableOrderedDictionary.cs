using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Maxine.Extensions.Collections;

public class ObservableOrderedDictionary<TKey, TValue> : ObservableCollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dict;
    private readonly IEqualityComparer<TKey> _comparer;

    public ObservableOrderedDictionary(IEqualityComparer<TKey>? comparer = null)
    {
        _dict = new Dictionary<TKey, TValue>(comparer);
        _comparer = comparer ?? EqualityComparer<TKey>.Default;
    }

    protected override void ClearItems()
    {
        _dict.Clear();
        
        base.ClearItems();
    }

    protected override void RemoveItem(int index)
    {
        var removedItem = this[index];
        RemoveKey(removedItem.Key);
        
        base.RemoveItem(index);
    }

    protected override void InsertItem(int index, KeyValuePair<TKey, TValue> item)
    {
        AddKey(item.Key, item.Value);
        
        base.InsertItem(index, item);
    }

    protected override void SetItem(int index, KeyValuePair<TKey, TValue> item)
    {
        var newKey = item.Key;
        var oldKey = this[index].Key;
        
        if (_comparer.Equals(oldKey, newKey))
        {
            _dict[newKey] = item.Value;
        }
        else
        {
            AddKey(newKey, item.Value);
            RemoveKey(oldKey);
        }

        base.SetItem(index, item);
    }

    private void AddKey(TKey key, TValue value)
    {
        _dict.Add(key, value);
    }

    private void RemoveKey(TKey key)
    {
        _dict.Remove(key);
    }

    // protected override void MoveItem(int oldIndex, int newIndex)
    // {
    //     base.MoveItem(oldIndex, newIndex);
    // }
    public void Add(TKey key, TValue value)
        => Add(KeyValuePair.Create(key, value));

    public bool ContainsKey(TKey key)
        => _dict.ContainsKey(key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        => _dict.TryGetValue(key, out value);

    public bool Remove(TKey key)
    {
        return _dict.TryGetValue(key, out var value) && Remove(KeyValuePair.Create(key, value)); // Slow
    }

    public TValue this[TKey key]
    {
        get => _dict[key];
        set
        {
            // SetItem used here instead of Remove + Add so that it doesn't do two separate collection events
            if (_dict.TryGetValue(key, out var oldValue))
            {
                var index = IndexOf(KeyValuePair.Create(key, oldValue)); // Slow

                SetItem(index, KeyValuePair.Create(key, value));
            }
            else
            {
                Add(key, value);
            }
        }
    }

    void IDictionary.Add(object key, object? value) => Add((TKey) key, (TValue) value!); // IDK what to do about the nullable here

    bool IDictionary.Contains(object key) => ContainsKey((TKey) key);

    void IDictionary.Remove(object key) => Remove((TKey) key);

    IDictionaryEnumerator IDictionary.GetEnumerator() => new DictionaryEnumerator(this);

    private sealed class DictionaryEnumerator : IDictionaryEnumerator
    {
        private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator; // Enumerator over the dictionary.

        internal DictionaryEnumerator(ObservableOrderedDictionary<TKey, TValue> dictionary) => _enumerator = dictionary.GetEnumerator();

        public DictionaryEntry Entry => new(_enumerator.Current.Key, _enumerator.Current.Value);

        public object Key => _enumerator.Current.Key;

        public object? Value => _enumerator.Current.Value;

        public object Current => Entry;

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();
    }

    public bool IsFixedSize => false;

    public bool IsReadOnly => false;

    object? IDictionary.this[object key]
    {
        get => this[(TKey) key];
        set => this[(TKey) key] = (TValue) value!;
    }

    public ICollection<TKey> Keys => _dict.Keys;

    public ICollection<TValue> Values => _dict.Values;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dict.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dict.Values;

    ICollection IDictionary.Keys => _dict.Keys;
    
    ICollection IDictionary.Values => _dict.Values;
}