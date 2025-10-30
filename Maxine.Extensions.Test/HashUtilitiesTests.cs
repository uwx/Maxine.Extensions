using Maxine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class HashUtilitiesTests
{
    [TestMethod]
    public void GetDisplayString_DifferentIndexes_ReturnsDifferentStrings()
    {
        var result1 = HashUtilities.GetDisplayString(1);
        var result2 = HashUtilities.GetDisplayString(2);
        Assert.AreNotEqual(result1, result2);
    }

    [TestMethod]
    public void GetDisplayString_SameIndex_ReturnsSameString()
    {
        var result1 = HashUtilities.GetDisplayString(42);
        var result2 = HashUtilities.GetDisplayString(42);
        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void GetRealIndex_ValidDisplayString_ReturnsOriginalIndex()
    {
        var displayString = HashUtilities.GetDisplayString(123);
        var realIndex = HashUtilities.GetRealIndex(displayString);
        Assert.AreEqual(123, realIndex);
    }

    [TestMethod]
    public void GetRealIndex_RoundTrip_PreservesValue()
    {
        for (int i = 0; i < 100; i++)
        {
            var display = HashUtilities.GetDisplayString(i);
            var decoded = HashUtilities.GetRealIndex(display);
            Assert.AreEqual(i, decoded);
        }
    }

    [TestMethod]
    public void GetRealIndex_InvalidString_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            HashUtilities.GetRealIndex("INVALID!");
        });
    }

    [TestMethod]
    public void TryGetRealIndex_ValidString_ReturnsTrue()
    {
        var displayString = HashUtilities.GetDisplayString(456);
        var success = HashUtilities.TryGetRealIndex(displayString, out var value);
        Assert.IsTrue(success);
        Assert.AreEqual(456, value);
    }

    [TestMethod]
    public void TryGetRealIndex_InvalidString_ReturnsFalse()
    {
        var success = HashUtilities.TryGetRealIndex("!@#$", out var value);
        Assert.IsFalse(success);
        Assert.AreEqual(0, value);
    }

    [TestMethod]
    public void TryGetRealIndex_EmptyString_ReturnsFalse()
    {
        var success = HashUtilities.TryGetRealIndex("", out var value);
        Assert.IsFalse(success);
    }
}

