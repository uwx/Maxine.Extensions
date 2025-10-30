using FlatSharp;

namespace Maxine.Extensions.KeyValue;

public interface IKeyValueDb : IKeyValueDbInternal, IDisposable
{
    TableBuilder Table(string name) => new(name, this);
}

public interface IKeyValueDbInternal
{
    IKeyValues<TKey, TValue> MakeTableInternal<TKey, TValue>(string name, ProviderType keyProviderType, ProviderType valueProviderType);
}

public enum ProviderType
{
    Default, Msgpack, FlatBuffers
}

public readonly record struct TableBuilder
{
    private readonly string _name;
    private readonly IKeyValueDbInternal _kvDbInternal;
    internal TableBuilder(string name, IKeyValueDbInternal kvDbInternal)
    {
        _name = name;
        _kvDbInternal = kvDbInternal;
    }

    public ValueTableBuilder<TKey> WithKey<TKey>() => new(_name, _kvDbInternal, ProviderType.Default);
    public ValueTableBuilder<TKey> WithMsgpackKey<TKey>() => new(_name, _kvDbInternal, ProviderType.Msgpack);
    public ValueTableBuilder<TKey> WithFlatbuffersKey<TKey>()
        where TKey : class, IFlatBufferSerializable<TKey>
        => new(_name, _kvDbInternal, ProviderType.FlatBuffers);

    public KeyTableBuilder<TValue> WithValue<TValue>() => new(_name, _kvDbInternal, ProviderType.Default);
    public KeyTableBuilder<TValue> WithMsgpackValue<TValue>() => new(_name, _kvDbInternal, ProviderType.Msgpack);
    public KeyTableBuilder<TValue> WithFlatbuffersValue<TValue>()
        where TValue : class, IFlatBufferSerializable<TValue>
        => new(_name, _kvDbInternal, ProviderType.FlatBuffers);

}

public readonly record struct ValueTableBuilder<TKey>
{
    private readonly string _name;
    private readonly IKeyValueDbInternal _kvDbInternal;
    private readonly ProviderType _keyProviderType;

    internal ValueTableBuilder(string name, IKeyValueDbInternal kvDbInternal, ProviderType keyProviderType)
    {
        _name = name;
        _kvDbInternal = kvDbInternal;
        _keyProviderType = keyProviderType;
    }

    public IKeyValues<TKey, TValue> WithValue<TValue>() => _kvDbInternal.MakeTableInternal<TKey, TValue>(_name, _keyProviderType, ProviderType.Default);
    public IKeyValues<TKey, TValue> WithMsgpackValue<TValue>() => _kvDbInternal.MakeTableInternal<TKey, TValue>(_name, _keyProviderType, ProviderType.Msgpack);
    public IKeyValues<TKey, TValue> WithFlatbuffersValue<TValue>()
        where TValue : class, IFlatBufferSerializable<TValue>
        => _kvDbInternal.MakeTableInternal<TKey, TValue>(_name, _keyProviderType, ProviderType.FlatBuffers);
}

public readonly record struct KeyTableBuilder<TValue>
{
    private readonly string _name;
    private readonly IKeyValueDbInternal _kvDbInternal;
    private readonly ProviderType _valueProviderType;

    internal KeyTableBuilder(string name, IKeyValueDbInternal kvDbInternal, ProviderType valueProviderType)
    {
        _name = name;
        _kvDbInternal = kvDbInternal;
        _valueProviderType = valueProviderType;
    }
    
    public IKeyValues<TKey, TValue> WithKey<TKey>() => _kvDbInternal.MakeTableInternal<TKey, TValue>(_name, ProviderType.Default, _valueProviderType);
    public IKeyValues<TKey, TValue> WithMsgpackKey<TKey>() => _kvDbInternal.MakeTableInternal<TKey, TValue>(_name, ProviderType.Msgpack, _valueProviderType);
    public IKeyValues<TKey, TValue> WithFlatbuffersKey<TKey>()
        where TKey : class, IFlatBufferSerializable<TKey>
        => _kvDbInternal.MakeTableInternal<TKey, TValue>(_name, ProviderType.FlatBuffers, _valueProviderType);
}
