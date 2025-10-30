namespace Maxine.Extensions.Test;

[TestClass]
public class FileNameHelpersTests
{
    [TestMethod]
    public void NormalizeFilename_BasicString_ReturnsNormalized()
    {
        var result = FileNameHelpers.NormalizeFilename("test file");
        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public void NormalizeFilename_WithDashes_HandlesCorrectly()
    {
        var result = FileNameHelpers.NormalizeFilename("test-file-name");
        Assert.IsNotNull(result);
        Assert.Contains('-', result);
    }

    [TestMethod]
    public void NormalizeFilename_WithSpaces_ReplacesWithSingleSpace()
    {
        var result = FileNameHelpers.NormalizeFilename("test  file   name");
        Assert.IsNotNull(result);
        Assert.DoesNotContain("  ", result); // No double spaces
    }

    [TestMethod]
    public void NormalizeFilename_WithUnderscores_PreservesUnderscores()
    {
        var result = FileNameHelpers.NormalizeFilename("test_file_name");
        Assert.IsNotNull(result);
        Assert.Contains('_', result);
    }

    [TestMethod]
    public void NormalizeFilename_WithInvalidChars_RemovesInvalidChars()
    {
        var result = FileNameHelpers.NormalizeFilename("test<>|:file");
        Assert.IsNotNull(result);
        Assert.DoesNotContain('<', result);
        Assert.DoesNotContain('>', result);
    }
}