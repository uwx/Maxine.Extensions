using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;
using System.Text.RegularExpressions;

namespace Maxine.Extensions.Test;

[TestClass]
public class RegexExtensionsAdditionalTests
{
    [TestMethod]
    public void ValueMatch_Regex_FindsMatch()
    {
        var regex = new Regex(@"\d+");
        var input = "abc123def".AsSpan();
        
        var match = regex.ValueMatch(input);
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual(3, match.Index);
        Assert.AreEqual(3, match.Length);
        Assert.IsTrue(match.Match.SequenceEqual("123".AsSpan()));
    }

    [TestMethod]
    public void ValueMatch_Regex_NoMatch_ReturnsDefault()
    {
        var regex = new Regex(@"\d+");
        var input = "abcdef".AsSpan();
        
        var match = regex.ValueMatch(input);
        
        Assert.IsFalse(match.IsMatch);
    }

    [TestMethod]
    public void ValueMatch_StaticPattern_FindsMatch()
    {
        var input = "test@example.com".AsSpan();
        
        var match = RegexExtensions.ValueMatch(input, @"@\w+");
        
        Assert.IsTrue(match.IsMatch);
        Assert.IsTrue(match.Match.SequenceEqual("@example".AsSpan()));
    }

    [TestMethod]
    public void ValueMatch_WithOptions_CaseInsensitive()
    {
        var input = "HELLO world".AsSpan();
        
        var match = RegexExtensions.ValueMatch(input, "hello", RegexOptions.IgnoreCase);
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual(0, match.Index);
    }

    [TestMethod]
    public void ValueMatch_WithTimeout_FindsMatch()
    {
        var input = "test123".AsSpan();
        
        var match = RegexExtensions.ValueMatch(input, @"\d+", RegexOptions.None, TimeSpan.FromSeconds(1));
        
        Assert.IsTrue(match.IsMatch);
        Assert.IsTrue(match.Match.SequenceEqual("123".AsSpan()));
    }

    [TestMethod]
    public void ValueMatch_String_FindsMatch()
    {
        var regex = new Regex(@"[a-z]+");
        var input = "123abc456";
        
        var match = regex.ValueMatch(input);
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual(3, match.Index);
        Assert.AreEqual("abc", match.Match.ToString());
    }

    [TestMethod]
    public void ValueMatch_String_NoMatch_ReturnsDefault()
    {
        var regex = new Regex(@"[a-z]+");
        var input = "123456";
        
        var match = regex.ValueMatch(input);
        
        Assert.IsFalse(match.IsMatch);
    }

    [TestMethod]
    public void ValueMatch_StringPattern_FindsMatch()
    {
        var input = "hello world";
        
        var match = RegexExtensions.ValueMatch(input, @"world");
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual("world", match.Match.ToString());
    }

    [TestMethod]
    public void ValueMatch_StringWithOptions_FindsMatch()
    {
        var input = "HELLO world";
        
        var match = RegexExtensions.ValueMatch(input, "hello", RegexOptions.IgnoreCase);
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual("HELLO", match.Match.ToString());
    }

    [TestMethod]
    public void ValueMatch_StringWithTimeout_FindsMatch()
    {
        var input = "test 123 end";
        
        var match = RegexExtensions.ValueMatch(input, @"\d+", RegexOptions.None, TimeSpan.FromSeconds(1));
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual("123", match.Match.ToString());
    }

    [TestMethod]
    public void ValueMatchSpan_ImplicitConversion_ToReadOnlySpan()
    {
        var regex = new Regex(@"\w+");
        var input = "hello world".AsSpan();
        
        var match = regex.ValueMatch(input);
        ReadOnlySpan<char> span = match;
        
        Assert.IsTrue(span.SequenceEqual("hello".AsSpan()));
    }

    [TestMethod]
    public void ValueMatchSpan_DefaultConstructor_NotMatch()
    {
        var match = new ValueMatchSpan();
        
        Assert.IsFalse(match.IsMatch);
        Assert.AreEqual(0, match.Index);
        Assert.AreEqual(0, match.Length);
    }

    [TestMethod]
    public void ValueMatchSegment_DefaultConstructor_NotMatch()
    {
        var match = new ValueMatchSegment();
        
        Assert.IsFalse(match.IsMatch);
        Assert.AreEqual(0, match.Index);
    }

    [TestMethod]
    public void ValueMatch_Regex_WithStartAt_FindsMatch()
    {
        var regex = new Regex(@"\d+");
        var input = "abc123def456".AsSpan();
        
        var match = regex.ValueMatch(input, 6);
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual(9, match.Index);
        Assert.IsTrue(match.Match.SequenceEqual("456".AsSpan()));
    }

    [TestMethod]
    public void ValueMatch_String_WithStartAt_FindsMatch()
    {
        var regex = new Regex(@"\d+");
        var input = "abc123def456";
        
        var match = regex.ValueMatch(input, 6);
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual(9, match.Index);
        Assert.AreEqual("456", match.Match.ToString());
    }
}
