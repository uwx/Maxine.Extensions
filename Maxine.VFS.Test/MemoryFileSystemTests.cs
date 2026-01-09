using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.VFS.Test;

[TestClass]
public class MemoryFileSystemTests
{
    [TestMethod]
    public void CreateDirectory_CreatesNewDirectory()
    {
        var fs = new MemoryFileSystem();

        fs.CreateDirectory("testdir");

        Assert.IsTrue(fs.DirectoryExists("testdir"));
    }

    [TestMethod]
    public void CreateDirectory_CreatesNestedDirectories()
    {
        var fs = new MemoryFileSystem();

        fs.CreateDirectory("parent/child/grandchild");

        Assert.IsTrue(fs.DirectoryExists("parent"));
        Assert.IsTrue(fs.DirectoryExists("parent/child"));
        Assert.IsTrue(fs.DirectoryExists("parent/child/grandchild"));
    }

    [TestMethod]
    public void DirectoryExists_ReturnsFalseForNonExistentDirectory()
    {
        var fs = new MemoryFileSystem();

        Assert.IsFalse(fs.DirectoryExists("nonexistent"));
    }

    [TestMethod]
    public void DirectoryExists_ReturnsTrueForRootDirectory()
    {
        var fs = new MemoryFileSystem();

        Assert.IsTrue(fs.DirectoryExists(""));
    }

    [TestMethod]
    public void WriteAllText_CreatesFile()
    {
        var fs = new MemoryFileSystem();

        fs.WriteAllText("file.txt", "content");

        Assert.IsTrue(fs.FileExists("file.txt"));
    }

    [TestMethod]
    public void WriteAllText_AndReadAllText_ReturnsContent()
    {
        var fs = new MemoryFileSystem();

        fs.WriteAllText("file.txt", "Hello, World!");
        var content = fs.ReadAllText("file.txt");

        Assert.AreEqual("Hello, World!", content);
    }

    [TestMethod]
    public void WriteAllBytes_AndReadAllBytes_ReturnsBytes()
    {
        var fs = new MemoryFileSystem();
        var data = new byte[] { 1, 2, 3, 4, 5 };

        fs.WriteAllBytes("data.bin", data);
        var result = fs.ReadAllBytes("data.bin");

        CollectionAssert.AreEqual(data, result);
    }

    [TestMethod]
    public void FileExists_ReturnsFalseForNonExistentFile()
    {
        var fs = new MemoryFileSystem();

        Assert.IsFalse(fs.FileExists("nonexistent.txt"));
    }

    [TestMethod]
    public void DeleteFile_RemovesFile()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("file.txt", "content");

        fs.DeleteFile("file.txt");

        Assert.IsFalse(fs.FileExists("file.txt"));
    }

    [TestMethod]
    public void DeleteFile_NonExistentFile_DoesNotThrow()
    {
        var fs = new MemoryFileSystem();

        // MemoryFileSystem doesn't throw for non-existent files
        fs.DeleteFile("nonexistent.txt");
        Assert.IsFalse(fs.FileExists("nonexistent.txt"));
    }

    [TestMethod]
    public void DeleteDirectory_RemovesEmptyDirectory()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("testdir");

        fs.DeleteDirectory("testdir");

        Assert.IsFalse(fs.DirectoryExists("testdir"));
    }

    [TestMethod]
    public void DeleteDirectory_Recursive_RemovesDirectoryWithContents()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("parent/child");
        fs.WriteAllText("parent/file.txt", "content");
        fs.WriteAllText("parent/child/file2.txt", "content2");

        fs.DeleteDirectory("parent", recursive: true);

        Assert.IsFalse(fs.DirectoryExists("parent"));
        Assert.IsFalse(fs.FileExists("parent/file.txt"));
        Assert.IsFalse(fs.FileExists("parent/child/file2.txt"));
    }

    [TestMethod]
    public void CopyFile_CopiesContent()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("source.txt", "content");

        fs.CopyFile("source.txt", "dest.txt");

        Assert.IsTrue(fs.FileExists("source.txt"));
        Assert.IsTrue(fs.FileExists("dest.txt"));
        Assert.AreEqual("content", fs.ReadAllText("dest.txt"));
    }

    [TestMethod]
    public void CopyFile_WithOverwrite_ReplacesExistingFile()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("source.txt", "new content");
        fs.WriteAllText("dest.txt", "old content");

        fs.CopyFile("source.txt", "dest.txt", overwrite: true);

        Assert.AreEqual("new content", fs.ReadAllText("dest.txt"));
    }

    [TestMethod]
    public void MoveFile_MovesFileToNewLocation()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("old.txt", "content");

        fs.MoveFile("old.txt", "new.txt");

        Assert.IsFalse(fs.FileExists("old.txt"));
        Assert.IsTrue(fs.FileExists("new.txt"));
        Assert.AreEqual("content", fs.ReadAllText("new.txt"));
    }

    [TestMethod]
    public void MoveFile_WithOverwrite_ReplacesExistingFile()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("source.txt", "new content");
        fs.WriteAllText("dest.txt", "old content");

        fs.MoveFile("source.txt", "dest.txt", overwrite: true);

        Assert.IsFalse(fs.FileExists("source.txt"));
        Assert.AreEqual("new content", fs.ReadAllText("dest.txt"));
    }

    [TestMethod]
    public void GetFiles_ReturnsFilesInDirectory()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("dir");
        fs.WriteAllText("dir/file1.txt", "1");
        fs.WriteAllText("dir/file2.txt", "2");

        var files = fs.GetFiles("dir");

        Assert.AreEqual(2, files.Count);
        Assert.IsTrue(files.Contains("dir/file1.txt"));
        Assert.IsTrue(files.Contains("dir/file2.txt"));
    }

    [TestMethod]
    public void GetFiles_EmptyDirectory_ReturnsEmpty()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("emptydir");

        var files = fs.GetFiles("emptydir");

        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void GetDirectories_ReturnsSubdirectories()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("parent/child1");
        fs.CreateDirectory("parent/child2");

        var dirs = fs.GetDirectories("parent");

        Assert.AreEqual(2, dirs.Count);
        Assert.IsTrue(dirs.Contains("parent/child1"));
        Assert.IsTrue(dirs.Contains("parent/child2"));
    }

    [TestMethod]
    public void GetFiles_WithPattern_ReturnsMatchingFiles()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("dir");
        fs.WriteAllText("dir/file1.txt", "1");
        fs.WriteAllText("dir/file2.log", "2");
        fs.WriteAllText("dir/file3.txt", "3");

        var files = fs.GetFiles("dir", "*.txt");

        Assert.AreEqual(2, files.Count);
        Assert.IsTrue(files.Contains("dir/file1.txt"));
        Assert.IsTrue(files.Contains("dir/file3.txt"));
    }

    [TestMethod]
    public void GetFiles_Recursive_ReturnsAllFiles()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("parent/child");
        fs.WriteAllText("parent/file1.txt", "1");
        fs.WriteAllText("parent/child/file2.txt", "2");

        var files = fs.GetFiles("parent", "*", SearchOption.AllDirectories);

        Assert.AreEqual(2, files.Count);
        Assert.IsTrue(files.Contains("parent/file1.txt"));
        Assert.IsTrue(files.Contains("parent/child/file2.txt"));
    }

    [TestMethod]
    public void GetDirectories_WithPattern_ReturnsMatchingDirectories()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("parent/test_dir1");
        fs.CreateDirectory("parent/test_dir2");
        fs.CreateDirectory("parent/prod_dir");

        var dirs = fs.GetDirectories("parent", "test*");

        Assert.AreEqual(2, dirs.Count);
        Assert.IsTrue(dirs.Contains("parent/test_dir1"));
        Assert.IsTrue(dirs.Contains("parent/test_dir2"));
    }

    [TestMethod]
    public void EnumerateFiles_YieldsFiles()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("dir");
        fs.WriteAllText("dir/file1.txt", "1");
        fs.WriteAllText("dir/file2.txt", "2");

        var files = fs.EnumerateFiles("dir").ToList();

        Assert.AreEqual(2, files.Count);
    }

    [TestMethod]
    public void EnumerateDirectories_YieldsDirectories()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("parent/child1");
        fs.CreateDirectory("parent/child2");

        var dirs = fs.EnumerateDirectories("parent").ToList();

        Assert.AreEqual(2, dirs.Count);
    }

    [TestMethod]
    public void OpenFile_CreatesNewFile()
    {
        var fs = new MemoryFileSystem();

        using (var stream = fs.OpenFile("test.txt", FileMode.Create, FileAccess.Write))
        using (var writer = new StreamWriter(stream))
        {
            writer.Write("Hello");
        }

        Assert.IsTrue(fs.FileExists("test.txt"));
        Assert.AreEqual("Hello", fs.ReadAllText("test.txt"));
    }

    [TestMethod]
    public void OpenFile_AppendMode_NotSupported()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("file.txt", "Hello");

        // MemoryFileSystem doesn't support FileMode.Append
        // Use OpenOrCreate and seek to end instead
        using (var stream = fs.OpenFile("file.txt", FileMode.OpenOrCreate, FileAccess.Write))
        {
            stream.Seek(0, SeekOrigin.End);
            using var writer = new StreamWriter(stream);
            writer.Write(" World");
        }

        Assert.AreEqual("Hello World", fs.ReadAllText("file.txt"));
    }

    [TestMethod]
    public void OpenRead_ReturnsReadableStream()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("file.txt", "content");

        using var stream = fs.OpenRead("file.txt");
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        Assert.AreEqual("content", content);
    }

    [TestMethod]
    public async Task OpenReadAsync_ReturnsReadableStream()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("file.txt", "async content");

        await using var stream = await fs.OpenReadAsync("file.txt");
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        Assert.AreEqual("async content", content);
    }

    [TestMethod]
    public async Task OpenFileAsync_WorksCorrectly()
    {
        var fs = new MemoryFileSystem();

        await using (var stream = await fs.OpenFileAsync("test.txt", FileMode.Create, FileAccess.Write))
        {
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync("Async write");
        }

        Assert.AreEqual("Async write", fs.ReadAllText("test.txt"));
    }

    [TestMethod]
    public void GetAttributes_ReturnsFileAttributes()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("file.txt", "content");

        var attributes = fs.GetAttributes("file.txt");

        Assert.AreNotEqual(FileAttributes.Directory, attributes & FileAttributes.Directory);
    }

    [TestMethod]
    public void SetAttributes_UpdatesFileAttributes()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("file.txt", "content");

        fs.SetAttributes("file.txt", FileAttributes.ReadOnly);
        var attributes = fs.GetAttributes("file.txt");

        Assert.AreEqual(FileAttributes.ReadOnly, attributes & FileAttributes.ReadOnly);
    }

    [TestMethod]
    public void Exists_ReturnsTrueForFile()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("file.txt", "content");

        Assert.IsTrue(fs.Exists("file.txt"));
    }

    [TestMethod]
    public void Exists_ReturnsTrueForDirectory()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("dir");

        Assert.IsTrue(fs.Exists("dir"));
    }

    [TestMethod]
    public void Exists_ReturnsFalseForNonExistent()
    {
        var fs = new MemoryFileSystem();

        Assert.IsFalse(fs.Exists("nonexistent"));
    }

    [TestMethod]
    public void PathNormalization_BackslashesToForwardSlashes()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("dir");
        fs.WriteAllText("dir/file.txt", "content");

        // Both forward and backslash should work after normalization
        Assert.IsTrue(fs.FileExists("dir/file.txt"));
        Assert.AreEqual("content", fs.ReadAllText("dir\\file.txt"));
    }

    [TestMethod]
    public void CreateDirectory_AlreadyExists_DoesNotThrow()
    {
        var fs = new MemoryFileSystem();
        fs.CreateDirectory("dir");

        // Should not throw
        fs.CreateDirectory("dir");

        Assert.IsTrue(fs.DirectoryExists("dir"));
    }

    [TestMethod]
    public void CopyFile_RequiresParentDirectory()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("source.txt", "content");

        // MemoryFileSystem requires parent directory to exist
        Assert.Throws<DirectoryNotFoundException>(() => fs.CopyFile("source.txt", "nested/dir/dest.txt"));

        // Works when parent exists
        fs.CreateDirectory("nested/dir");
        fs.CopyFile("source.txt", "nested/dir/dest.txt");
        Assert.IsTrue(fs.FileExists("nested/dir/dest.txt"));
        Assert.AreEqual("content", fs.ReadAllText("nested/dir/dest.txt"));
    }

    [TestMethod]
    public void MoveFile_RequiresParentDirectory()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("source.txt", "content");

        // MemoryFileSystem requires parent directory to exist
        Assert.Throws<DirectoryNotFoundException>(() => fs.MoveFile("source.txt", "nested/dir/dest.txt"));

        // Works when parent exists
        fs.CreateDirectory("nested/dir");
        fs.MoveFile("source.txt", "nested/dir/dest.txt");
        Assert.IsFalse(fs.FileExists("source.txt"));
        Assert.IsTrue(fs.FileExists("nested/dir/dest.txt"));
    }

    [TestMethod]
    public void OpenFile_CreateNew_ThrowsIfFileExists()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("file.txt", "existing");

        Assert.Throws<IOException>(() => fs.OpenFile("file.txt", FileMode.CreateNew));
    }

    [TestMethod]
    public void OpenFile_Open_ThrowsIfFileDoesNotExist()
    {
        var fs = new MemoryFileSystem();

        Assert.Throws<FileNotFoundException>(() => fs.OpenFile("nonexistent.txt", FileMode.Open));
    }

    [TestMethod]
    public async Task ReadAllBytesAsync_ReturnsFileContent()
    {
        var fs = new MemoryFileSystem();
        var data = new byte[] { 10, 20, 30 };
        fs.WriteAllBytes("data.bin", data);

        var result = await fs.ReadAllBytesAsync("data.bin");

        CollectionAssert.AreEqual(data, result);
    }

    [TestMethod]
    public async Task ReadAllTextAsync_ReturnsFileContent()
    {
        var fs = new MemoryFileSystem();
        fs.WriteAllText("file.txt", "async text");

        var content = await fs.ReadAllTextAsync("file.txt");

        Assert.AreEqual("async text", content);
    }

    [TestMethod]
    public void MultipleOperations_MaintainConsistency()
    {
        var fs = new MemoryFileSystem();

        // Complex sequence of operations
        fs.CreateDirectory("project/src");
        fs.CreateDirectory("project/tests");
        fs.WriteAllText("project/src/main.cs", "Main code");
        fs.WriteAllText("project/tests/test.cs", "Test code");
        fs.CopyFile("project/src/main.cs", "project/src/main.backup.cs");
        fs.MoveFile("project/tests/test.cs", "project/src/test_moved.cs");

        Assert.IsTrue(fs.FileExists("project/src/main.cs"));
        Assert.IsTrue(fs.FileExists("project/src/main.backup.cs"));
        Assert.IsTrue(fs.FileExists("project/src/test_moved.cs"));
        Assert.IsFalse(fs.FileExists("project/tests/test.cs"));
    }
}
