using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RayTech.RayLog.MEL;

/// <summary>
/// An immutable guaranteed O(1) hashless mapping from an enumeration of type <typeparamref name="TKey"/> to
/// values of type <typeparamref name="TValue"/>. <c>EnumMap</c> instances are backed by an array whose size is the
/// highest enum value, plus one.
/// </summary>
/// <typeparam name="TKey">The type of the keys in this table</typeparam>
/// <typeparam name="TValue">The type of the values in this table</typeparam>
public readonly struct EnumMap<TKey, TValue> where TKey : unmanaged, Enum
{
    private readonly (bool isSet, TValue value)[] _entries;

    // the correct methods must be called when relevant or else there will be memory corruption
    // direct casts are impossible in c#; cast styles were benchmarked in RayBot.Benchmarks.EnumCastMarks
    // the JIT should remove dead branches
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static unsafe int KeyToIndex(TKey value)
    {
        if (sizeof(TKey) >= sizeof(int)) return *((int*)&value);
        
        if (typeof(TKey) == typeof(byte)) return *((byte*) &value);
        if (typeof(TKey) == typeof(sbyte)) return *((sbyte*) &value);
        if (typeof(TKey) == typeof(short)) return *((short*) &value);
        if (typeof(TKey) == typeof(ushort)) return *((ushort*) &value);

        return -1; // unknown type
    }
    
    // ReSharper disable once StaticMemberInGenericType
    private static readonly int LargestEnumMember = -1;

    static EnumMap()
    {
        // underlying type
        var underlyingType = Enum.GetUnderlyingType(typeof(TKey));

        if (underlyingType != typeof(byte) && underlyingType != typeof(sbyte) && underlyingType != typeof(short) &&
            underlyingType != typeof(ushort) && underlyingType != typeof(int))
            throw new ArgumentException("Enum type may not be uint/long/ulong", nameof(TKey));

        // max
        foreach (int value in Enum.GetValues(typeof(TKey)))
        {
            if (value < 0)
                throw new ArgumentException("All enum values must be >= 0", nameof(TKey));

            if (value > LargestEnumMember) LargestEnumMember = value;
        }
    }

    public EnumMap()
    {
        _entries = new (bool isSet, TValue value)[LargestEnumMember + 1];
    }

    public EnumMap(IEnumerable<KeyValuePair<TKey, TValue>> entries)
    {
        _entries = new (bool isSet, TValue value)[LargestEnumMember + 1];

        foreach (var (key, value) in entries)
        {
            _entries[KeyToIndex(key)] = (true, value);
        }
    }

    public EnumMap(params KeyValuePair<TKey, TValue>[] entries)
    {
        _entries = new (bool isSet, TValue value)[LargestEnumMember + 1];

        foreach (var (key, value) in entries)
        {
            _entries[KeyToIndex(key)] = (true, value);
        }
    }

    public EnumMap(IEnumerable<(TKey, TValue)> entries)
    {
        _entries = new (bool isSet, TValue value)[LargestEnumMember + 1];

        foreach (var (key, value) in entries)
        {
            _entries[KeyToIndex(key)] = (true, value);
        }
    }

    public bool ContainsKey(TKey key) => _entries[KeyToIndex(key)].isSet;

    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)
    {
        var i = KeyToIndex(key);
        if (i >= 0 && i < _entries.Length)
        {
            var (isSet, newValue) = _entries[i];
            if (isSet)
            {
                value = newValue!;
                return true;
            }
        }

        value = default;
        return false;
    }

    private void Set(TKey key, TValue value)
    {
        var i = KeyToIndex(key);
        if (i >= 0 && i < _entries.Length)
        {
            _entries[i] = (true, value);
        }
    }

    public TValue this[TKey key]
    {
        get => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException(key.ToString());
        set => Set(key, value);
    }
}