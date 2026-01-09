using System.IO.Compression;

namespace Maxine.VFS;

public class ZipFileSystem : ReadOnlyFileSystem
{
    public sealed override IPath Path => IPath.MemoryPath.Instance;

    private readonly Dictionary<string, MemoryDirectory> _directories = new(StringComparer.OrdinalIgnoreCase)
    {
        [""] = new MemoryDirectory("")
    };

    private readonly Dictionary<string, FileInZip> _files = new(StringComparer.OrdinalIgnoreCase);
    private readonly ZipArchive _zipArchive;

    public ZipFileSystem(ZipArchive zipArchive)
    {
        _zipArchive = zipArchive;
        foreach (var entry in zipArchive.Entries)
        {
            var fullPath = Path.GetFullPath(entry.FullName);
            var directoryPath = Path.GetDirectoryName(fullPath) ?? "";
            var fileEntry = new FileInZip(entry);
            _files[fullPath] = fileEntry;

            // Ensure parent directories exist - process from root to leaf
            if (!string.IsNullOrEmpty(directoryPath) && !_directories.ContainsKey(directoryPath))
            {
                // Build list of missing directories from root to leaf
                var missingDirs = new Stack<string>();
                var currentPath = directoryPath;
                while (!string.IsNullOrEmpty(currentPath) && !_directories.ContainsKey(currentPath))
                {
                    missingDirs.Push(currentPath);
                    currentPath = Path.GetDirectoryName(currentPath) ?? "";
                }

                // Create directories from root to leaf
                while (missingDirs.Count > 0)
                {
                    var dirPath = missingDirs.Pop();
                    var parentPath = Path.GetDirectoryName(dirPath) ?? "";
                    var dirEntry = new MemoryDirectory(dirPath);

                    if (_directories.TryGetValue(parentPath, out var parentDirEntry))
                    {
                        parentDirEntry.Add(dirEntry);
                    }

                    _directories[dirPath] = dirEntry;
                }
            }

            // Add file to its parent directory
            var value = _directories[directoryPath];
            value.Add(fileEntry);
        }
    }

    public override ICollection<string> GetFiles(string path)
    {
        return EnumerateFiles(path).ToArray();
    }

    public override ICollection<string> GetDirectories(string path)
    {
        return EnumerateDirectories(path).ToArray();
    }

    public override IEnumerable<string> EnumerateFiles(string path)
    {
        return MemoryVfsHelpers.EnumerateFiles(Path, _directories, path);
    }

    public override IEnumerable<string> EnumerateDirectories(string path)
    {
        return MemoryVfsHelpers.EnumerateDirectories(Path, _directories, path);
    }

    public override ICollection<string> GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateChildren(path, searchPattern, static e => !e.IsDirectory, searchOption).ToArray();
    }

    public override ICollection<string> GetDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateChildren(path, searchPattern, static e => e.IsDirectory, searchOption).ToArray();
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateChildren(path, searchPattern, static e => !e.IsDirectory, searchOption);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateChildren(path, searchPattern, static e => e.IsDirectory, searchOption);
    }

    private IEnumerable<string> EnumerateChildren(string path, string searchPattern, Func<BaseFsEntry, bool> predicate, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return MemoryVfsHelpers.EnumerateChildren(Path, _directories, path, searchPattern, predicate, searchOption);
    }

    public override bool FileExists(string path) => _files.ContainsKey(Path.GetFullPath(path));

    public override bool DirectoryExists(string path) => _directories.ContainsKey(Path.GetFullPath(path));

    public override Stream OpenRead(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (_files.TryGetValue(fullPath, out var fileEntry))
        {
            return fileEntry.Open();
        }
        throw new FileNotFoundException($"File not found in zip: {path}");
    }

    public override async ValueTask<Stream> OpenReadAsync(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (_files.TryGetValue(fullPath, out var fileEntry))
        {
            return await fileEntry.OpenAsync();
        }
        throw new FileNotFoundException($"File not found in zip: {path}");
    }

    public override FileAttributes GetAttributes(string file)
    {
        return FileAttributes.ReadOnly;
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        base.Dispose();
        _zipArchive.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await base.DisposeAsync();
        await _zipArchive.DisposeAsync();
    }
}

internal sealed class FileInZip(ZipArchiveEntry entry) : BaseFsEntry(entry.FullName)
{
    public override bool IsDirectory => false;

    public Stream Open()
    {
        return entry.Open();
    }

    public async ValueTask<Stream> OpenAsync()
    {
        return await entry.OpenAsync();
    }
}