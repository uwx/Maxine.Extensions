using System.Collections.Immutable;

namespace Maxine.VFS;

public class MountingFileSystem : BaseFileSystem
{
    private BaseFileSystem? _newFileTarget;
    private ImmutableArray<ReadOnlyFileSystem> _fileSystems = ImmutableArray<ReadOnlyFileSystem>.Empty;
    private BaseFileSystem _actualFileSystem;
    
    public MountingFileSystem()
    {
        _actualFileSystem = new NullFileSystem();
    }
    
    public void MountNewFileTarget(BaseFileSystem newFileTarget)
    {
        _newFileTarget = newFileTarget;
        RebuildFileSystem();
    }
    
    public void MountFileSystem(ReadOnlyFileSystem fileSystem)
    {
        _fileSystems = _fileSystems.Add(fileSystem);
        RebuildFileSystem();
    }

    private void RebuildFileSystem()
    {
        _actualFileSystem = _newFileTarget != null
            ? new WritableFallbackFileSystem(_newFileTarget, _fileSystems.Length > 0 ? _fileSystems.AsSpan() : [new NullFileSystem()])
            : new NullFileSystem();
    }

    public override ICollection<string> GetFiles(string path)
    {
        return _actualFileSystem.GetFiles(path);
    }

    public override ICollection<string> GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return _actualFileSystem.GetFiles(path, searchPattern, searchOption);
    }

    public override IEnumerable<string> EnumerateFiles(string path)
    {
        return _actualFileSystem.EnumerateFiles(path);
    }

    public override IEnumerable<string> EnumerateDirectories(string path)
    {
        return _actualFileSystem.EnumerateDirectories(path);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern,
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return _actualFileSystem.EnumerateFiles(path, searchPattern, searchOption);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern,
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return _actualFileSystem.EnumerateDirectories(path, searchPattern, searchOption);
    }

    public override bool FileExists(string path)
    {
        return _actualFileSystem.FileExists(path);
    }

    public override bool DirectoryExists(string path)
    {
        return _actualFileSystem.DirectoryExists(path);
    }

    public override FileAttributes GetAttributes(string file)
    {
        return _actualFileSystem.GetAttributes(file);
    }

    public override Stream OpenFile(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        return _actualFileSystem.OpenFile(path, mode, access);
    }

    public override ValueTask<Stream> OpenFileAsync(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        return _actualFileSystem.OpenFileAsync(path, mode, access);
    }

    public override void CreateDirectory(string path)
    {
        _actualFileSystem.CreateDirectory(path);
    }

    public override void DeleteFile(string path)
    {
        _actualFileSystem.DeleteFile(path);
    }

    public override void DeleteDirectory(string path, bool recursive = false)
    {
        _actualFileSystem.DeleteDirectory(path, recursive);
    }

    public override void CopyFile(string from, string to, bool overwrite = false)
    {
        _actualFileSystem.CopyFile(from, to, overwrite);
    }

    public override void MoveFile(string from, string to, bool overwrite = false)
    {
        _actualFileSystem.MoveFile(from, to, overwrite);
    }

    public override void SetAttributes(string file, FileAttributes attributes)
    {
        _actualFileSystem.SetAttributes(file, attributes);
    }
}