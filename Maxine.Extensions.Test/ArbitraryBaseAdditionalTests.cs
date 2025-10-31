using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class ArbitraryBaseAdditionalTests
{
    [TestMethod]
    public void B31_ToBase_Zero_ReturnsZeroChar()
    {
        var result = ArbitraryBase.B31.ToBase(0);
        Assert.AreEqual("2", result); // First character in B31 alphabet
    }

    [TestMethod]
    public void B31_ToBase_One_ReturnsFirstNonZeroChar()
    {
        var result = ArbitraryBase.B31.ToBase(1);
        Assert.AreEqual("3", result);
    }

    [TestMethod]
    public void B31_ToBase_WithPadding_PadsCorrectly()
    {
        var result = ArbitraryBase.B31.ToBase(5, 4);
        Assert.IsTrue(result.Length == 4);
        Assert.IsTrue(result.StartsWith("222"));
    }

    [TestMethod]
    public void B31_FromBase_ValidString_ReturnsCorrectValue()
    {
        var encoded = ArbitraryBase.B31.ToBase(12345);
        var decoded = ArbitraryBase.B31.FromBase(encoded);
        Assert.AreEqual(12345u, decoded);
    }

    [TestMethod]
    public void B31_RoundTrip_PreservesValue()
    {
        for (uint i = 0; i < 1000; i++)
        {
            var encoded = ArbitraryBase.B31.ToBase(i);
            var decoded = ArbitraryBase.B31.FromBase(encoded);
            Assert.AreEqual(i, decoded, $"Failed for value {i}");
        }
    }

    [TestMethod]
    public void B31_TryFromBase_ValidString_ReturnsTrue()
    {
        var success = ArbitraryBase.B31.TryFromBase("abc", out var result);
        Assert.IsTrue(success);
        Assert.IsTrue(result > 0);
    }

    [TestMethod]
    public void B31_TryFromBase_InvalidChar_ReturnsFalse()
    {
        var success = ArbitraryBase.B31.TryFromBase("xyz1", out var result);
        Assert.IsFalse(success); // '1' is not in B31 alphabet
        Assert.AreEqual(0u, result);
    }

    [TestMethod]
    public void B31_TryFromBase_EmptyString_ReturnsFalse()
    {
        var success = ArbitraryBase.B31.TryFromBase(string.Empty, out var result);
        Assert.IsFalse(success);
        Assert.AreEqual(0u, result);
    }

    [TestMethod]
    public void B31_TryFromBase_NullString_ReturnsFalse()
    {
        var success = ArbitraryBase.B31.TryFromBase(null!, out var result);
        Assert.IsFalse(success);
        Assert.AreEqual(0u, result);
    }

    [TestMethod]
    public void B31_FromBase_InvalidString_ThrowsException()
    {
        try
        {
            ArbitraryBase.B31.FromBase("0"); // '0' not in B31
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void B93_ToBase_Zero_ReturnsZeroChar()
    {
        var result = ArbitraryBase.B93.ToBase(0);
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void B93_ToBase_SmallNumber_WorksCorrectly()
    {
        var result = ArbitraryBase.B93.ToBase(10);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void B93_RoundTrip_PreservesValue()
    {
        for (int i = 0; i < 500; i++)
        {
            var encoded = ArbitraryBase.B93.ToBase(i);
            var decoded = ArbitraryBase.B93.FromBase(encoded);
            Assert.AreEqual(i, decoded, $"Failed for value {i}");
        }
    }

    [TestMethod]
    public void B93_RoundTrip_NegativeValues_WorksCorrectly()
    {
        for (int i = -100; i < 0; i++)
        {
            var encoded = ArbitraryBase.B93.ToBase(i);
            var decoded = ArbitraryBase.B93.FromBase(encoded);
            Assert.AreEqual(i, decoded, $"Failed for value {i}");
        }
    }

    [TestMethod]
    public void B93_TryFromBase_ValidString_ReturnsTrue()
    {
        var success = ArbitraryBase.B93.TryFromBase("ABC", out var result);
        Assert.IsTrue(success);
    }

    [TestMethod]
    public void B93_ToBase_WithPadding_PadsCorrectly()
    {
        var result = ArbitraryBase.B93.ToBase(5, 5);
        Assert.AreEqual(5, result.Length);
        Assert.IsTrue(result.StartsWith("0000"));
    }

    [TestMethod]
    public void B93_ToBase_LargeNumber_HandlesCorrectly()
    {
        var result = ArbitraryBase.B93.ToBase(int.MaxValue);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void B31_ToBase_LargeNumber_HandlesCorrectly()
    {
        var result = ArbitraryBase.B31.ToBase(uint.MaxValue);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void B31_Consistency_SameInputGivesSameOutput()
    {
        var result1 = ArbitraryBase.B31.ToBase(9999);
        var result2 = ArbitraryBase.B31.ToBase(9999);
        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void B93_Consistency_SameInputGivesSameOutput()
    {
        var result1 = ArbitraryBase.B93.ToBase(12345);
        var result2 = ArbitraryBase.B93.ToBase(12345);
        Assert.AreEqual(result1, result2);
    }
}
