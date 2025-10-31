using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class ProjectUtilsAdditionalTests
{
    [TestMethod]
    public void TryGetSolutionDirectory_FindsSolution()
    {
        // Assuming we're running from within the solution
        var solutionDir = ProjectUtils.TryGetSolutionDirectory();

        Assert.IsNotNull(solutionDir);
        Assert.IsTrue(Directory.Exists(solutionDir));
    }

    [TestMethod]
    public void TryGetProjectDirectory_FindsProject()
    {
        // Assuming we're running from within a project
        var projectDir = ProjectUtils.TryGetProjectDirectory();

        Assert.IsNotNull(projectDir);
        Assert.IsTrue(Directory.Exists(projectDir));
    }

    [TestMethod]
    public void TryGetSolutionDirectory_WithSpecificPath_FindsSolution()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var solutionDir = ProjectUtils.TryGetSolutionDirectory(currentDir);

        Assert.IsNotNull(solutionDir);
    }

    [TestMethod]
    public void TryGetProjectDirectory_WithSpecificPath_FindsProject()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var projectDir = ProjectUtils.TryGetProjectDirectory(currentDir);

        Assert.IsNotNull(projectDir);
    }

    [TestMethod]
    public void TryGetSolutionDirectory_CachesResult()
    {
        // Clear any existing cache by using reflection (if needed)
        var first = ProjectUtils.TryGetSolutionDirectory();
        var second = ProjectUtils.TryGetSolutionDirectory();

        Assert.AreEqual(first, second);
    }

    [TestMethod]
    public void TryGetProjectDirectory_CachesResult()
    {
        var first = ProjectUtils.TryGetProjectDirectory();
        var second = ProjectUtils.TryGetProjectDirectory();

        Assert.AreEqual(first, second);
    }
}
