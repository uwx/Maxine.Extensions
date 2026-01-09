namespace Maxine.VFS;

public class FileSystemUtils
{
    public static Dictionary<string, byte[]> ReadAllFilesToDictionary(ReadOnlyFileSystem fileSystem, string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var result = new Dictionary<string, byte[]>();

        foreach (var file in fileSystem.EnumerateFiles(path, searchPattern, searchOption))
        {
            result[file] = fileSystem.ReadAllBytes(file);
        }

        return result;
    }
    
    public static async Task<Dictionary<string, byte[]>> ReadAllFilesToDictionaryAsync(ReadOnlyFileSystem fileSystem, string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, byte[]>();

        await foreach (var file in fileSystem.EnumerateFiles(path, searchPattern, searchOption).ToAsyncEnumerable().WithCancellation(cancellationToken))
        {
            result[file] = await fileSystem.ReadAllBytesAsync(file, cancellationToken);
        }

        return result;
    }
    
    public static void WriteAllFilesFromDictionary(BaseFileSystem fileSystem, IReadOnlyDictionary<string, byte[]> files)
    {
        foreach (var (path, data) in files)
        {
            if (fileSystem.Path.GetDirectoryName(path) is { } dir && !fileSystem.DirectoryExists(dir))
            {
                fileSystem.CreateDirectory(dir);
            }
            using var stream = fileSystem.OpenFile(path, FileMode.Create, FileAccess.Write);
            stream.Write(data, 0, data.Length);
        }
    }
    
    public static async Task WriteAllFilesFromDictionaryAsync(BaseFileSystem fileSystem, IReadOnlyDictionary<string, byte[]> files, CancellationToken cancellationToken = default)
    {
        foreach (var (path, data) in files)
        {
            if (fileSystem.Path.GetDirectoryName(path) is { } dir && !fileSystem.DirectoryExists(dir))
            {
                fileSystem.CreateDirectory(dir);
            }
            await using var stream = fileSystem.OpenFile(path, FileMode.Create, FileAccess.Write);
            await stream.WriteAsync(data, cancellationToken);
        }
    }
}