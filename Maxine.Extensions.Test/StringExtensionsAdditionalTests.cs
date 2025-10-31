using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class StringExtensionsAdditionalTests
{
    [TestMethod]
    public void StripLeadingBom_WithBom_RemovesBom()
    {
        var str = "\uFEFFHello";
        var result = str.StripLeadingBom();
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void StripLeadingBom_WithZeroWidthSpace_RemovesIt()
    {
        var str = "\u200BWorld";
        var result = str.StripLeadingBom();
        Assert.AreEqual("World", result);
    }

    [TestMethod]
    public void StripLeadingBom_WithoutBom_ReturnsUnchanged()
    {
        var str = "Hello";
        var result = str.StripLeadingBom();
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void StripLeadingBom_MultipleBoms_RemovesAll()
    {
        var str = "\uFEFF\uFEFF\u200BTest";
        var result = str.StripLeadingBom();
        Assert.AreEqual("Test", result);
    }

    [TestMethod]
    public void StripLeadingBom_EmptyString_ReturnsEmpty()
    {
        var str = string.Empty;
        var result = str.StripLeadingBom();
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void StripLeadingBom_ReadOnlySpan_WithBom_RemovesBom()
    {
        ReadOnlySpan<char> span = "\uFEFFTest".AsSpan();
        var result = span.StripLeadingBom();
        Assert.AreEqual("Test", result.ToString());
    }

    [TestMethod]
    public void Truncate_ShorterThanLength_ReturnsOriginal()
    {
        var str = "Hello";
        var result = str.Truncate(10);
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void Truncate_EqualToLength_ReturnsOriginal()
    {
        var str = "Hello";
        var result = str.Truncate(5);
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void Truncate_LongerThanLength_TruncatesCorrectly()
    {
        var str = "Hello World";
        var result = str.Truncate(5);
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void Truncate_ZeroLength_ReturnsEmpty()
    {
        var str = "Hello";
        var result = str.Truncate(0);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Truncate_EmptyString_ReturnsEmpty()
    {
        var str = string.Empty;
        var result = str.Truncate(5);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void SplitEnumerator_SingleChar_SplitsCorrectly()
    {
        var str = "a,b,c";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(','))
        {
            parts.Add(part.ToString());
        }

        CollectionAssert.AreEqual(new[] { "a", "b", "c" }, parts);
    }

    [TestMethod]
    public void SplitEnumerator_NoDelimiter_ReturnsSinglePart()
    {
        var str = "abc";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(','))
        {
            parts.Add(part.ToString());
        }

        CollectionAssert.AreEqual(new[] { "abc" }, parts);
    }

    [TestMethod]
    public void SplitEnumerator_EmptyString_ReturnsEmpty()
    {
        var str = string.Empty;
        var count = 0;
        
        foreach (var part in str.SplitEnumerator(','))
        {
            count++;
        }

        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void SplitEnumerator_TrailingDelimiter_IncludesEmptyPart()
    {
        var str = "a,b,";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(','))
        {
            parts.Add(part.ToString());
        }

        Assert.AreEqual(3, parts.Count);
        Assert.AreEqual("a", parts[0]);
        Assert.AreEqual("b", parts[1]);
        Assert.AreEqual(string.Empty, parts[2]);
    }

    [TestMethod]
    public void SplitEnumerator_LeadingDelimiter_IncludesEmptyPart()
    {
        var str = ",a,b";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(','))
        {
            parts.Add(part.ToString());
        }

        Assert.AreEqual(3, parts.Count);
        Assert.AreEqual(string.Empty, parts[0]);
        Assert.AreEqual("a", parts[1]);
        Assert.AreEqual("b", parts[2]);
    }

    [TestMethod]
    public void SplitEnumerator_ConsecutiveDelimiters_IncludesEmptyParts()
    {
        var str = "a,,b";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(','))
        {
            parts.Add(part.ToString());
        }

        Assert.AreEqual(3, parts.Count);
        Assert.AreEqual("a", parts[0]);
        Assert.AreEqual(string.Empty, parts[1]);
        Assert.AreEqual("b", parts[2]);
    }

    [TestMethod]
    public void SplitEnumerator_MultipleChars_SplitsOnAny()
    {
        var str = "a,b;c:d";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator([',', ';', ':']))
        {
            parts.Add(part.ToString());
        }

        CollectionAssert.AreEqual(new[] { "a", "b", "c", "d" }, parts);
    }

    [TestMethod]
    public void SplitEnumerator_WithOptions_RemovesEmptyEntries()
    {
        var str = "a,,b";
        var parts = new List<string>();
        
        foreach (var part in str.SplitEnumerator(',', StringSplitOptions.RemoveEmptyEntries))
        {
            parts.Add(part.ToString());
        }

        CollectionAssert.AreEqual(new[] { "a", "b" }, parts);
    }

    [TestMethod]
    public void SplitEnumerator_Span_WorksCorrectly()
    {
        ReadOnlySpan<char> span = "x|y|z".AsSpan();
        var parts = new List<string>();
        
        foreach (var part in span.SplitEnumerator('|'))
        {
            parts.Add(part.ToString());
        }

        CollectionAssert.AreEqual(new[] { "x", "y", "z" }, parts);
    }

    [TestMethod]
    public void SplitEnumerator_RealWorldExample_ParsesCsv()
    {
        var csv = "Name,Age,City";
        var parts = new List<string>();
        
        foreach (var part in csv.SplitEnumerator(','))
        {
            parts.Add(part.ToString());
        }

        CollectionAssert.AreEqual(new[] { "Name", "Age", "City" }, parts);
    }

    [TestMethod]
    public void Truncate_LongString_PreservesStart()
    {
        var str = "The quick brown fox jumps over the lazy dog";
        var result = str.Truncate(15);
        Assert.AreEqual("The quick brown", result);
    }
}
