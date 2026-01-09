using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.VFS.Test;

[TestClass]
public class IoFileSystemTests
{
    private string _tempDir = null!;
    private IoFileSystem _fs = null!;

    [TestInitialize]
    public void Setup()
    {
        _tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"IoFileSystemTests_{Guid.NewGuid()}");
        System.IO.Directory.CreateDirectory(_tempDir);
        _fs = new IoFileSystem();
    }

    [TestCleanup]
    public void Cleanup()
    {
        try
        {
            if (System.IO.Directory.Exists(_tempDir))
            {
                System.IO.Directory.Delete(_tempDir, recursive: true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    private string GetPath(params string[] paths)
    {
        return System.IO.Path.Combine(new[] { _tempDir }.Concat(paths).ToArray());
    }

    [TestMethod]
    public void CreateDirectory_CreatesPhysicalDirectory()
    {
        var dirPath = GetPath("testdir");
        _fs.CreateDirectory(dirPath);

        Assert.IsTrue(_fs.DirectoryExists(dirPath));
        Assert.IsTrue(System.IO.Directory.Exists(dirPath));
    }

    [TestMethod]
    public void CreateDirectory_CreatesNestedDirectories()
    {
        var dirPath = GetPath("parent", "child", "grandchild");
        _fs.CreateDirectory(dirPath);

        Assert.IsTrue(_fs.DirectoryExists(GetPath("parent")));
        Assert.IsTrue(_fs.DirectoryExists(GetPath("parent", "child")));
        Assert.IsTrue(_fs.DirectoryExists(dirPath));
    }

    [TestMethod]
    public void DirectoryExists_ReturnsFalseForNonExistent()
    {
        Assert.IsFalse(_fs.DirectoryExists(GetPath("nonexistent")));
    }

    [TestMethod]
    public void DirectoryExists_ReturnsTrueForExisting()
    {
        Assert.IsTrue(_fs.DirectoryExists(_tempDir));
    }

    [TestMethod]
    public void WriteAllText_CreatesPhysicalFile()
    {
        var filePath = GetPath("file.txt");
        _fs.WriteAllText(filePath, "content");

        Assert.IsTrue(_fs.FileExists(filePath));
        Assert.IsTrue(System.IO.File.Exists(filePath));
        Assert.AreEqual("content", System.IO.File.ReadAllText(filePath));
    }

    [TestMethod]
    public void WriteAllText_AndReadAllText_ReturnsContent()
    {
        var filePath = GetPath("file.txt");
        _fs.WriteAllText(filePath, "Hello, World!");
        var content = _fs.ReadAllText(filePath);

        Assert.AreEqual("Hello, World!", content);
    }

    [TestMethod]
    public void WriteAllBytes_AndReadAllBytes_ReturnsBytes()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var filePath = GetPath("data.bin");

        _fs.WriteAllBytes(filePath, data);
        var result = _fs.ReadAllBytes(filePath);

        CollectionAssert.AreEqual(data, result);
    }

    [TestMethod]
    public void FileExists_ReturnsFalseForNonExistent()
    {
        Assert.IsFalse(_fs.FileExists(GetPath("nonexistent.txt")));
    }

    [TestMethod]
    public void DeleteFile_RemovesPhysicalFile()
    {
        var filePath = GetPath("file.txt");
        _fs.WriteAllText(filePath, "content");

        _fs.DeleteFile(filePath);

        Assert.IsFalse(_fs.FileExists(filePath));
        Assert.IsFalse(System.IO.File.Exists(filePath));
    }

    [TestMethod]
    public void DeleteDirectory_RemovesEmptyDirectory()
    {
        var dirPath = GetPath("testdir");
        _fs.CreateDirectory(dirPath);

        _fs.DeleteDirectory(dirPath);

        Assert.IsFalse(_fs.DirectoryExists(dirPath));
    }

    [TestMethod]
    public void DeleteDirectory_Recursive_RemovesDirectoryWithContents()
    {
        var parentPath = GetPath("parent");
        _fs.CreateDirectory(GetPath("parent", "child"));
        _fs.WriteAllText(GetPath("parent", "file.txt"), "content");
        _fs.WriteAllText(GetPath("parent", "child", "file2.txt"), "content2");

        _fs.DeleteDirectory(parentPath, recursive: true);

        Assert.IsFalse(_fs.DirectoryExists(parentPath));
        Assert.IsFalse(_fs.FileExists(GetPath("parent", "file.txt")));
    }

    [TestMethod]
    public void CopyFile_CopiesPhysicalFile()
    {
        var sourcePath = GetPath("source.txt");
        var destPath = GetPath("dest.txt");
        _fs.WriteAllText(sourcePath, "content");

        _fs.CopyFile(sourcePath, destPath);

        Assert.IsTrue(_fs.FileExists(sourcePath));
        Assert.IsTrue(_fs.FileExists(destPath));
        Assert.AreEqual("content", _fs.ReadAllText(destPath));
    }

    [TestMethod]
    public void CopyFile_WithOverwrite_ReplacesExistingFile()
    {
        var sourcePath = GetPath("source.txt");
        var destPath = GetPath("dest.txt");
        _fs.WriteAllText(sourcePath, "new content");
        _fs.WriteAllText(destPath, "old content");

        _fs.CopyFile(sourcePath, destPath, overwrite: true);

        Assert.AreEqual("new content", _fs.ReadAllText(destPath));
    }

    [TestMethod]
    public void MoveFile_MovesPhysicalFile()
    {
        var oldPath = GetPath("old.txt");
        var newPath = GetPath("new.txt");
        _fs.WriteAllText(oldPath, "content");

        _fs.MoveFile(oldPath, newPath);

        Assert.IsFalse(_fs.FileExists(oldPath));
        Assert.IsTrue(_fs.FileExists(newPath));
        Assert.AreEqual("content", _fs.ReadAllText(newPath));
    }

    [TestMethod]
    public void MoveFile_WithOverwrite_ReplacesExistingFile()
    {
        var sourcePath = GetPath("source.txt");
        var destPath = GetPath("dest.txt");
        _fs.WriteAllText(sourcePath, "new content");
        _fs.WriteAllText(destPath, "old content");

        _fs.MoveFile(sourcePath, destPath, overwrite: true);

        Assert.IsFalse(_fs.FileExists(sourcePath));
        Assert.AreEqual("new content", _fs.ReadAllText(destPath));
    }

    [TestMethod]
    public void GetFiles_ReturnsFilesInDirectory()
    {
        var dirPath = GetPath("dir");
        _fs.CreateDirectory(dirPath);
        _fs.WriteAllText(GetPath("dir", "file1.txt"), "1");
        _fs.WriteAllText(GetPath("dir", "file2.txt"), "2");

        var files = _fs.GetFiles(dirPath);

        Assert.AreEqual(2, files.Count);
        Assert.IsTrue(files.Any(f => f.EndsWith("file1.txt")));
        Assert.IsTrue(files.Any(f => f.EndsWith("file2.txt")));
    }

    [TestMethod]
    public void GetFiles_EmptyDirectory_ReturnsEmpty()
    {
        var dirPath = GetPath("emptydir");
        _fs.CreateDirectory(dirPath);

        var files = _fs.GetFiles(dirPath);

        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void GetDirectories_ReturnsSubdirectories()
    {
        var parentPath = GetPath("parent");
        _fs.CreateDirectory(GetPath("parent", "child1"));
        _fs.CreateDirectory(GetPath("parent", "child2"));

        var dirs = _fs.GetDirectories(parentPath);

        Assert.AreEqual(2, dirs.Count);
        Assert.IsTrue(dirs.Any(d => d.EndsWith("child1")));
        Assert.IsTrue(dirs.Any(d => d.EndsWith("child2")));
    }

    [TestMethod]
    public void GetFiles_WithPattern_ReturnsMatchingFiles()
    {
        var dirPath = GetPath("dir");
        _fs.CreateDirectory(dirPath);
        _fs.WriteAllText(GetPath("dir", "file1.txt"), "1");
        _fs.WriteAllText(GetPath("dir", "file2.log"), "2");
        _fs.WriteAllText(GetPath("dir", "file3.txt"), "3");

        var files = _fs.GetFiles(dirPath, "*.txt");

        Assert.AreEqual(2, files.Count);
        Assert.IsTrue(files.All(f => f.EndsWith(".txt")));
    }

    [TestMethod]
    public void GetFiles_Recursive_ReturnsAllFiles()
    {
        var parentPath = GetPath("parent");
        _fs.CreateDirectory(GetPath("parent", "child"));
        _fs.WriteAllText(GetPath("parent", "file1.txt"), "1");
        _fs.WriteAllText(GetPath("parent", "child", "file2.txt"), "2");

        var files = _fs.GetFiles(parentPath, "*", SearchOption.AllDirectories);

        Assert.AreEqual(2, files.Count);
    }

    [TestMethod]
    public void GetDirectories_WithPattern_ReturnsMatchingDirectories()
    {
        var parentPath = GetPath("parent");
        _fs.CreateDirectory(GetPath("parent", "test_dir1"));
        _fs.CreateDirectory(GetPath("parent", "test_dir2"));
        _fs.CreateDirectory(GetPath("parent", "prod_dir"));

        var dirs = _fs.GetDirectories(parentPath, "test*");

        Assert.AreEqual(2, dirs.Count);
        Assert.IsTrue(dirs.All(d => d.Contains("test")));
    }

    [TestMethod]
    public void EnumerateFiles_YieldsFiles()
    {
        var dirPath = GetPath("dir");
        _fs.CreateDirectory(dirPath);
        _fs.WriteAllText(GetPath("dir", "file1.txt"), "1");
        _fs.WriteAllText(GetPath("dir", "file2.txt"), "2");

        var files = _fs.EnumerateFiles(dirPath).ToList();

        Assert.AreEqual(2, files.Count);
    }

    [TestMethod]
    public void EnumerateDirectories_YieldsDirectories()
    {
        var parentPath = GetPath("parent");
        _fs.CreateDirectory(GetPath("parent", "child1"));
        _fs.CreateDirectory(GetPath("parent", "child2"));

        var dirs = _fs.EnumerateDirectories(parentPath).ToList();

        Assert.AreEqual(2, dirs.Count);
    }

    [TestMethod]
    public void OpenFile_CreatesPhysicalFile()
    {
        var filePath = GetPath("test.txt");
        using (var stream = _fs.OpenFile(filePath, FileMode.Create, FileAccess.Write))
        using (var writer = new StreamWriter(stream))
        {
            writer.Write("Hello");
        }

        Assert.IsTrue(_fs.FileExists(filePath));
        Assert.AreEqual("Hello", _fs.ReadAllText(filePath));
    }

    [TestMethod]
    public void OpenFile_AppendMode_AppendsContent()
    {
        var filePath = GetPath("file.txt");
        _fs.WriteAllText(filePath, "Hello");

        using (var stream = _fs.OpenFile(filePath, FileMode.Append, FileAccess.Write))
        using (var writer = new StreamWriter(stream))
        {
            writer.Write(" World");
        }

        Assert.AreEqual("Hello World", _fs.ReadAllText(filePath));
    }

    [TestMethod]
    public void GetAttributes_ReturnsPhysicalFileAttributes()
    {
        var filePath = GetPath("file.txt");
        _fs.WriteAllText(filePath, "content");

        var attributes = _fs.GetAttributes(filePath);

        Assert.AreNotEqual(FileAttributes.Directory, attributes & FileAttributes.Directory);
    }

    [TestMethod]
    public void SetAttributes_UpdatesPhysicalFileAttributes()
    {
        var filePath = GetPath("file.txt");
        _fs.WriteAllText(filePath, "content");

        _fs.SetAttributes(filePath, FileAttributes.ReadOnly);
        var attributes = _fs.GetAttributes(filePath);

        Assert.AreEqual(FileAttributes.ReadOnly, attributes & FileAttributes.ReadOnly);

        // Clean up - remove read-only to allow deletion
        _fs.SetAttributes(filePath, FileAttributes.Normal);
    }

    [TestMethod]
    public void Exists_ReturnsTrueForFile()
    {
        var filePath = GetPath("file.txt");
        _fs.WriteAllText(filePath, "content");

        Assert.IsTrue(_fs.Exists(filePath));
    }

    [TestMethod]
    public void Exists_ReturnsTrueForDirectory()
    {
        var dirPath = GetPath("dir");
        _fs.CreateDirectory(dirPath);

        Assert.IsTrue(_fs.Exists(dirPath));
    }

    [TestMethod]
    public void Exists_ReturnsFalseForNonExistent()
    {
        Assert.IsFalse(_fs.Exists(GetPath("nonexistent")));
    }

    [TestMethod]
    public async Task ReadAllBytesAsync_ReturnsFileContent()
    {
        var data = new byte[] { 10, 20, 30 };
        var filePath = GetPath("data.bin");
        _fs.WriteAllBytes(filePath, data);

        var result = await _fs.ReadAllBytesAsync(filePath);

        CollectionAssert.AreEqual(data, result);
    }

    [TestMethod]
    public async Task ReadAllTextAsync_ReturnsFileContent()
    {
        var filePath = GetPath("file.txt");
        _fs.WriteAllText(filePath, "async text");

        var content = await _fs.ReadAllTextAsync(filePath);

        Assert.AreEqual("async text", content);
    }

    [TestMethod]
    public void WorksWithPhysicalFileSystem()
    {
        // Create file using VFS
        var vfsFile = GetPath("vfs_file.txt");
        _fs.WriteAllText(vfsFile, "VFS content");

        // Verify using physical file system
        Assert.IsTrue(System.IO.File.Exists(vfsFile));
        Assert.AreEqual("VFS content", System.IO.File.ReadAllText(vfsFile));

        // Create file using physical file system
        var physicalFile = GetPath("physical_file.txt");
        System.IO.File.WriteAllText(physicalFile, "Physical content");

        // Verify using VFS
        Assert.IsTrue(_fs.FileExists(physicalFile));
        Assert.AreEqual("Physical content", _fs.ReadAllText(physicalFile));
    }

    [TestMethod]
    public void MultipleOperations_MaintainConsistency()
    {
        // Complex sequence of operations
        _fs.CreateDirectory(GetPath("project", "src"));
        _fs.CreateDirectory(GetPath("project", "tests"));
        _fs.WriteAllText(GetPath("project", "src", "main.cs"), "Main code");
        _fs.WriteAllText(GetPath("project", "tests", "test.cs"), "Test code");
        _fs.CopyFile(GetPath("project", "src", "main.cs"), GetPath("project", "src", "main.backup.cs"));
        _fs.MoveFile(GetPath("project", "tests", "test.cs"), GetPath("project", "src", "test_moved.cs"));

        Assert.IsTrue(_fs.FileExists(GetPath("project", "src", "main.cs")));
        Assert.IsTrue(_fs.FileExists(GetPath("project", "src", "main.backup.cs")));
        Assert.IsTrue(_fs.FileExists(GetPath("project", "src", "test_moved.cs")));
        Assert.IsFalse(_fs.FileExists(GetPath("project", "tests", "test.cs")));
    }
}
