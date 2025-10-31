using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class HashUtilitiesAdditionalTests
{
    [TestMethod]
    public void GetDisplayString_ZeroIndex_ReturnsValidString()
    {
        var result = HashUtilities.GetDisplayString(0);
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length >= 4); // Can be longer with padding
    }

    [TestMethod]
    public void GetDisplayString_PositiveIndex_ReturnsValidString()
    {
        var result = HashUtilities.GetDisplayString(12345);
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length >= 4);
    }

    [TestMethod]
    public void GetDisplayString_NegativeIndex_ReturnsValidString()
    {
        var result = HashUtilities.GetDisplayString(-1);
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length >= 4);
    }

    [TestMethod]
    public void GetDisplayString_MaxValue_ReturnsValidString()
    {
        var result = HashUtilities.GetDisplayString(int.MaxValue);
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length >= 4);
    }

    [TestMethod]
    public void GetDisplayString_DifferentValues_ReturnsDifferentStrings()
    {
        var result1 = HashUtilities.GetDisplayString(1);
        var result2 = HashUtilities.GetDisplayString(2);
        
        Assert.AreNotEqual(result1, result2);
    }

    [TestMethod]
    public void GetRealIndex_ValidDisplayString_ReturnsCorrectIndex()
    {
        var displayString = HashUtilities.GetDisplayString(42);
        var realIndex = HashUtilities.GetRealIndex(displayString);
        
        Assert.AreEqual(42, realIndex);
    }

    [TestMethod]
    public void GetRealIndex_RoundTrip_PreservesValue()
    {
        for (int i = 0; i < 100; i++)
        {
            var display = HashUtilities.GetDisplayString(i);
            var recovered = HashUtilities.GetRealIndex(display);
            Assert.AreEqual(i, recovered, $"Failed for index {i}");
        }
    }

    [TestMethod]
    public void GetRealIndex_InvalidCharacter_ThrowsArgumentException()
    {
        try
        {
            HashUtilities.GetRealIndex("!!!!");
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void GetRealIndex_EmptyString_ThrowsArgumentException()
    {
        try
        {
            HashUtilities.GetRealIndex(string.Empty);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void TryGetRealIndex_ValidDisplayString_ReturnsTrue()
    {
        var displayString = HashUtilities.GetDisplayString(100);
        var success = HashUtilities.TryGetRealIndex(displayString, out var value);
        
        Assert.IsTrue(success);
        Assert.AreEqual(100, value);
    }

    [TestMethod]
    public void TryGetRealIndex_InvalidCharacter_ReturnsFalse()
    {
        var success = HashUtilities.TryGetRealIndex("!!!!", out var value);
        
        Assert.IsFalse(success);
        Assert.AreEqual(0, value);
    }

    [TestMethod]
    public void TryGetRealIndex_EmptyString_ReturnsFalse()
    {
        var success = HashUtilities.TryGetRealIndex(string.Empty, out var value);
        
        Assert.IsFalse(success);
        Assert.AreEqual(0, value);
    }

    [TestMethod]
    public void TryGetRealIndex_RoundTrip_WorksCorrectly()
    {
        for (int i = -100; i < 100; i++)
        {
            var display = HashUtilities.GetDisplayString(i);
            var success = HashUtilities.TryGetRealIndex(display, out var recovered);
            Assert.IsTrue(success, $"TryGetRealIndex failed for index {i}");
            Assert.AreEqual(i, recovered, $"Value mismatch for index {i}");
        }
    }

    [TestMethod]
    public void GetDisplayString_Consistency_SameInputProducesSameOutput()
    {
        var result1 = HashUtilities.GetDisplayString(999);
        var result2 = HashUtilities.GetDisplayString(999);
        
        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void TryGetRealIndex_NullString_ReturnsFalse()
    {
        var success = HashUtilities.TryGetRealIndex(null!, out var value);
        
        Assert.IsFalse(success);
        Assert.AreEqual(0, value);
    }
}
