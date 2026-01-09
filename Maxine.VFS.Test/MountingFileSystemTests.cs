using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.VFS.Test;

[TestClass]
public class MountingFileSystemTests
{
    [TestMethod]
    public void Constructor_InitializesWithNullFileSystem()
    {
        var fs = new MountingFileSystem();

        // Should behave like NullFileSystem initially
        Assert.IsFalse(fs.FileExists("anyfile.txt"));
        Assert.IsFalse(fs.DirectoryExists("anydir"));
    }

    [TestMethod]
    public void MountNewFileTarget_AllowsFileCreation()
    {
        var fs = new MountingFileSystem();
        var memoryFs = new MemoryFileSystem();

        fs.MountNewFileTarget(memoryFs);

        // Now we should be able to create files
        fs.CreateDirectory("testdir");
        Assert.IsTrue(fs.DirectoryExists("testdir"));
    }

    [TestMethod]
    public void MountNewFileTarget_DelegatesToTarget()
    {
        var fs = new MountingFileSystem();
        var memoryFs = new MemoryFileSystem();
        memoryFs.CreateDirectory("dir1");
        memoryFs.WriteAllText("dir1/file.txt", "content");

        fs.MountNewFileTarget(memoryFs);

        Assert.IsTrue(fs.DirectoryExists("dir1"));
        Assert.IsTrue(fs.FileExists("dir1/file.txt"));
        Assert.AreEqual("content", fs.ReadAllText("dir1/file.txt"));
    }

    [TestMethod]
    public void MountFileSystem_AddsReadOnlyLayer()
    {
        var fs = new MountingFileSystem();
        var memoryFs1 = new MemoryFileSystem();
        memoryFs1.WriteAllText("file1.txt", "content1");

        var memoryFs2 = new MemoryFileSystem();
        memoryFs2.WriteAllText("file2.txt", "content2");

        fs.MountNewFileTarget(memoryFs1);
        fs.MountFileSystem(memoryFs2);

        // Should see files from both file systems
        Assert.IsTrue(fs.FileExists("file1.txt"));
        Assert.IsTrue(fs.FileExists("file2.txt"));
    }

    [TestMethod]
    public void MountFileSystem_WritableTargetTakesPrecedence()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();
        var readOnlyFs = new MemoryFileSystem();

        readOnlyFs.WriteAllText("config.txt", "readonly-value");
        writableFs.WriteAllText("config.txt", "writable-value");

        fs.MountNewFileTarget(writableFs);
        fs.MountFileSystem(readOnlyFs);

        // Writable target should take precedence
        Assert.AreEqual("writable-value", fs.ReadAllText("config.txt"));
    }

    [TestMethod]
    public void MountFileSystem_FallsBackToReadOnlyLayers()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();
        var readOnlyFs = new MemoryFileSystem();

        writableFs.WriteAllText("file1.txt", "writable");
        readOnlyFs.WriteAllText("file2.txt", "readonly");

        fs.MountNewFileTarget(writableFs);
        fs.MountFileSystem(readOnlyFs);

        Assert.AreEqual("writable", fs.ReadAllText("file1.txt"));
        Assert.AreEqual("readonly", fs.ReadAllText("file2.txt"));
    }

    [TestMethod]
    public void GetFiles_CombinesAllLayers()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();
        var readOnlyFs1 = new MemoryFileSystem();
        var readOnlyFs2 = new MemoryFileSystem();

        writableFs.CreateDirectory("dir");
        readOnlyFs1.CreateDirectory("dir");
        readOnlyFs2.CreateDirectory("dir");

        writableFs.WriteAllText("dir/file1.txt", "1");
        readOnlyFs1.WriteAllText("dir/file2.txt", "2");
        readOnlyFs2.WriteAllText("dir/file3.txt", "3");

        fs.MountNewFileTarget(writableFs);
        fs.MountFileSystem(readOnlyFs1);
        fs.MountFileSystem(readOnlyFs2);

        var files = fs.GetFiles("dir").ToList();

        Assert.IsTrue(files.Contains("dir/file1.txt"));
        Assert.IsTrue(files.Contains("dir/file2.txt"));
        Assert.IsTrue(files.Contains("dir/file3.txt"));
    }

    [TestMethod]
    public void GetDirectories_CombinesAllLayers()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();
        var readOnlyFs1 = new MemoryFileSystem();

        writableFs.CreateDirectory("dir1");
        readOnlyFs1.CreateDirectory("dir2");

        fs.MountNewFileTarget(writableFs);
        fs.MountFileSystem(readOnlyFs1);

        var dirs = fs.GetDirectories("").ToList();

        Assert.IsTrue(dirs.Contains("dir1"));
        Assert.IsTrue(dirs.Contains("dir2"));
    }

    [TestMethod]
    public void CreateDirectory_WritesToNewFileTarget()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();

        fs.MountNewFileTarget(writableFs);
        fs.CreateDirectory("newdir");

        // Verify it was created in the writable target
        Assert.IsTrue(writableFs.DirectoryExists("newdir"));
    }

    [TestMethod]
    public void WriteFile_WritesToNewFileTarget()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();
        var readOnlyFs = new MemoryFileSystem();

        readOnlyFs.WriteAllText("readonly.txt", "readonly");

        fs.MountNewFileTarget(writableFs);
        fs.MountFileSystem(readOnlyFs);

        fs.WriteAllText("newfile.txt", "content");

        // Verify it was written to the writable target
        Assert.IsTrue(writableFs.FileExists("newfile.txt"));
        Assert.AreEqual("content", writableFs.ReadAllText("newfile.txt"));
    }

    [TestMethod]
    public void DeleteFile_DeletesFromNewFileTarget()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();

        writableFs.WriteAllText("file.txt", "content");

        fs.MountNewFileTarget(writableFs);
        fs.DeleteFile("file.txt");

        Assert.IsFalse(writableFs.FileExists("file.txt"));
    }

    [TestMethod]
    public void DeleteDirectory_DeletesFromNewFileTarget()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();

        writableFs.CreateDirectory("dir");

        fs.MountNewFileTarget(writableFs);
        fs.DeleteDirectory("dir");

        Assert.IsFalse(writableFs.DirectoryExists("dir"));
    }

    [TestMethod]
    public void CopyFile_CopiesToNewFileTarget()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();

        writableFs.WriteAllText("source.txt", "content");

        fs.MountNewFileTarget(writableFs);
        fs.CopyFile("source.txt", "dest.txt");

        Assert.IsTrue(writableFs.FileExists("dest.txt"));
        Assert.AreEqual("content", writableFs.ReadAllText("dest.txt"));
    }

    [TestMethod]
    public void MoveFile_MovesInNewFileTarget()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();

        writableFs.WriteAllText("old.txt", "content");

        fs.MountNewFileTarget(writableFs);
        fs.MoveFile("old.txt", "new.txt");

        Assert.IsFalse(writableFs.FileExists("old.txt"));
        Assert.IsTrue(writableFs.FileExists("new.txt"));
    }

    [TestMethod]
    public void SetAttributes_SetsOnNewFileTarget()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();

        writableFs.WriteAllText("file.txt", "content");

        fs.MountNewFileTarget(writableFs);
        fs.SetAttributes("file.txt", FileAttributes.ReadOnly);

        Assert.AreEqual(FileAttributes.ReadOnly, writableFs.GetAttributes("file.txt"));
    }

    [TestMethod]
    public void EnumerateFiles_WithPattern_WorksAcrossLayers()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();
        var readOnlyFs = new MemoryFileSystem();

        writableFs.CreateDirectory("dir");
        readOnlyFs.CreateDirectory("dir");

        writableFs.WriteAllText("dir/file1.txt", "1");
        writableFs.WriteAllText("dir/file2.log", "2");
        readOnlyFs.WriteAllText("dir/file3.txt", "3");

        fs.MountNewFileTarget(writableFs);
        fs.MountFileSystem(readOnlyFs);

        var txtFiles = fs.EnumerateFiles("dir", "*.txt").ToList();

        Assert.AreEqual(2, txtFiles.Count);
        Assert.IsTrue(txtFiles.Contains("dir/file1.txt"));
        Assert.IsTrue(txtFiles.Contains("dir/file3.txt"));
    }

    [TestMethod]
    public void EnumerateDirectories_WithPattern_WorksAcrossLayers()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();
        var readOnlyFs = new MemoryFileSystem();

        writableFs.CreateDirectory("test_dir");
        writableFs.CreateDirectory("prod_dir");
        readOnlyFs.CreateDirectory("test_data");

        fs.MountNewFileTarget(writableFs);
        fs.MountFileSystem(readOnlyFs);

        var testDirs = fs.EnumerateDirectories("", "test*").ToList();

        Assert.AreEqual(2, testDirs.Count);
        Assert.IsTrue(testDirs.Contains("test_dir"));
        Assert.IsTrue(testDirs.Contains("test_data"));
    }

    [TestMethod]
    public void OpenFile_ReadsFromCorrectLayer()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();
        var readOnlyFs = new MemoryFileSystem();

        writableFs.WriteAllText("file.txt", "writable");
        readOnlyFs.WriteAllText("readonly.txt", "readonly");

        fs.MountNewFileTarget(writableFs);
        fs.MountFileSystem(readOnlyFs);

        using var stream1 = fs.OpenFile("file.txt", FileMode.Open, FileAccess.Read);
        using var stream2 = fs.OpenFile("readonly.txt", FileMode.Open, FileAccess.Read);

        using var reader1 = new StreamReader(stream1);
        using var reader2 = new StreamReader(stream2);

        Assert.AreEqual("writable", reader1.ReadToEnd());
        Assert.AreEqual("readonly", reader2.ReadToEnd());
    }

    [TestMethod]
    public async Task OpenFileAsync_WorksCorrectly()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();

        writableFs.WriteAllText("file.txt", "content");

        fs.MountNewFileTarget(writableFs);

        await using var stream = await fs.OpenFileAsync("file.txt", FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(stream);

        Assert.AreEqual("content", await reader.ReadToEndAsync());
    }

    [TestMethod]
    public void MultipleReadOnlyLayers_FirstMatchWins()
    {
        var fs = new MountingFileSystem();
        var writableFs = new MemoryFileSystem();
        var readOnlyFs1 = new MemoryFileSystem();
        var readOnlyFs2 = new MemoryFileSystem();

        readOnlyFs1.WriteAllText("shared.txt", "from-layer1");
        readOnlyFs2.WriteAllText("shared.txt", "from-layer2");

        fs.MountNewFileTarget(writableFs);
        fs.MountFileSystem(readOnlyFs1);
        fs.MountFileSystem(readOnlyFs2);

        // First read-only layer should be checked first
        var content = fs.ReadAllText("shared.txt");
        Assert.AreEqual("from-layer1", content);
    }

    [TestMethod]
    public void WithoutNewFileTarget_CannotWriteFiles()
    {
        var fs = new MountingFileSystem();
        var readOnlyFs = new MemoryFileSystem();

        readOnlyFs.WriteAllText("file.txt", "content");
        fs.MountFileSystem(readOnlyFs);

        // Should behave like NullFileSystem for writes
        Assert.Throws<DirectoryNotFoundException>(() => fs.CreateDirectory("newdir"));
    }
}
