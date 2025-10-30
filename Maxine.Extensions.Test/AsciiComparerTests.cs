namespace Maxine.Extensions.Test;

[TestClass]
public class AsciiComparerTests
{
    [TestMethod]
    public void Instance_ReturnsSingleton()
    {
        var instance1 = AsciiComparer.Instance;
        var instance2 = AsciiComparer.Instance;
        Assert.AreSame(instance1, instance2);
    }

    [TestMethod]
    public void Equals_SameAsciiBytes_ReturnsTrue()
    {
        var comparer = new AsciiComparer();
        Assert.IsTrue(comparer.Equals((byte)'A', (byte)'A'));
        Assert.IsTrue(comparer.Equals((byte)'z', (byte)'z'));
        Assert.IsTrue(comparer.Equals((byte)'5', (byte)'5'));
    }

    [TestMethod]
    public void Equals_UppercaseAndLowercase_ReturnsTrue()
    {
        var comparer = new AsciiComparer();
        Assert.IsTrue(comparer.Equals((byte)'A', (byte)'a'));
        Assert.IsTrue(comparer.Equals((byte)'Z', (byte)'z'));
        Assert.IsTrue(comparer.Equals((byte)'M', (byte)'m'));
    }

    [TestMethod]
    public void Equals_DifferentLetters_ReturnsFalse()
    {
        var comparer = new AsciiComparer();
        Assert.IsFalse(comparer.Equals((byte)'A', (byte)'B'));
        Assert.IsFalse(comparer.Equals((byte)'a', (byte)'z'));
    }

    [TestMethod]
    public void Equals_NonAsciiBytes_ReturnsFalse()
    {
        var comparer = new AsciiComparer();
        Assert.IsFalse(comparer.Equals(200, 200)); // Non-ASCII values
        Assert.IsFalse(comparer.Equals(128, 130));
    }

    [TestMethod]
    public void GetHashCode_SameLetterDifferentCase_ReturnsSameHash()
    {
        var comparer = new AsciiComparer();
        var hash1 = comparer.GetHashCode((byte)'A');
        var hash2 = comparer.GetHashCode((byte)'a');
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetHashCode_NonLetter_ReturnsOriginalValue()
    {
        var comparer = new AsciiComparer();
        var hash = comparer.GetHashCode((byte)'5');
        Assert.AreEqual((byte)'5', hash);
    }
}

