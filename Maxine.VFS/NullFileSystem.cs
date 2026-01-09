namespace Maxine.VFS;

public sealed class NullFileSystem : BaseFileSystem
{
    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return [];
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return [];
    }

    public override bool FileExists(string path)
    {
        return false;
    }

    public override bool DirectoryExists(string path)
    {
        return false;
    }

    public override FileAttributes GetAttributes(string file)
    {
        throw new FileNotFoundException();
    }

    public override Stream OpenFile(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        throw new FileNotFoundException();
    }

    public override void CreateDirectory(string path)
    {
        throw new DirectoryNotFoundException();
    }

    public override void DeleteFile(string path)
    {
        throw new DirectoryNotFoundException();
    }

    public override void DeleteDirectory(string path, bool recursive = false)
    {
        throw new DirectoryNotFoundException();
    }

    public override void CopyFile(string from, string to, bool overwrite = false)
    {
        throw new FileNotFoundException();
    }

    public override void MoveFile(string from, string to, bool overwrite = false)
    {
        throw new FileNotFoundException();
    }

    public override void SetAttributes(string file, FileAttributes attributes)
    {
        throw new FileNotFoundException();
    }
}