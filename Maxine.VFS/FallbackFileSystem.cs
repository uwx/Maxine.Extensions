namespace Maxine.VFS;

internal interface IFallbackFileSystem
{
    ReadOnlyFileSystem[] FileSystems { get; }
}

public sealed class FallbackFileSystem : ReadOnlyFileSystem, IFallbackFileSystem
{
    private readonly bool _leaveOpen;
    private readonly ReadOnlyFileSystem[] _fileSystems;

    ReadOnlyFileSystem[] IFallbackFileSystem.FileSystems => _fileSystems;

    public FallbackFileSystem(bool leaveOpen, params ReadOnlyFileSystem[] fileSystems) : this(fileSystems)
    {
        _leaveOpen = leaveOpen;
    }

    public FallbackFileSystem(params ReadOnlyFileSystem[] fileSystems)
    {
        _fileSystems = fileSystems.SelectMany(e => e is IFallbackFileSystem ffs ? ffs.FileSystems : [e]).ToArray();
    }

    public override ICollection<string> GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => EnumerateFiles(path, searchPattern, searchOption).ToArray();

    public override ICollection<string> GetDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => EnumerateDirectories(path, searchPattern, searchOption).ToArray();

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => _fileSystems.SelectMany(e => e.EnumerateFiles(path, searchPattern, searchOption));

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => _fileSystems.SelectMany(e => e.EnumerateDirectories(path, searchPattern, searchOption));

    public override bool FileExists(string path)
        => _fileSystems.Any(e => e.FileExists(path));

    public override bool DirectoryExists(string path)
        => _fileSystems.Any(e => e.DirectoryExists(path));

    public override Stream OpenRead(string path)
    {
        foreach (var fs in _fileSystems)
        {
            if (fs.FileExists(path))
            {
                return fs.OpenRead(path);
            }
        }

        throw new FileNotFoundException();
    }

    public override FileAttributes GetAttributes(string file)
    {
        foreach (var fs in _fileSystems)
        {
            if (fs.Exists(file))
            {
                return fs.GetAttributes(file);
            }
        }

        throw new FileNotFoundException();
    }

    public override void Dispose()
    {
        if (!_leaveOpen)
        {
            foreach (var fs in _fileSystems)
            {
                fs.Dispose();
            }
        }
    }

    public override async ValueTask DisposeAsync()
    {
        if (!_leaveOpen)
        {
            foreach (var fs in _fileSystems)
            {
                await fs.DisposeAsync();
            }
        }
    }
}