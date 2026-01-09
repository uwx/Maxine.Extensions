using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.VFS.Test;

[TestClass]
public class NullFileSystemTests
{
    [TestMethod]
    public void EnumerateFiles_ReturnsEmptyCollection()
    {
        var fs = new NullFileSystem();

        var files = fs.EnumerateFiles("anypath").ToList();

        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void EnumerateFiles_WithPattern_ReturnsEmptyCollection()
    {
        var fs = new NullFileSystem();

        var files = fs.EnumerateFiles("anypath", "*.txt").ToList();

        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void EnumerateFiles_WithPatternRecursive_ReturnsEmptyCollection()
    {
        var fs = new NullFileSystem();

        var files = fs.EnumerateFiles("anypath", "*.txt", SearchOption.AllDirectories).ToList();

        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void GetFiles_ReturnsEmptyCollection()
    {
        var fs = new NullFileSystem();

        var files = fs.GetFiles("anypath");

        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void GetFiles_WithPattern_ReturnsEmptyCollection()
    {
        var fs = new NullFileSystem();

        var files = fs.GetFiles("anypath", "*.txt");

        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void EnumerateDirectories_ReturnsEmptyCollection()
    {
        var fs = new NullFileSystem();

        var dirs = fs.EnumerateDirectories("anypath").ToList();

        Assert.AreEqual(0, dirs.Count);
    }

    [TestMethod]
    public void EnumerateDirectories_WithPattern_ReturnsEmptyCollection()
    {
        var fs = new NullFileSystem();

        var dirs = fs.EnumerateDirectories("anypath", "*").ToList();

        Assert.AreEqual(0, dirs.Count);
    }

    [TestMethod]
    public void GetDirectories_ReturnsEmptyCollection()
    {
        var fs = new NullFileSystem();

        var dirs = fs.GetDirectories("anypath");

        Assert.AreEqual(0, dirs.Count);
    }

    [TestMethod]
    public void FileExists_ReturnsFalse()
    {
        var fs = new NullFileSystem();

        Assert.IsFalse(fs.FileExists("anyfile.txt"));
    }

    [TestMethod]
    public void DirectoryExists_ReturnsFalse()
    {
        var fs = new NullFileSystem();

        Assert.IsFalse(fs.DirectoryExists("anydir"));
    }

    [TestMethod]
    public void Exists_ReturnsFalse()
    {
        var fs = new NullFileSystem();

        Assert.IsFalse(fs.Exists("anypath"));
    }

    [TestMethod]
    public void GetAttributes_ThrowsFileNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<FileNotFoundException>(() => fs.GetAttributes("anyfile.txt"));
    }

    [TestMethod]
    public void OpenFile_ThrowsFileNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<FileNotFoundException>(() => fs.OpenFile("anyfile.txt"));
    }

    [TestMethod]
    public void OpenFile_WithMode_ThrowsFileNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<FileNotFoundException>(() => fs.OpenFile("anyfile.txt", FileMode.Open));
    }

    [TestMethod]
    public void OpenFile_WithModeAndAccess_ThrowsFileNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<FileNotFoundException>(() => fs.OpenFile("anyfile.txt", FileMode.Open, FileAccess.Read));
    }

    [TestMethod]
    public void CreateDirectory_ThrowsDirectoryNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<DirectoryNotFoundException>(() => fs.CreateDirectory("anydir"));
    }

    [TestMethod]
    public void DeleteFile_ThrowsDirectoryNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<DirectoryNotFoundException>(() => fs.DeleteFile("anyfile.txt"));
    }

    [TestMethod]
    public void DeleteDirectory_ThrowsDirectoryNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<DirectoryNotFoundException>(() => fs.DeleteDirectory("anydir"));
    }

    [TestMethod]
    public void DeleteDirectory_Recursive_ThrowsDirectoryNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<DirectoryNotFoundException>(() => fs.DeleteDirectory("anydir", recursive: true));
    }

    [TestMethod]
    public void CopyFile_ThrowsFileNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<FileNotFoundException>(() => fs.CopyFile("from.txt", "to.txt"));
    }

    [TestMethod]
    public void CopyFile_WithOverwrite_ThrowsFileNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<FileNotFoundException>(() => fs.CopyFile("from.txt", "to.txt", overwrite: true));
    }

    [TestMethod]
    public void MoveFile_ThrowsFileNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<FileNotFoundException>(() => fs.MoveFile("from.txt", "to.txt"));
    }

    [TestMethod]
    public void MoveFile_WithOverwrite_ThrowsFileNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<FileNotFoundException>(() => fs.MoveFile("from.txt", "to.txt", overwrite: true));
    }

    [TestMethod]
    public void SetAttributes_ThrowsFileNotFoundException()
    {
        var fs = new NullFileSystem();

        Assert.Throws<FileNotFoundException>(() => fs.SetAttributes("anyfile.txt", FileAttributes.ReadOnly));
    }
}
