namespace Maxine.VFS.Test;

[TestClass]
public class RelativeFileSystemTests
{
    [TestMethod]
    public void FileExists_ShouldResolveRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root/subdir");
        innerFs.WriteAllText("root/subdir/file.txt", "content");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        Assert.IsTrue(relativeFs.FileExists("subdir/file.txt"));
        Assert.IsFalse(relativeFs.FileExists("root/subdir/file.txt"));
    }

    [TestMethod]
    public void DirectoryExists_ShouldResolveRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root/subdir");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        Assert.IsTrue(relativeFs.DirectoryExists("subdir"));
        Assert.IsTrue(relativeFs.DirectoryExists(""));
    }

    [TestMethod]
    public void WriteAllText_ShouldWriteToRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        relativeFs.WriteAllText("file.txt", "content");
        
        Assert.IsTrue(innerFs.FileExists("root/file.txt"));
        Assert.AreEqual("content", innerFs.ReadAllText("root/file.txt"));
    }

    [TestMethod]
    public void ReadAllText_ShouldReadFromRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        innerFs.WriteAllText("root/file.txt", "content");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        var content = relativeFs.ReadAllText("file.txt");
        Assert.AreEqual("content", content);
    }

    [TestMethod]
    public void CreateDirectory_ShouldCreateInRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        relativeFs.CreateDirectory("subdir/nested");
        
        Assert.IsTrue(innerFs.DirectoryExists("root/subdir/nested"));
    }

    [TestMethod]
    public void DeleteFile_ShouldDeleteFromRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        innerFs.WriteAllText("root/file.txt", "content");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        relativeFs.DeleteFile("file.txt");
        
        Assert.IsFalse(innerFs.FileExists("root/file.txt"));
    }

    [TestMethod]
    public void DeleteDirectory_ShouldDeleteFromRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root/subdir");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        relativeFs.DeleteDirectory("subdir");
        
        Assert.IsFalse(innerFs.DirectoryExists("root/subdir"));
    }

    [TestMethod]
    public void CopyFile_ShouldCopyWithinRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        innerFs.WriteAllText("root/source.txt", "content");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        relativeFs.CopyFile("source.txt", "dest.txt");
        
        Assert.IsTrue(innerFs.FileExists("root/source.txt"));
        Assert.IsTrue(innerFs.FileExists("root/dest.txt"));
        Assert.AreEqual("content", innerFs.ReadAllText("root/dest.txt"));
    }

    [TestMethod]
    public void MoveFile_ShouldMoveWithinRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        innerFs.WriteAllText("root/source.txt", "content");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        relativeFs.MoveFile("source.txt", "dest.txt");
        
        Assert.IsFalse(innerFs.FileExists("root/source.txt"));
        Assert.IsTrue(innerFs.FileExists("root/dest.txt"));
        Assert.AreEqual("content", innerFs.ReadAllText("root/dest.txt"));
    }

    [TestMethod]
    public void EnumerateFiles_ShouldReturnRelativePaths()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root/subdir");
        innerFs.WriteAllText("root/subdir/file1.txt", "content1");
        innerFs.WriteAllText("root/subdir/file2.txt", "content2");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        var files = relativeFs.EnumerateFiles("subdir").ToList();
        
        Assert.AreEqual(2, files.Count);
        Assert.IsTrue(files.All(f => !f.StartsWith("root")));
    }

    [TestMethod]
    public void EnumerateDirectories_ShouldReturnRelativePaths()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root/dir1");
        innerFs.CreateDirectory("root/dir2");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        var dirs = relativeFs.EnumerateDirectories("").ToList();
        
        Assert.AreEqual(2, dirs.Count);
        Assert.IsTrue(dirs.All(d => !d.StartsWith("root")));
    }

    [TestMethod]
    public void GetFiles_ShouldReturnRelativePaths()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root/subdir");
        innerFs.WriteAllText("root/subdir/file1.txt", "content1");
        innerFs.WriteAllText("root/subdir/file2.txt", "content2");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        var files = relativeFs.GetFiles("subdir");
        
        Assert.AreEqual(2, files.Count);
    }

    [TestMethod]
    public void GetAttributes_ShouldGetFromRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        innerFs.WriteAllText("root/file.txt", "content");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        var attributes = relativeFs.GetAttributes("file.txt");
        Assert.IsNotNull(attributes);
    }

    [TestMethod]
    public void SetAttributes_ShouldSetInRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        innerFs.WriteAllText("root/file.txt", "content");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        relativeFs.SetAttributes("file.txt", FileAttributes.ReadOnly);
        
        var attributes = innerFs.GetAttributes("root/file.txt");
        Assert.IsTrue((attributes & FileAttributes.ReadOnly) != 0);
    }

    [TestMethod]
    public void OpenFile_ShouldOpenFromRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        innerFs.WriteAllText("root/file.txt", "content");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        using var stream = relativeFs.OpenFile("file.txt", FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        
        Assert.AreEqual("content", content);
    }

    [TestMethod]
    public void RootPath_EmptyOrSlash_ShouldResolveToRootDirectory()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        innerFs.WriteAllText("root/file.txt", "content");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        Assert.IsTrue(relativeFs.DirectoryExists(""));
        Assert.IsTrue(relativeFs.DirectoryExists("/"));
    }

    [TestMethod]
    public void NestedPaths_ShouldWorkCorrectly()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root/a/b/c");
        innerFs.WriteAllText("root/a/b/c/file.txt", "content");
        
        var relativeFs = new RelativeFileSystem(innerFs, "root");
        
        Assert.IsTrue(relativeFs.FileExists("a/b/c/file.txt"));
        Assert.AreEqual("content", relativeFs.ReadAllText("a/b/c/file.txt"));
    }
}

[TestClass]
public class RelativeReadOnlyFileSystemTests
{
    [TestMethod]
    public void FileExists_ShouldResolveRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root/subdir");
        innerFs.WriteAllText("root/subdir/file.txt", "content");
        
        var relativeFs = new RelativeReadOnlyFileSystem(innerFs, "root");
        
        Assert.IsTrue(relativeFs.FileExists("subdir/file.txt"));
    }

    [TestMethod]
    public void DirectoryExists_ShouldResolveRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root/subdir");
        
        var relativeFs = new RelativeReadOnlyFileSystem(innerFs, "root");
        
        Assert.IsTrue(relativeFs.DirectoryExists("subdir"));
    }

    [TestMethod]
    public void ReadAllText_ShouldReadFromRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        innerFs.WriteAllText("root/file.txt", "content");
        
        var relativeFs = new RelativeReadOnlyFileSystem(innerFs, "root");
        
        var content = relativeFs.ReadAllText("file.txt");
        Assert.AreEqual("content", content);
    }

    [TestMethod]
    public void EnumerateFiles_ShouldReturnRelativePaths()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root/subdir");
        innerFs.WriteAllText("root/subdir/file1.txt", "content1");
        innerFs.WriteAllText("root/subdir/file2.txt", "content2");
        
        var relativeFs = new RelativeReadOnlyFileSystem(innerFs, "root");
        
        var files = relativeFs.EnumerateFiles("subdir").ToList();
        
        Assert.AreEqual(2, files.Count);
        Assert.IsTrue(files.All(f => !f.StartsWith("root")));
    }

    [TestMethod]
    public void OpenRead_ShouldOpenFromRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        innerFs.WriteAllText("root/file.txt", "content");
        
        var relativeFs = new RelativeReadOnlyFileSystem(innerFs, "root");
        
        using var stream = relativeFs.OpenRead("file.txt");
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        
        Assert.AreEqual("content", content);
    }

    [TestMethod]
    public async Task ReadAllBytesAsync_ShouldReadFromRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        var data = new byte[] { 1, 2, 3, 4, 5 };
        innerFs.WriteAllBytes("root/file.bin", data);
        
        var relativeFs = new RelativeReadOnlyFileSystem(innerFs, "root");
        
        var readData = await relativeFs.ReadAllBytesAsync("file.bin");
        CollectionAssert.AreEqual(data, readData);
    }

    [TestMethod]
    public void GetAttributes_ShouldGetFromRelativePath()
    {
        var innerFs = new MemoryFileSystem();
        innerFs.CreateDirectory("root");
        innerFs.WriteAllText("root/file.txt", "content");
        
        var relativeFs = new RelativeReadOnlyFileSystem(innerFs, "root");
        
        var attributes = relativeFs.GetAttributes("file.txt");
        Assert.IsNotNull(attributes);
    }
}

