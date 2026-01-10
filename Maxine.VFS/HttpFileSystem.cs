namespace Maxine.VFS;

public class HttpFileSystem : ReadOnlyFileSystem
{
    private readonly HttpClient _client;
    private readonly Func<string, Uri> _uriFromFilePathBuilder;

    private readonly Dictionary<string, MemoryDirectory> _directories = new(StringComparer.OrdinalIgnoreCase)
    {
        [""] = new MemoryDirectory("")
    };

    private readonly Dictionary<string, HttpRemoteFile> _files = new(StringComparer.OrdinalIgnoreCase);
    
    public HttpFileSystem(HttpClient client, Func<string, Uri> uriFromFilePathBuilder)
    {
        _client = client;
        _uriFromFilePathBuilder = uriFromFilePathBuilder;
    }

    public void AddKnownFiles(IEnumerable<string> files)
    {
        foreach (var entry in files)
        {
            var fullPath = Path.GetFullPath(entry);
            var directoryPath = Path.GetDirectoryName(fullPath) ?? "";
            var fileEntry = new HttpRemoteFile(entry, _client, _uriFromFilePathBuilder);
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
    
    public HttpFileSystem(Uri baseUri)
    {
        _client = new HttpClient
        {
            BaseAddress = baseUri
        };
        _uriFromFilePathBuilder = path => new Uri(baseUri, path);
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

    public override bool FileExists(string path)
    {
        var uri = _uriFromFilePathBuilder(path);
        using var request = new HttpRequestMessage(HttpMethod.Head, uri);
        using var response = _client.Send(request);
        return response.IsSuccessStatusCode;
    }

    public override bool DirectoryExists(string path) => _directories.ContainsKey(Path.GetFullPath(path));

    public override Stream OpenRead(string path)
    {
        var uri = _uriFromFilePathBuilder(path);
        var response = _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
    }

    public override async ValueTask<Stream> OpenReadAsync(string path)
    {
        var uri = _uriFromFilePathBuilder(path);
        var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }

    public override FileAttributes GetAttributes(string file)
    {
        return FileAttributes.ReadOnly;
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        base.Dispose();
        _client.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await base.DisposeAsync();
        _client.Dispose();
    }
}

internal sealed class HttpRemoteFile(string fullName, HttpClient client, Func<string, Uri> uriFromFilePathBuilder) : BaseFsEntry(fullName)
{
    public override bool IsDirectory => false;
}