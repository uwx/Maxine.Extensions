using Maxine.Extensions.SpanExtensions;

namespace Maxine.TU.VFS;

public sealed class RelativeFileSystem : BaseFileSystem
{
    private readonly BaseFileSystem _innerFileSystem;
    private readonly string _rootDirectory;
    
    public override IPath Path => _innerFileSystem.Path;

    public RelativeFileSystem(string rootDirectory) : this(new IoFileSystem(), rootDirectory)
    {
    }
    
    public RelativeFileSystem(BaseFileSystem innerFileSystem, string rootDirectory)
    {
        _innerFileSystem = innerFileSystem;
        _rootDirectory = Path.GetFullPath(rootDirectory);
    }

    private string ResolvePath(string path)
    {
        if (path is "" or "/" or "\\") return _rootDirectory;

        var fullPath = Path.GetFullPath(path);
        if (fullPath.StartsWith(_rootDirectory)) return path;
        return Path.Combine(_rootDirectory, path);
    }

    private string StripPrefix(string path)
    {
        if (ResolvePath(path).StartsWith(_rootDirectory))
        {
            var span = path.AsSpan();
            span = span[_rootDirectory.Length..];

            if (span.StartsWithAnySequence(['\\'], ['/'], [Path.DirectorySeparatorChar]))
            {
                span = span[1..];
            }

            return new string(span);
        }

        return path;
    }

    public override ICollection<string> GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => EnumerateFiles(path, searchPattern, searchOption).ToArray();

    public override ICollection<string> GetDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => EnumerateDirectories(path, searchPattern, searchOption).ToArray();

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => _innerFileSystem.EnumerateFiles(ResolvePath(path), searchPattern, searchOption)
            .Select(StripPrefix)
            .ToArray();

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => _innerFileSystem.EnumerateDirectories(ResolvePath(path), searchPattern, searchOption)
            .Select(StripPrefix)
            .ToArray();

    public override bool FileExists(string path) => _innerFileSystem.Exists(ResolvePath(path));
    public override bool DirectoryExists(string path) => _innerFileSystem.Exists(ResolvePath(path));

    public override Stream OpenFile(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite) => _innerFileSystem.OpenFile(ResolvePath(path), mode, access);

    public override void CreateDirectory(string path) => _innerFileSystem.CreateDirectory(ResolvePath(path));

    public override void DeleteFile(string path) => _innerFileSystem.DeleteFile(ResolvePath(path));

    public override void DeleteDirectory(string path, bool recursive = false) => _innerFileSystem.DeleteDirectory(ResolvePath(path), recursive);
    public override void CopyFile(string from, string to, bool overwrite = false) => _innerFileSystem.CopyFile(ResolvePath(from), ResolvePath(to), overwrite);
    public override void MoveFile(string from, string to, bool overwrite = false) => _innerFileSystem.MoveFile(ResolvePath(from), ResolvePath(to), overwrite);

    public override FileAttributes GetAttributes(string file) => _innerFileSystem.GetAttributes(ResolvePath(file));
    public override void SetAttributes(string file, FileAttributes attributes) => _innerFileSystem.SetAttributes(ResolvePath(file), attributes);
}

public sealed class RelativeReadOnlyFileSystem : ReadOnlyFileSystem
{
    private readonly ReadOnlyFileSystem _innerFileSystem;
    private readonly string _rootDirectory;
    
    public override IPath Path => _innerFileSystem.Path;

    public RelativeReadOnlyFileSystem(string rootDirectory) : this(new IoFileSystem(), rootDirectory)
    {
    }
    
    public RelativeReadOnlyFileSystem(ReadOnlyFileSystem innerFileSystem, string rootDirectory)
    {
        _innerFileSystem = innerFileSystem;
        _rootDirectory = Path.GetFullPath(rootDirectory);
    }

    private string ResolvePath(string path)
    {
        if (path is "" or "/" or "\\") return _rootDirectory;

        var fullPath = Path.GetFullPath(path);
        if (fullPath.StartsWith(_rootDirectory)) return path;
        return Path.Combine(_rootDirectory, path);
    }

    private string StripPrefix(string path)
    {
        if (ResolvePath(path).StartsWith(_rootDirectory))
        {
            var span = path.AsSpan();
            span = span[_rootDirectory.Length..];

            if (span.StartsWithAnySequence(['\\'], ['/'], [Path.DirectorySeparatorChar]))
            {
                span = span[1..];
            }

            return new string(span);
        }

        return path;
    }

    public override ICollection<string> GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => EnumerateFiles(path, searchPattern, searchOption).ToArray();

    public override ICollection<string> GetDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => EnumerateDirectories(path, searchPattern, searchOption).ToArray();

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => _innerFileSystem.EnumerateFiles(ResolvePath(path), searchPattern, searchOption)
            .Select(StripPrefix)
            .ToArray();

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => _innerFileSystem.EnumerateDirectories(ResolvePath(path), searchPattern, searchOption)
            .Select(StripPrefix)
            .ToArray();

    public override bool FileExists(string path) => _innerFileSystem.Exists(ResolvePath(path));
    public override bool DirectoryExists(string path) => _innerFileSystem.Exists(ResolvePath(path));

    public override Stream OpenRead(string path) => _innerFileSystem.OpenRead(ResolvePath(path));

    public override FileAttributes GetAttributes(string file) => _innerFileSystem.GetAttributes(ResolvePath(file));
}