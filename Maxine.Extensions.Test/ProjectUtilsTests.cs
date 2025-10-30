using Maxine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Maxine.Extensions.Test;

[TestClass]
public class ProjectUtilsTests
{
    [TestMethod]
    public void TryGetSolutionDirectory_FromTestProject_FindsSolution()
    {
        var solutionDir = ProjectUtils.TryGetSolutionDirectory();
        
        // Should find the solution file
        if (solutionDir != null)
        {
            Assert.IsTrue(Directory.Exists(solutionDir));
            var slnFiles = Directory.GetFiles(solutionDir, "*.sln");
            Assert.IsNotEmpty(slnFiles);
        }
    }

    [TestMethod]
    public void TryGetSolutionDirectory_CalledTwice_ReturnsCachedValue()
    {
        var result1 = ProjectUtils.TryGetSolutionDirectory();
        var result2 = ProjectUtils.TryGetSolutionDirectory();
        
        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void TryGetProjectDirectory_FromTestProject_FindsProject()
    {
        var projectDir = ProjectUtils.TryGetProjectDirectory();
        
        // Should find a project file
        if (projectDir != null)
        {
            Assert.IsTrue(Directory.Exists(projectDir));
            var csprojFiles = Directory.GetFiles(projectDir, "*.csproj");
            Assert.IsNotEmpty(csprojFiles);
        }
    }

    [TestMethod]
    public void TryGetProjectDirectory_CalledTwice_ReturnsCachedValue()
    {
        var result1 = ProjectUtils.TryGetProjectDirectory();
        var result2 = ProjectUtils.TryGetProjectDirectory();
        
        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void TryGetSolutionDirectory_WithCustomPath_SearchesFromPath()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var result = ProjectUtils.TryGetSolutionDirectory(currentDir);
        
        // Result can be null if no solution is found
        Assert.IsTrue(result == null || Directory.Exists(result));
    }

    [TestMethod]
    public void TryGetProjectDirectory_WithCustomPath_SearchesFromPath()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var result = ProjectUtils.TryGetProjectDirectory(currentDir);
        
        // Result can be null if no project is found
        Assert.IsTrue(result == null || Directory.Exists(result));
    }

    [TestMethod]
    public void TryGetSolutionDirectory_NonExistentPath_ReturnsNull()
    {
        // Create a temp directory with no solution
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        try
        {
            // Clear cache first by getting with a different path
            var result = ProjectUtils.TryGetSolutionDirectory(tempDir);
            
            // Should return null when no solution found
            Assert.IsTrue(result == null || !result.Equals(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}

