namespace Maxine.VFS;

public sealed class IoFileSystem : BaseFileSystem
{
    public override ICollection<string> GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) => Directory.GetFiles(path, searchPattern, searchOption);
    public override ICollection<string> GetDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) => Directory.GetDirectories(path, searchPattern, searchOption);

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) => Directory.EnumerateFiles(path, searchPattern, searchOption);
    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) => Directory.EnumerateDirectories(path, searchPattern, searchOption);

    public override bool FileExists(string path) => File.Exists(path);
    public override bool DirectoryExists(string path) => Directory.Exists(path);

    public override Stream OpenFile(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite) => File.Open(path, mode, access);

    public override void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public override void DeleteFile(string path) => File.Delete(path);

    public override void DeleteDirectory(string path, bool recursive = false) => Directory.Delete(path, recursive);
    public override void CopyFile(string from, string to, bool overwrite = false) => File.Copy(from, to, overwrite);
    public override void MoveFile(string from, string to, bool overwrite = false) => File.Move(from, to, overwrite);

    public override FileAttributes GetAttributes(string file) => File.GetAttributes(file);
    public override void SetAttributes(string file, FileAttributes attributes) => File.SetAttributes(file, attributes);
}