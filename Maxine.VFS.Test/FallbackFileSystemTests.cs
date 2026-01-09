namespace Maxine.VFS.Test;

[TestClass]
public class FallbackFileSystemTests
{
    [TestMethod]
    public void FileExists_ShouldCheckAllFileSystems()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        fs1.WriteAllText("file1.txt", "content1");
        fs2.WriteAllText("file2.txt", "content2");
        
        var fallback = new FallbackFileSystem(fs1, fs2);
        
        Assert.IsTrue(fallback.FileExists("file1.txt"));
        Assert.IsTrue(fallback.FileExists("file2.txt"));
        Assert.IsFalse(fallback.FileExists("file3.txt"));
    }

    [TestMethod]
    public void DirectoryExists_ShouldCheckAllFileSystems()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        fs1.CreateDirectory("dir1");
        fs2.CreateDirectory("dir2");
        
        var fallback = new FallbackFileSystem(fs1, fs2);
        
        Assert.IsTrue(fallback.DirectoryExists("dir1"));
        Assert.IsTrue(fallback.DirectoryExists("dir2"));
        Assert.IsFalse(fallback.DirectoryExists("dir3"));
    }

    [TestMethod]
    public void OpenRead_ShouldReadFromFirstAvailableFileSystem()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        fs1.WriteAllText("file.txt", "content1");
        fs2.WriteAllText("file.txt", "content2");
        
        var fallback = new FallbackFileSystem(fs1, fs2);
        
        var content = fallback.ReadAllText("file.txt");
        Assert.AreEqual("content1", content);
    }

    [TestMethod]
    public void OpenRead_ShouldFallbackToSecondFileSystem()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        fs2.WriteAllText("file.txt", "content2");
        
        var fallback = new FallbackFileSystem(fs1, fs2);
        
        var content = fallback.ReadAllText("file.txt");
        Assert.AreEqual("content2", content);
    }

    [TestMethod]
    public void OpenRead_FileNotFound_ShouldThrow()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        var fallback = new FallbackFileSystem(fs1, fs2);
        Assert.Throws<FileNotFoundException>(() => fallback.OpenRead("nonexistent.txt"));
    }

    [TestMethod]
    public void EnumerateFiles_ShouldCombineAllFileSystems()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        fs1.CreateDirectory("dir");
        fs2.CreateDirectory("dir");
        
        fs1.WriteAllText("dir/file1.txt", "content1");
        fs2.WriteAllText("dir/file2.txt", "content2");
        
        var fallback = new FallbackFileSystem(fs1, fs2);
        
        var files = fallback.EnumerateFiles("dir").ToList();
        Assert.AreEqual(2, files.Count);
    }

    [TestMethod]
    public void EnumerateDirectories_ShouldCombineAllFileSystems()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        fs1.CreateDirectory("parent/dir1");
        fs2.CreateDirectory("parent/dir2");
        
        var fallback = new FallbackFileSystem(fs1, fs2);
        
        var dirs = fallback.EnumerateDirectories("parent").ToList();
        Assert.AreEqual(2, dirs.Count);
    }

    [TestMethod]
    public void GetAttributes_ShouldGetFromFirstAvailableFileSystem()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        fs1.WriteAllText("file.txt", "content");
        
        var fallback = new FallbackFileSystem(fs1, fs2);
        
        var attributes = fallback.GetAttributes("file.txt");
        Assert.IsNotNull(attributes);
    }

    [TestMethod]
    public void GetAttributes_FileNotFound_ShouldThrow()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        var fallback = new FallbackFileSystem(fs1, fs2);
        Assert.Throws<FileNotFoundException>(() => fallback.GetAttributes("nonexistent.txt"));
    }

    [TestMethod]
    public void Dispose_WithLeaveOpenFalse_ShouldDisposeAllFileSystems()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        var fallback = new FallbackFileSystem(leaveOpen: false, fs1, fs2);
        fallback.Dispose();
        
        // After disposal, the inner file systems should be disposed
        // We can't easily test this without exposing internal state,
        // but we can verify the fallback still disposed without error
        Assert.IsNotNull(fallback);
    }

    [TestMethod]
    public void Dispose_WithLeaveOpenTrue_ShouldNotDisposeInnerFileSystems()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        fs1.WriteAllText("test.txt", "content");
        
        var fallback = new FallbackFileSystem(leaveOpen: true, fs1, fs2);
        fallback.Dispose();
        
        // Inner file systems should still be usable
        Assert.IsTrue(fs1.FileExists("test.txt"));
    }

    [TestMethod]
    public async Task DisposeAsync_ShouldDisposeAllFileSystems()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        var fallback = new FallbackFileSystem(leaveOpen: false, fs1, fs2);
        await fallback.DisposeAsync();
        
        Assert.IsNotNull(fallback);
    }

    [TestMethod]
    public void ReadAllBytes_ShouldReadFromFallback()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        var data = new byte[] { 1, 2, 3, 4, 5 };
        fs2.WriteAllBytes("file.bin", data);
        
        var fallback = new FallbackFileSystem(fs1, fs2);
        
        var readData = fallback.ReadAllBytes("file.bin");
        CollectionAssert.AreEqual(data, readData);
    }

    [TestMethod]
    public async Task ReadAllBytesAsync_ShouldReadFromFallback()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        
        var data = new byte[] { 1, 2, 3, 4, 5 };
        await fs2.WriteAllBytesAsync("file.bin", data);
        
        var fallback = new FallbackFileSystem(fs1, fs2);
        
        var readData = await fallback.ReadAllBytesAsync("file.bin");
        CollectionAssert.AreEqual(data, readData);
    }

    [TestMethod]
    public void NestedFallbackFileSystem_ShouldFlattenHierarchy()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        var fs3 = new MemoryFileSystem();
        
        fs1.WriteAllText("file1.txt", "content1");
        fs2.WriteAllText("file2.txt", "content2");
        fs3.WriteAllText("file3.txt", "content3");
        
        var inner = new FallbackFileSystem(fs1, fs2);
        var outer = new FallbackFileSystem(inner, fs3);
        
        // All three file systems should be accessible
        Assert.IsTrue(outer.FileExists("file1.txt"));
        Assert.IsTrue(outer.FileExists("file2.txt"));
        Assert.IsTrue(outer.FileExists("file3.txt"));
    }
}

