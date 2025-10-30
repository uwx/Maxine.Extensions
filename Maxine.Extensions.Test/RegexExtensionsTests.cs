using Maxine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Maxine.Extensions.Test;

[TestClass]
public class RegexExtensionsTests
{
    [TestMethod]
    public void ValueMatch_WithRegexAndSpan_FindsMatch()
    {
        var regex = new Regex(@"\d+");
        ReadOnlySpan<char> input = "abc123def".AsSpan();
        
        var match = regex.ValueMatch(input);
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual("123", match.Match.ToString());
        Assert.AreEqual(3, match.Index);
        Assert.AreEqual(3, match.Length);
    }

    [TestMethod]
    public void ValueMatch_WithRegexAndSpan_NoMatch_ReturnsNotMatch()
    {
        var regex = new Regex(@"\d+");
        ReadOnlySpan<char> input = "abcdef".AsSpan();
        
        var match = regex.ValueMatch(input);
        
        Assert.IsFalse(match.IsMatch);
    }

    [TestMethod]
    public void ValueMatch_StaticWithSpan_FindsMatch()
    {
        ReadOnlySpan<char> input = "test@example.com".AsSpan();
        
        var match = RegexExtensions.ValueMatch(input, @"\w+@\w+\.\w+");
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual("test@example.com", match.Match.ToString());
    }

    [TestMethod]
    public void ValueMatch_WithRegexAndString_FindsMatch()
    {
        var regex = new Regex(@"[A-Z]+");
        var input = "helloWORLDgoodbye";
        
        var match = regex.ValueMatch(input);
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual("WORLD", match.Match.ToString());
        Assert.AreEqual(5, match.Index);
        Assert.AreEqual(5, match.Length);
    }

    [TestMethod]
    public void ValueMatch_WithRegexAndString_NoMatch_ReturnsNotMatch()
    {
        var regex = new Regex(@"\d+");
        var input = "nodigits";
        
        var match = regex.ValueMatch(input);
        
        Assert.IsFalse(match.IsMatch);
    }

    [TestMethod]
    public void ValueMatch_StaticWithString_FindsMatch()
    {
        var input = "Price: $99.99";
        
        var match = RegexExtensions.ValueMatch(input, @"\$\d+\.\d+");
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual("$99.99", match.Match.ToString());
    }

    [TestMethod]
    public void ValueMatch_WithOptions_FindsMatch()
    {
        var input = "HELLO world";
        
        var match = RegexExtensions.ValueMatch(input, "hello", RegexOptions.IgnoreCase);
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual("HELLO", match.Match.ToString());
    }

    [TestMethod]
    public void ValueMatchSpan_ImplicitConversion_WorksCorrectly()
    {
        var regex = new Regex(@"\w+");
        ReadOnlySpan<char> input = "hello world".AsSpan();
        
        var match = regex.ValueMatch(input);
        ReadOnlySpan<char> span = match; // Implicit conversion
        
        Assert.AreEqual("hello", span.ToString());
    }

    [TestMethod]
    public void ValueMatchSegment_ImplicitConversions_WorkCorrectly()
    {
        var regex = new Regex(@"\w+");
        var input = "hello world";
        
        var match = regex.ValueMatch(input);
        
        // Test implicit conversions
        ReadOnlySpan<char> span = match;
        Assert.AreEqual("hello", span.ToString());
        
        ReadOnlyMemory<char> memory = match;
        Assert.AreEqual("hello", memory.ToString());
    }

    [TestMethod]
    public void ValueMatch_MultipleMatchesInString_ReturnsFirstMatch()
    {
        var regex = new Regex(@"\d+");
        var input = "abc123def456ghi";
        
        var match = regex.ValueMatch(input);
        
        Assert.IsTrue(match.IsMatch);
        Assert.AreEqual("123", match.Match.ToString());
        Assert.AreEqual(3, match.Index);
    }
}
