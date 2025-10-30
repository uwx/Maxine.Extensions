namespace Maxine.Extensions.Test;

[TestClass]
public class StringHelpersTests
{
    [TestMethod]
    public void NormalizeFilename_BasicString_ReturnsNormalized()
    {
        var result = StringHelpers.NormalizeFilename("test file");
        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public void NormalizeFilename_WithSpaces_NormalizesSpaces()
    {
        var result = StringHelpers.NormalizeFilename("test  multiple   spaces");
        Assert.IsNotNull(result);
        // Should collapse multiple spaces
        Assert.DoesNotContain("  ", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithSpecialChars_HandlesCorrectly()
    {
        var result = StringHelpers.NormalizeFilename("test-file_name");
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains('-') || result.Contains('_'));
    }
}