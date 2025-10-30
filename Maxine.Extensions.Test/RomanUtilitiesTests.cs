namespace Maxine.Extensions.Test;

[TestClass]
public class RomanUtilitiesTests
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
    public void ToRomanNumeral_FiveHundred_ReturnsD()
    {
        var result = RomanUtilities.ToRomanNumeral(500);
        Assert.AreEqual("D", result);
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
    public void ToRomanNumeral_Zero_ReturnsEmptyString()
    {
        var result = RomanUtilities.ToRomanNumeral(0);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ToRomanNumeral_Negative_ReturnsEmptyString()
    {
        var result = RomanUtilities.ToRomanNumeral(-5);
        Assert.AreEqual(string.Empty, result);
    }
}

