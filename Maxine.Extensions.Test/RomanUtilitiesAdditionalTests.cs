using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class RomanUtilitiesAdditionalTests
{
    [TestMethod]
    public void ToRomanNumeral_One_ReturnsI()
    {
        var result = RomanUtilities.ToRomanNumeral(1);
        Assert.AreEqual("I", result);
    }

    [TestMethod]
    public void ToRomanNumeral_Four_ReturnsIV()
    {
        var result = RomanUtilities.ToRomanNumeral(4);
        Assert.AreEqual("IV", result);
    }

    [TestMethod]
    public void ToRomanNumeral_Five_ReturnsV()
    {
        var result = RomanUtilities.ToRomanNumeral(5);
        Assert.AreEqual("V", result);
    }

    [TestMethod]
    public void ToRomanNumeral_Nine_ReturnsIX()
    {
        var result = RomanUtilities.ToRomanNumeral(9);
        Assert.AreEqual("IX", result);
    }

    [TestMethod]
    public void ToRomanNumeral_Ten_ReturnsX()
    {
        var result = RomanUtilities.ToRomanNumeral(10);
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void ToRomanNumeral_Forty_ReturnsXL()
    {
        var result = RomanUtilities.ToRomanNumeral(40);
        Assert.AreEqual("XL", result);
    }

    [TestMethod]
    public void ToRomanNumeral_Fifty_ReturnsL()
    {
        var result = RomanUtilities.ToRomanNumeral(50);
        Assert.AreEqual("L", result);
    }

    [TestMethod]
    public void ToRomanNumeral_Ninety_ReturnsXC()
    {
        var result = RomanUtilities.ToRomanNumeral(90);
        Assert.AreEqual("XC", result);
    }

    [TestMethod]
    public void ToRomanNumeral_OneHundred_ReturnsC()
    {
        var result = RomanUtilities.ToRomanNumeral(100);
        Assert.AreEqual("C", result);
    }

    [TestMethod]
    public void ToRomanNumeral_FourHundred_ReturnsCD()
    {
        var result = RomanUtilities.ToRomanNumeral(400);
        Assert.AreEqual("CD", result);
    }

    [TestMethod]
    public void ToRomanNumeral_FiveHundred_ReturnsD()
    {
        var result = RomanUtilities.ToRomanNumeral(500);
        Assert.AreEqual("D", result);
    }

    [TestMethod]
    public void ToRomanNumeral_NineHundred_ReturnsCM()
    {
        var result = RomanUtilities.ToRomanNumeral(900);
        Assert.AreEqual("CM", result);
    }

    [TestMethod]
    public void ToRomanNumeral_OneThousand_ReturnsM()
    {
        var result = RomanUtilities.ToRomanNumeral(1000);
        Assert.AreEqual("M", result);
    }

    [TestMethod]
    public void ToRomanNumeral_1994_ReturnsMCMXCIV()
    {
        var result = RomanUtilities.ToRomanNumeral(1994);
        Assert.AreEqual("MCMXCIV", result);
    }

    [TestMethod]
    public void ToRomanNumeral_2024_ReturnsMMXXIV()
    {
        var result = RomanUtilities.ToRomanNumeral(2024);
        Assert.AreEqual("MMXXIV", result);
    }

    [TestMethod]
    public void ToRomanNumeral_3999_ReturnsMMMCMXCIX()
    {
        var result = RomanUtilities.ToRomanNumeral(3999);
        Assert.AreEqual("MMMCMXCIX", result);
    }

    [TestMethod]
    public void ToRomanNumeral_27_ReturnsXXVII()
    {
        var result = RomanUtilities.ToRomanNumeral(27);
        Assert.AreEqual("XXVII", result);
    }

    [TestMethod]
    public void ToRomanNumeral_444_ReturnsCDXLIV()
    {
        var result = RomanUtilities.ToRomanNumeral(444);
        Assert.AreEqual("CDXLIV", result);
    }

    [TestMethod]
    public void ToRomanNumeral_Zero_ReturnsEmpty()
    {
        var result = RomanUtilities.ToRomanNumeral(0);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ToRomanNumeral_NegativeNumber_ReturnsEmpty()
    {
        var result = RomanUtilities.ToRomanNumeral(-5);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ToRomanNumeral_3888_ReturnsMMM()
    {
        var result = RomanUtilities.ToRomanNumeral(3888);
        Assert.AreEqual("MMMDCCCLXXXVIII", result);
    }

    [TestMethod]
    public void ToRomanNumeral_58_ReturnsLVIII()
    {
        var result = RomanUtilities.ToRomanNumeral(58);
        Assert.AreEqual("LVIII", result);
    }

    [TestMethod]
    public void ToRomanNumeral_1776_ReturnsMDCCLXXVI()
    {
        var result = RomanUtilities.ToRomanNumeral(1776);
        Assert.AreEqual("MDCCLXXVI", result);
    }

    [TestMethod]
    public void ToRomanNumeral_2999_ReturnsMMCMXCIX()
    {
        var result = RomanUtilities.ToRomanNumeral(2999);
        Assert.AreEqual("MMCMXCIX", result);
    }

    [TestMethod]
    public void ToRomanNumeral_3_ReturnsIII()
    {
        var result = RomanUtilities.ToRomanNumeral(3);
        Assert.AreEqual("III", result);
    }
}
