using System.Diagnostics.CodeAnalysis;

namespace Maxine.Extensions.KeyValue;

public interface IKeyValues<TKey, TValue> : IDisposable
{
    bool TryGet(in TKey key, [MaybeNullWhen(false)] out TValue value);
    void Upsert(in TKey key, in TValue value);
    bool Delete(in TKey key);
}