using System.IO.Compression;

namespace Maxine.Extensions.KeyValue;

public static class KeyValueDbs
{
    public static IKeyValueDb GetKeyValueDb(string rootPath, DatabaseProvider provider) => provider switch
    {
        DatabaseProvider.ZoneTree => new ZoneTreeKeyValueDb2(rootPath),
        DatabaseProvider.RocksDb => new RocksKeyValueDb(rootPath),
        _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
    };
}

public enum DatabaseProvider
{
    ZoneTree, RocksDb
}