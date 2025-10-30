using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Maxine.Extensions.Collections;

/// <summary>
/// An immutable guaranteed O(1) hashless mapping from an enumeration of type <typeparamref name="TKey"/> to
/// values of type <typeparamref name="TValue"/>. <c>EnumMap</c> instances are backed by an array whose size is the
/// highest enum value, plus one.
/// </summary>
/// <typeparam name="TKey">The type of the keys in this table</typeparam>
/// <typeparam name="TValue">The type of the values in this table</typeparam>
public readonly struct EnumMap<TKey, TValue> where TKey : unmanaged, Enum
{
    private readonly struct ValueContainer
    {
        public readonly bool HasValue;
        public readonly TValue Value;

        public ValueContainer(TValue value)
        {
            HasValue = true;
            Value = value;
        }
    }
    
    private static int KeyToIndex(TKey key)
    {
        var index = KeyToIndexInternal(key);
        if (index >= 0 && index <= LargestEnumMember)
        {
            return index;
        }

        // the correct methods must be called when relevant or else there will be memory corruption
        // direct casts are impossible in c#; cast styles were benchmarked in RayBot.Benchmarks.EnumCastMarks
        // the JIT should remove dead branches
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static unsafe int KeyToIndexInternal(TKey key)
        {
            if (sizeof(TKey) >= sizeof(int)) return *(int*) &key;
        
            if (typeof(TKey) == typeof(byte)) return *(byte*) &key;
            if (typeof(TKey) == typeof(sbyte)) return *(sbyte*) &key;
            if (typeof(TKey) == typeof(short)) return *(short*) &key;
            if (typeof(TKey) == typeof(ushort)) return *(ushort*) &key;

            return -1; // unknown type
        }

        [MethodImpl(MethodImplOptions.NoInlining), DoesNotReturn, StackTraceHidden]
        static int ThrowOutOfRangeException(TKey key)
            => throw new ArgumentOutOfRangeException(nameof(key), key, $"Value not in range [0,{LargestEnumMember}] inclusive");

        return ThrowOutOfRangeException(key);
    }

    private readonly ValueContainer[] _entries;
    
    // ReSharper disable once StaticMemberInGenericType
    private static readonly int LargestEnumMember = -1;

    static EnumMap()
    {
        // underlying type
        var underlyingType = Enum.GetUnderlyingType(typeof(TKey));

        if (underlyingType != typeof(byte) &&
            underlyingType != typeof(sbyte) &&
            underlyingType != typeof(short) &&
            underlyingType != typeof(ushort) &&
            underlyingType != typeof(int))
            throw new ArgumentException("Enum type may not be uint/long/ulong", nameof(TKey));

        // max
        foreach (int value in Enum.GetValues(typeof(TKey)))
        {
            if (value < 0)
                throw new ArgumentException("All enum values must be nonnegative", nameof(TKey));

            if (value > LargestEnumMember) LargestEnumMember = value;
        }
    }

    public EnumMap()
    {
        _entries = new ValueContainer[LargestEnumMember + 1];
    }

    public EnumMap(IEnumerable<KeyValuePair<TKey, TValue>> entries)
    {
        _entries = new ValueContainer[LargestEnumMember + 1];

        foreach (var (key, value) in entries)
        {
            SetValue(key, value);
        }
    }

    public EnumMap(IEnumerable<(TKey, TValue)> entries)
    {
        _entries = new ValueContainer[LargestEnumMember + 1];

        foreach (var (key, value) in entries)
        {
            SetValue(key, value);
        }
    }

    public EnumMap(params (TKey, TValue)[] entries)
    {
        _entries = new ValueContainer[LargestEnumMember + 1];

        foreach (var (key, value) in entries)
        {
            SetValue(key, value);
        }
    }

    public bool ContainsKey(TKey key) => _entries[KeyToIndex(key)].HasValue;

    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)
    {
        if (_entries[KeyToIndex(key)] is { HasValue: true } entry)
        {
            value = entry.Value!;
            return true;
        }

        value = default;
        return false;
    }

    private void SetValue(TKey key, TValue value)
    {
        _entries[KeyToIndex(key)] = new ValueContainer(value);
    }

    public TValue this[TKey key]
    {
        get => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
        set => SetValue(key, value);
    }
}