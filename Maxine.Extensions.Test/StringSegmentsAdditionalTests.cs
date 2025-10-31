using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;
using Microsoft.Extensions.Primitives;

namespace Maxine.Extensions.Test;

[TestClass]
public class StringSegmentsAdditionalTests
{
    [TestMethod]
    public void Concat_TwoSegments_CombinesCorrectly()
    {
        var seg1 = new StringSegment("Hello");
        var seg2 = new StringSegment("World");
        
        var result = StringSegments.Concat(seg1, seg2);
        
        Assert.AreEqual("HelloWorld", result);
    }

    [TestMethod]
    public void Concat_TwoSegments_WithEmpty_HandlesCorrectly()
    {
        var seg1 = new StringSegment("Hello");
        var seg2 = new StringSegment(string.Empty);
        
        var result = StringSegments.Concat(seg1, seg2);
        
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void Concat_ThreeSegments_CombinesCorrectly()
    {
        var seg1 = new StringSegment("One");
        var seg2 = new StringSegment("Two");
        var seg3 = new StringSegment("Three");
        
        var result = StringSegments.Concat(seg1, seg2, seg3);
        
        Assert.AreEqual("OneTwoThree", result);
    }

    [TestMethod]
    public void Concat_ThreeSegments_WithSpaces_CombinesCorrectly()
    {
        var seg1 = new StringSegment("Hello");
        var seg2 = new StringSegment(" ");
        var seg3 = new StringSegment("World");
        
        var result = StringSegments.Concat(seg1, seg2, seg3);
        
        Assert.AreEqual("Hello World", result);
    }

    [TestMethod]
    public void Concat_FourSegments_CombinesCorrectly()
    {
        var seg1 = new StringSegment("A");
        var seg2 = new StringSegment("B");
        var seg3 = new StringSegment("C");
        var seg4 = new StringSegment("D");
        
        var result = StringSegments.Concat(seg1, seg2, seg3, seg4);
        
        Assert.AreEqual("ABCD", result);
    }

    [TestMethod]
    public void Concat_FourSegments_MixedContent_CombinesCorrectly()
    {
        var seg1 = new StringSegment("Hello");
        var seg2 = new StringSegment(" ");
        var seg3 = new StringSegment("beautiful");
        var seg4 = new StringSegment(" world!");
        
        var result = StringSegments.Concat(seg1, seg2, seg3, seg4);
        
        Assert.AreEqual("Hello beautiful world!", result);
    }

    [TestMethod]
    public void Concat_TwoSegments_Substrings_WorksCorrectly()
    {
        var source = "Hello World";
        var seg1 = new StringSegment(source, 0, 5); // "Hello"
        var seg2 = new StringSegment(source, 6, 5); // "World"
        
        var result = StringSegments.Concat(seg1, seg2);
        
        Assert.AreEqual("HelloWorld", result);
    }

    [TestMethod]
    public void Concat_ThreeSegments_AllEmpty_ReturnsEmpty()
    {
        var seg1 = new StringSegment(string.Empty);
        var seg2 = new StringSegment(string.Empty);
        var seg3 = new StringSegment(string.Empty);
        
        var result = StringSegments.Concat(seg1, seg2, seg3);
        
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Concat_FourSegments_SomeEmpty_CombinesNonEmpty()
    {
        var seg1 = new StringSegment("A");
        var seg2 = new StringSegment(string.Empty);
        var seg3 = new StringSegment("C");
        var seg4 = new StringSegment(string.Empty);
        
        var result = StringSegments.Concat(seg1, seg2, seg3, seg4);
        
        Assert.AreEqual("AC", result);
    }

    [TestMethod]
    public void Concat_TwoSegments_SpecialCharacters_PreservesContent()
    {
        var seg1 = new StringSegment("Test@#$");
        var seg2 = new StringSegment("%^&*()");
        
        var result = StringSegments.Concat(seg1, seg2);
        
        Assert.AreEqual("Test@#$%^&*()", result);
    }

    [TestMethod]
    public void Concat_ThreeSegments_Unicode_PreservesContent()
    {
        var seg1 = new StringSegment("Hello");
        var seg2 = new StringSegment("世界");
        var seg3 = new StringSegment("🌍");
        
        var result = StringSegments.Concat(seg1, seg2, seg3);
        
        Assert.AreEqual("Hello世界🌍", result);
    }

    [TestMethod]
    public void Concat_FourSegments_Numbers_CombinesCorrectly()
    {
        var seg1 = new StringSegment("123");
        var seg2 = new StringSegment("456");
        var seg3 = new StringSegment("789");
        var seg4 = new StringSegment("000");
        
        var result = StringSegments.Concat(seg1, seg2, seg3, seg4);
        
        Assert.AreEqual("123456789000", result);
    }
}
