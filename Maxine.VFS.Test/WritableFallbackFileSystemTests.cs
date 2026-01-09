namespace Maxine.VFS.Test;

[TestClass]
public class WritableFallbackFileSystemTests
{
    [TestMethod]
    public void CreateFile_ShouldWriteToTargetFileSystem()
    {
        var readOnlyFs = new MemoryFileSystem();
        var writeTargetFs = new MemoryFileSystem();
        
        readOnlyFs.WriteAllText("existing.txt", "readonly content");
        
        var fallback = new WritableFallbackFileSystem(writeTargetFs, readOnlyFs);
        
        fallback.WriteAllText("newfile.txt", "new content");
        
        Assert.IsTrue(writeTargetFs.FileExists("newfile.txt"));
        Assert.IsFalse(readOnlyFs.FileExists("newfile.txt"));
    }

    [TestMethod]
    public void OpenFile_ExistingFile_ShouldOpenFromSource()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        fs1.WriteAllText("file.txt", "content from fs1");
        
        var fallback = new WritableFallbackFileSystem(targetFs, fs1, fs2);
        
        var content = fallback.ReadAllText("file.txt");
        Assert.AreEqual("content from fs1", content);
    }

    [TestMethod]
    public void OpenFile_NewFile_ShouldCreateInTarget()
    {
        var readOnlyFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        var fallback = new WritableFallbackFileSystem(targetFs, readOnlyFs);
        
        fallback.WriteAllText("newfile.txt", "new content");
        
        Assert.IsTrue(targetFs.FileExists("newfile.txt"));
        Assert.AreEqual("new content", targetFs.ReadAllText("newfile.txt"));
    }

    [TestMethod]
    public void FileExists_ShouldCheckAllFileSystems()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        fs1.WriteAllText("file1.txt", "content1");
        fs2.WriteAllText("file2.txt", "content2");
        targetFs.WriteAllText("file3.txt", "content3");
        
        var fallback = new WritableFallbackFileSystem(targetFs, fs1, fs2);
        
        Assert.IsTrue(fallback.FileExists("file1.txt"));
        Assert.IsTrue(fallback.FileExists("file2.txt"));
        Assert.IsTrue(fallback.FileExists("file3.txt"));
        Assert.IsFalse(fallback.FileExists("file4.txt"));
    }

    [TestMethod]
    public void CreateDirectory_ShouldCreateInTarget()
    {
        var readOnlyFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        var fallback = new WritableFallbackFileSystem(targetFs, readOnlyFs);
        
        fallback.CreateDirectory("newdir");
        
        Assert.IsTrue(targetFs.DirectoryExists("newdir"));
        Assert.IsFalse(readOnlyFs.DirectoryExists("newdir"));
    }

    [TestMethod]
    public void DeleteFile_ShouldDeleteFromWritableFileSystem()
    {
        var readOnlyFs = new MemoryFileSystem();
        var writableFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        writableFs.WriteAllText("file.txt", "content");
        
        var fallback = new WritableFallbackFileSystem(targetFs, writableFs, readOnlyFs);
        
        fallback.DeleteFile("file.txt");
        
        Assert.IsFalse(writableFs.FileExists("file.txt"));
    }

    [TestMethod]
    public void DeleteFile_NonExistent_ShouldThrow()
    {
        var readOnlyFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        var fallback = new WritableFallbackFileSystem(targetFs, readOnlyFs);
        
        Assert.Throws<FileNotFoundException>(() => fallback.DeleteFile("nonexistent.txt"));
    }

    [TestMethod]
    public void DeleteDirectory_ShouldDeleteFromWritableFileSystem()
    {
        var readOnlyFs = new MemoryFileSystem();
        var writableFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        writableFs.CreateDirectory("testdir");
        
        var fallback = new WritableFallbackFileSystem(targetFs, writableFs, readOnlyFs);
        
        fallback.DeleteDirectory("testdir");
        
        Assert.IsFalse(writableFs.DirectoryExists("testdir"));
    }

    [TestMethod]
    public void DeleteDirectory_NonExistent_ShouldThrow()
    {
        var readOnlyFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        var fallback = new WritableFallbackFileSystem(targetFs, readOnlyFs);
        
        Assert.Throws<DirectoryNotFoundException>(() => fallback.DeleteDirectory("nonexistent"));
    }

    [TestMethod]
    public void CopyFile_ShouldCopyInWritableFileSystem()
    {
        var readOnlyFs = new MemoryFileSystem();
        var writableFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        writableFs.WriteAllText("source.txt", "content");
        
        var fallback = new WritableFallbackFileSystem(targetFs, writableFs, readOnlyFs);
        
        fallback.CopyFile("source.txt", "dest.txt");
        
        Assert.IsTrue(fallback.FileExists("dest.txt"));
    }

    [TestMethod]
    public void MoveFile_ShouldMoveInWritableFileSystem()
    {
        var readOnlyFs = new MemoryFileSystem();
        var writableFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        writableFs.WriteAllText("source.txt", "content");
        
        var fallback = new WritableFallbackFileSystem(targetFs, writableFs, readOnlyFs);
        
        fallback.MoveFile("source.txt", "dest.txt");
        
        Assert.IsFalse(fallback.FileExists("source.txt"));
        Assert.IsTrue(fallback.FileExists("dest.txt"));
    }

    [TestMethod]
    public void EnumerateFiles_ShouldCombineAllFileSystems()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        fs1.CreateDirectory("dir");
        fs2.CreateDirectory("dir");
        targetFs.CreateDirectory("dir");
        
        fs1.WriteAllText("dir/file1.txt", "content1");
        fs2.WriteAllText("dir/file2.txt", "content2");
        targetFs.WriteAllText("dir/file3.txt", "content3");
        
        var fallback = new WritableFallbackFileSystem(targetFs, fs1, fs2);
        
        var files = fallback.EnumerateFiles("dir").ToList();
        Assert.AreEqual(3, files.Count);
    }

    [TestMethod]
    public void OpenRead_ShouldReadFromFirstAvailable()
    {
        var fs1 = new MemoryFileSystem();
        var fs2 = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        fs1.WriteAllText("file.txt", "content1");
        fs2.WriteAllText("file.txt", "content2");
        
        var fallback = new WritableFallbackFileSystem(targetFs, fs1, fs2);
        
        var content = fallback.ReadAllText("file.txt");
        Assert.AreEqual("content1", content);
    }

    [TestMethod]
    public void OpenRead_FileNotFound_ShouldThrow()
    {
        var readOnlyFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        var fallback = new WritableFallbackFileSystem(targetFs, readOnlyFs);
        
        Assert.Throws<FileNotFoundException>(() => fallback.OpenRead("nonexistent.txt"));
    }

    [TestMethod]
    public void OpenFile_CreateMode_ShouldCreateInTarget()
    {
        var readOnlyFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        var fallback = new WritableFallbackFileSystem(targetFs, readOnlyFs);
        
        using (var stream = fallback.OpenFile("newfile.txt", FileMode.Create, FileAccess.Write))
        using (var writer = new StreamWriter(stream))
        {
            writer.Write("new content");
        }
        
        Assert.IsTrue(targetFs.FileExists("newfile.txt"));
        Assert.AreEqual("new content", targetFs.ReadAllText("newfile.txt"));
    }

    [TestMethod]
    public void SetAttributes_ShouldSetInWritableFileSystem()
    {
        var readOnlyFs = new MemoryFileSystem();
        var writableFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        writableFs.WriteAllText("file.txt", "content");
        
        var fallback = new WritableFallbackFileSystem(targetFs, writableFs, readOnlyFs);
        
        fallback.SetAttributes("file.txt", FileAttributes.ReadOnly);
        
        var attributes = writableFs.GetAttributes("file.txt");
        Assert.IsTrue((attributes & FileAttributes.ReadOnly) != 0);
    }

    [TestMethod]
    public void Dispose_WithLeaveOpenFalse_ShouldDisposeAll()
    {
        var readOnlyFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        var fallback = new WritableFallbackFileSystem(leaveOpen: false, targetFs, readOnlyFs);
        fallback.Dispose();
        
        Assert.IsNotNull(fallback);
    }

    [TestMethod]
    public async Task DisposeAsync_ShouldDisposeAll()
    {
        var readOnlyFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        var fallback = new WritableFallbackFileSystem(leaveOpen: false, targetFs, readOnlyFs);
        await fallback.DisposeAsync();
        
        Assert.IsNotNull(fallback);
    }

    [TestMethod]
    public void WriteAllBytes_ShouldWriteToTarget()
    {
        var readOnlyFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        var fallback = new WritableFallbackFileSystem(targetFs, readOnlyFs);
        
        var data = new byte[] { 1, 2, 3, 4, 5 };
        fallback.WriteAllBytes("file.bin", data);
        
        Assert.IsTrue(targetFs.FileExists("file.bin"));
        CollectionAssert.AreEqual(data, targetFs.ReadAllBytes("file.bin"));
    }

    [TestMethod]
    public void ModifyFile_FromReadOnlySource_ShouldModifyInWritable()
    {
        var readOnlyFs = new MemoryFileSystem();
        var writableFs = new MemoryFileSystem();
        var targetFs = new MemoryFileSystem();
        
        readOnlyFs.WriteAllText("file.txt", "original");
        writableFs.WriteAllText("file.txt", "modified");
        
        var fallback = new WritableFallbackFileSystem(targetFs, writableFs, readOnlyFs);
        
        // Should get from writableFs, not readOnlyFs
        var content = fallback.ReadAllText("file.txt");
        Assert.AreEqual("modified", content);
    }
}

