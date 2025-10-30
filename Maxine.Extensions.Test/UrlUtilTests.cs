namespace Maxine.Extensions.Test;

[TestClass]
public class UrlUtilTests
{
    [TestMethod]
    public void Combine_TwoSegments_CombinesWithSlash()
    {
        var result = UrlUtil.Combine("http://example.com", "path");
        Assert.AreEqual("http://example.com/path", result);
    }

    [TestMethod]
    public void Combine_TwoSegmentsWithTrailingSlash_RemovesDuplicateSlash()
    {
        var result = UrlUtil.Combine("http://example.com/", "path");
        Assert.AreEqual("http://example.com/path", result);
    }

    [TestMethod]
    public void Combine_TwoSegmentsWithLeadingSlash_RemovesDuplicateSlash()
    {
        var result = UrlUtil.Combine("http://example.com", "/path");
        Assert.AreEqual("http://example.com/path", result);
    }

    [TestMethod]
    public void Combine_TwoSegmentsWithBothSlashes_RemovesDuplicateSlash()
    {
        var result = UrlUtil.Combine("http://example.com/", "/path");
        Assert.AreEqual("http://example.com/path", result);
    }

    [TestMethod]
    public void Combine_ThreeSegments_CombinesCorrectly()
    {
        var result = UrlUtil.Combine("http://example.com", "api", "users");
        Assert.AreEqual("http://example.com/api/users", result);
    }

    [TestMethod]
    public void Combine_ThreeSegmentsWithSlashes_RemovesDuplicateSlashes()
    {
        var result = UrlUtil.Combine("http://example.com/", "/api/", "/users");
        Assert.AreEqual("http://example.com/api/users", result);
    }

    [TestMethod]
    public void Combine_EmptySegments_HandlesCorrectly()
    {
        var result = UrlUtil.Combine("http://example.com", "");
        Assert.AreEqual("http://example.com/", result);
    }
}

