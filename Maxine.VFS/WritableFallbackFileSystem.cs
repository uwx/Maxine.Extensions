namespace Maxine.VFS;

public class WritableFallbackFileSystem : BaseFileSystem, IFallbackFileSystem
{
    private readonly bool _leaveOpen;
    private readonly ReadOnlyFileSystem[] _fileSystems;
    private readonly BaseFileSystem[] _writeFileSystems;
    private readonly BaseFileSystem _newFileTarget;

    ReadOnlyFileSystem[] IFallbackFileSystem.FileSystems => _fileSystems;

    public WritableFallbackFileSystem(bool leaveOpen, BaseFileSystem newFileTarget, params ReadOnlyFileSystem[] fileSystems) : this(newFileTarget, fileSystems)
    {
        _leaveOpen = leaveOpen;
    }

    public WritableFallbackFileSystem(BaseFileSystem newFileTarget, params ReadOnlyFileSystem[] fileSystems)
    {
        _fileSystems = fileSystems.SelectMany(e => e is IFallbackFileSystem ffs ? ffs.FileSystems : [e]).ToArray();
        _writeFileSystems = _fileSystems.OfType<BaseFileSystem>().ToArray();
        _newFileTarget = newFileTarget;
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

    public override Stream OpenFile(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        if (mode == FileMode.CreateNew || (mode == FileMode.Create && !FileExists(path)))
        {
            return _newFileTarget.OpenFile(path, mode, access);
        }

        foreach (var fs in _writeFileSystems)
        {
            if (fs.FileExists(path))
            {
                return fs.OpenFile(path, mode, access);
            }
        }

        return _newFileTarget.OpenFile(path, mode, access);
    }

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

    public override void CreateDirectory(string path)
    {
        _newFileTarget.CreateDirectory(path);
    }

    public override void DeleteFile(string path)
    {
        foreach (var fs in _writeFileSystems)
        {
            if (fs.FileExists(path))
            {
                fs.DeleteFile(path);
            }
        }

        throw new FileNotFoundException();
    }

    public override void DeleteDirectory(string path, bool recursive = false)
    {
        foreach (var fs in _writeFileSystems)
        {
            if (fs.DirectoryExists(path))
            {
                fs.DeleteDirectory(path, recursive);
            }
        }

        throw new DirectoryNotFoundException();
    }

    public override void CopyFile(string from, string to, bool overwrite = false)
    {
        // fast-case: source and destination exist in the same FS
        foreach (var fs in _writeFileSystems)
        {
            if (fs.FileExists(from) && fs.FileExists(to))
            {
                fs.CopyFile(from, to, overwrite);
                return;
            }
        }
        
        // if paths are the same, no-op
        if (Path.PathEquals(from, to))
        {
            return;
        }

        // otherwise, read from source and write to destination
        using var sourceStream = OpenRead(from);
        using var destStream = OpenFile(to, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write);
        sourceStream.CopyTo(destStream);
    }

    public override void MoveFile(string from, string to, bool overwrite = false)
    {
        // fast-case: source and destination exist in the same FS
        foreach (var fs in _writeFileSystems)
        {
            if (fs.FileExists(from) && fs.FileExists(to))
            {
                fs.MoveFile(from, to, overwrite);
                return;
            }
        }
        
        // if paths are the same, no-op
        if (Path.PathEquals(from, to))
        {
            return;
        }

        // otherwise, copy and delete
        CopyFile(from, to, overwrite);
        DeleteFile(from);
    }

    public override void SetAttributes(string file, FileAttributes attributes)
    {
        foreach (var fs in _writeFileSystems)
        {
            if (fs.FileExists(file))
            {
                fs.SetAttributes(file, attributes);
                return;
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