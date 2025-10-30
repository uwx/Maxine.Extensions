using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    public void StripLeadingBom_WithBom_RemovesBom()
    {
        var str = "\uFEFFHello World";
        var result = str.StripLeadingBom();
        Assert.AreEqual("Hello World", result);
    }

    [TestMethod]
    public void StripLeadingBom_WithZeroWidthSpace_RemovesIt()
    {
        var str = "\u200BHello World";
        var result = str.StripLeadingBom();
        Assert.AreEqual("Hello World", result);
    }

    [TestMethod]
    public void StripLeadingBom_WithoutBom_ReturnsOriginal()
    {
        var str = "Hello World";
        var result = str.StripLeadingBom();
        Assert.AreEqual("Hello World", result);
    }

    [TestMethod]
    public void StripLeadingBom_Span_WithBom_RemovesBom()
    {
        ReadOnlySpan<char> str = "\uFEFFHello".AsSpan();
        var result = str.StripLeadingBom();
        Assert.AreEqual("Hello", result.ToString());
    }

    [TestMethod]
    public void Truncate_ShorterThanLength_ReturnsOriginal()
    {
        var str = "Hello";
        var result = str.Truncate(10);
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void Truncate_LongerThanLength_TruncatesString()
    {
        var str = "Hello World";
        var result = str.Truncate(5);
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void Truncate_ExactLength_ReturnsOriginal()
    {
        var str = "Hello";
        var result = str.Truncate(5);
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void Truncate_ZeroLength_ReturnsEmpty()
    {
        var str = "Hello";
        var result = str.Truncate(0);
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void SplitEnumerator_SingleChar_SplitsCorrectly()
    {
        var str = "a,b,c,d";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(','))
        {
            parts.Add(part.ToString());
        }

        Assert.HasCount(4, parts);
        Assert.AreEqual("a", parts[0]);
        Assert.AreEqual("b", parts[1]);
        Assert.AreEqual("c", parts[2]);
        Assert.AreEqual("d", parts[3]);
    }

    [TestMethod]
    public void SplitEnumerator_MultipleChars_SplitsOnAny()
    {
        var str = "a,b;c.d";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator([',', ';', '.']))
        {
            parts.Add(part.ToString());
        }

        Assert.HasCount(4, parts);
        Assert.AreEqual("a", parts[0]);
        Assert.AreEqual("b", parts[1]);
        Assert.AreEqual("c", parts[2]);
        Assert.AreEqual("d", parts[3]);
    }

    [TestMethod]
    public void SplitEnumerator_WithOptions_RemovesEmptyEntries()
    {
        var str = "a,,b,c";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(',', StringSplitOptions.RemoveEmptyEntries))
        {
            parts.Add(part.ToString());
        }

        Assert.HasCount(3, parts);
        Assert.AreEqual("a", parts[0]);
        Assert.AreEqual("b", parts[1]);
        Assert.AreEqual("c", parts[2]);
    }

    [TestMethod]
    public void SplitEnumerator_WithOptions_TrimsEntries()
    {
        var str = "a , b , c";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(',', StringSplitOptions.TrimEntries))
        {
            parts.Add(part.ToString());
        }

        Assert.HasCount(3, parts);
        Assert.AreEqual("a", parts[0]);
        Assert.AreEqual("b", parts[1]);
        Assert.AreEqual("c", parts[2]);
    }

    [TestMethod]
    public void SplitEnumerator_EmptyString_ReturnsNoElements()
    {
        var str = "";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(','))
        {
            parts.Add(part.ToString());
        }

        Assert.IsEmpty(parts);
    }

    [TestMethod]
    public void SplitEnumerator_NoDelimiter_ReturnsSingleElement()
    {
        var str = "hello";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(','))
        {
            parts.Add(part.ToString());
        }

        Assert.HasCount(1, parts);
        Assert.AreEqual("hello", parts[0]);
    }

    [TestMethod]
    public void SplitEnumerator_Span_SplitsCorrectly()
    {
        ReadOnlySpan<char> str = "x|y|z".AsSpan();
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator('|'))
        {
            parts.Add(part.ToString());
        }

        Assert.HasCount(3, parts);
        Assert.AreEqual("x", parts[0]);
        Assert.AreEqual("y", parts[1]);
        Assert.AreEqual("z", parts[2]);
    }

    [TestMethod]
    public void SplitEnumerator_ConsecutiveDelimiters_CreatesEmptyParts()
    {
        var str = "a,,c";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(','))
        {
            parts.Add(part.ToString());
        }

        Assert.HasCount(3, parts);
        Assert.AreEqual("a", parts[0]);
        Assert.AreEqual("", parts[1]);
        Assert.AreEqual("c", parts[2]);
    }
}
