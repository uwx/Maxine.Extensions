using Maxine.VFS;

namespace Maxine.VFS.Test;

[TestClass]
public class IPathTests
{
    [DataTestMethod]
    [DataRow("/home/user/documents/../photos", "/home/user/photos")]
    [DataRow("/home/user/../../root", "/root")]
    [DataRow("folder/subfolder/../file.txt", "folder/file.txt")]
    [DataRow("/root/a/b/../../c", "/root/c")]
    [DataRow("./folder/../file.txt", "file.txt")]
    public void MemoryPath_GetFullPath_ResolvesParentDirectorySegments(string input, string expected)
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.GetFullPath(input);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void MemoryPath_Combine_HandlesAbsolutePaths_UnixFormat_Test1()
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.Combine("/absolute/path", "relative/path");
        StringAssert.StartsWith(result, "/absolute/path");
    }

    [TestMethod]
    public void MemoryPath_Combine_HandlesAbsolutePaths_UnixFormat_Test2()
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.Combine("base/path", "/absolute/override");
        StringAssert.StartsWith(result, "/absolute/override");
    }

    [TestMethod]
    public void MemoryPath_Combine_HandlesAbsolutePaths_WindowsFormat_Test1()
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.Combine("C:\\Windows\\System32", "file.txt");
        
        // When a Windows absolute path is encountered, it should start from there
        StringAssert.StartsWith(result, "C:");
    }

    [TestMethod]
    public void MemoryPath_Combine_HandlesAbsolutePaths_WindowsFormat_Test2()
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.Combine("base\\path", "C:\\absolute\\override");
        
        // When a Windows absolute path is encountered, it should start from there
        StringAssert.StartsWith(result, "C:");
    }

    [DataTestMethod]
    [DataRow("path\\to\\file.txt")]
    [DataRow("folder\\subfolder\\..\\file.txt")]
    [DataRow("C:\\Windows\\System32")]
    [DataRow("mixed/path\\segments\\here")]
    public void MemoryPath_GetFullPath_NormalizesBackslashesToForwardSlashes(string input)
    {
        var path = IPath.MemoryPath.Instance;
        
        // Test through GetFullPath which should normalize
        var result = path.GetFullPath(input);
        Assert.IsFalse(result.Contains('\\'), $"Result '{result}' should not contain backslashes");
        Assert.IsTrue(result.Contains('/'), $"Result '{result}' should contain forward slashes");
    }

    [DataTestMethod]
    [DataRow("path\\to\\file.txt", "file.txt")]
    [DataRow("folder/subfolder\\file.txt", "file.txt")]
    public void MemoryPath_GetFileName_HandlesBackslashes(string input, string expected)
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.GetFileName(input);
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow("path\\to\\file.txt", "path\\to")]
    [DataRow("folder/subfolder\\file.txt", "folder/subfolder")]
    [DataRow("mixed\\forward/back\\file.txt", "mixed\\forward/back")]
    public void MemoryPath_GetDirectoryName_HandlesBackslashes(string input, string expected)
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.GetDirectoryName(input);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void MemoryPath_Combine_JoinsPathsWithForwardSlash_TwoPaths()
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.Combine("folder1", "folder2");
        Assert.AreEqual("folder1/folder2", result);
    }

    [TestMethod]
    public void MemoryPath_Combine_JoinsPathsWithForwardSlash_ThreePaths()
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.Combine("folder1", "folder2", "folder3");
        Assert.AreEqual("folder1/folder2/folder3", result);
    }

    [TestMethod]
    public void MemoryPath_Combine_BackslashAbsolutePath_TreatAsAbsolute()
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.Combine("base", "\\absolute\\path");
        
        // Check if path starts with backslash and verify it's treated as absolute
        StringAssert.StartsWith(result, "\\");
    }

    [TestMethod]
    public void MemoryPath_GetFullPath_RemovesTrailingSlashes()
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.GetFullPath("/path/to/folder/");
        Assert.IsFalse(result.EndsWith('/'), $"Result '{result}' should not end with a slash");
    }

    [TestMethod]
    public void MemoryPath_GetFullPath_RemovesLeadingDotSlash()
    {
        var path = IPath.MemoryPath.Instance;
        var result = path.GetFullPath("./folder/file.txt");
        Assert.IsFalse(result.StartsWith("./"), $"Result '{result}' should not start with './'");
    }
}
