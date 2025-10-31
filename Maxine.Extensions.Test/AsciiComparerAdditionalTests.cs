namespace Maxine.Extensions.Test;

[TestClass]
public class AsciiComparerAdditionalTests
{
    [TestMethod]
    public void SpanComparerExtensions_IndexOf_WithEqualityComparer()
    {
        var comparer = AsciiComparer.Instance;
        ReadOnlySpan<byte> span = "Hello World"u8;
        ReadOnlySpan<byte> search = "world"u8;
        
        var index = span.IndexOf(search, comparer);
        
        Assert.AreEqual(6, index);
    }

    [TestMethod]
    public void SpanComparerExtensions_IndexOf_NotFound_ReturnsMinusOne()
    {
        var comparer = AsciiComparer.Instance;
        ReadOnlySpan<byte> span = "Hello World"u8;
        ReadOnlySpan<byte> search = "xyz"u8;
        
        var index = span.IndexOf(search, comparer);
        
        Assert.AreEqual(-1, index);
    }

    [TestMethod]
    public void SpanComparerExtensions_IndexOf_EmptySearch_ReturnsZero()
    {
        var comparer = AsciiComparer.Instance;
        ReadOnlySpan<byte> span = "Hello"u8;
        ReadOnlySpan<byte> search = ReadOnlySpan<byte>.Empty;
        
        var index = span.IndexOf(search, comparer);
        
        Assert.AreEqual(0, index);
    }

    [TestMethod]
    public void SpanComparerExtensions_IndexOf_Span_WorksCorrectly()
    {
        var comparer = AsciiComparer.Instance;
        Span<byte> span = stackalloc byte[] { (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };
        ReadOnlySpan<byte> search = "LLO"u8;
        
        var index = span.IndexOf(search, comparer);
        
        Assert.AreEqual(2, index);
    }

    [TestMethod]
    public void AsciiComparer_Equals_NonLetterCharacters_WorksCorrectly()
    {
        var comparer = AsciiComparer.Instance;
        
        Assert.IsTrue(comparer.Equals((byte)'1', (byte)'1'));
        Assert.IsFalse(comparer.Equals((byte)'1', (byte)'2'));
        Assert.IsTrue(comparer.Equals((byte)' ', (byte)' '));
        Assert.IsFalse(comparer.Equals((byte)' ', (byte)'-'));
    }

    [TestMethod]
    public void AsciiComparer_GetHashCode_ConsistentForSameInput()
    {
        var comparer = AsciiComparer.Instance;
        
        var hash1 = comparer.GetHashCode((byte)'A');
        var hash2 = comparer.GetHashCode((byte)'A');
        
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void AsciiComparer_GetHashCode_NonAscii_ReturnsOriginalValue()
    {
        var comparer = AsciiComparer.Instance;
        
        var hash = comparer.GetHashCode(200);
        Assert.AreEqual(200, hash);
    }

    [TestMethod]
    public void SpanComparerExtensions_IndexOf_CaseInsensitiveMatch()
    {
        var comparer = AsciiComparer.Instance;
        ReadOnlySpan<byte> span = "The Quick Brown Fox"u8;
        ReadOnlySpan<byte> search = "QUICK"u8;
        
        var index = span.IndexOf(search, comparer);
        
        Assert.AreEqual(4, index);
    }

    [TestMethod]
    public void SpanComparerExtensions_IndexOf_MultipleMatches_ReturnsFirst()
    {
        var comparer = AsciiComparer.Instance;
        ReadOnlySpan<byte> span = "abcabcabc"u8;
        ReadOnlySpan<byte> search = "ABC"u8;
        
        var index = span.IndexOf(search, comparer);
        
        Assert.AreEqual(0, index);
    }

    [TestMethod]
    public void SpanComparerExtensions_IndexOf_SearchLongerThanSpan_ReturnsMinusOne()
    {
        var comparer = AsciiComparer.Instance;
        ReadOnlySpan<byte> span = "Hi"u8;
        ReadOnlySpan<byte> search = "Hello"u8;
        
        var index = span.IndexOf(search, comparer);
        
        Assert.AreEqual(-1, index);
    }

    [TestMethod]
    public void SpanComparerExtensions_IndexOf_EndOfSpan_WorksCorrectly()
    {
        var comparer = AsciiComparer.Instance;
        ReadOnlySpan<byte> span = "Hello World"u8;
        ReadOnlySpan<byte> search = "WORLD"u8;
        
        var index = span.IndexOf(search, comparer);
        
        Assert.AreEqual(6, index);
    }
}
