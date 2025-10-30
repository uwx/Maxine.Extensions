using Microsoft.Extensions.Primitives;

namespace Maxine.Extensions.Test;

[TestClass]
public class StringSegmentsTests
{
    [TestMethod]
    public void Concat_TwoSegments_ReturnsConcatenatedString()
    {
        var seg1 = new StringSegment("abc");
        var seg2 = new StringSegment("def");
        var result = StringSegments.Concat(seg1, seg2);
        Assert.AreEqual("abcdef", result);
    }

    [TestMethod]
    public void Concat_ThreeSegments_ReturnsConcatenatedString()
    {
        var seg1 = new StringSegment("a");
        var seg2 = new StringSegment("b");
        var seg3 = new StringSegment("c");
        var result = StringSegments.Concat(seg1, seg2, seg3);
        Assert.AreEqual("abc", result);
    }

    [TestMethod]
    public void Concat_FourSegments_ReturnsConcatenatedString()
    {
        var seg1 = new StringSegment("Hello");
        var seg2 = new StringSegment(" ");
        var seg3 = new StringSegment("World");
        var seg4 = new StringSegment("!");
        var result = StringSegments.Concat(seg1, seg2, seg3, seg4);
        Assert.AreEqual("Hello World!", result);
    }

    [TestMethod]
    public void Concat_EmptySegments_ReturnsEmptyString()
    {
        var seg1 = new StringSegment("");
        var seg2 = new StringSegment("");
        var result = StringSegments.Concat(seg1, seg2);
        Assert.AreEqual("", result);
    }
}

