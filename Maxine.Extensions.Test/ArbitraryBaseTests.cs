namespace Maxine.Extensions.Test;

[TestClass]
public class ArbitraryBaseTests
{
    [TestMethod]
    public void Constructor_WithBaseDigits_CreatesConverter()
    {
        var converter = new ArbitraryBaseConverter<int, uint>("0123456789ABCDEF");
        Assert.IsNotNull(converter);
    }

    [TestMethod]
    public void ToBase_Zero_ReturnsFirstDigit()
    {
        var converter = new ArbitraryBaseConverter<int, uint>("0123456789");
        var result = converter.ToBase(0);
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void ToBase_PositiveNumber_ConvertsCorrectly()
    {
        var converter = new ArbitraryBaseConverter<int, uint>("01");
        var result = converter.ToBase(5); // 5 in binary is 101
        Assert.AreEqual("101", result);
    }

    [TestMethod]
    public void ToBase_HexadecimalConversion_WorksCorrectly()
    {
        var converter = new ArbitraryBaseConverter<int, uint>("0123456789ABCDEF");
        var result = converter.ToBase(255); // 255 in hex is FF
        Assert.AreEqual("FF", result);
    }

    [TestMethod]
    public void FromBase_Zero_ReturnsZero()
    {
        var converter = new ArbitraryBaseConverter<int, uint>("0123456789");
        var result = converter.FromBase("0");
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void FromBase_ValidString_ConvertsCorrectly()
    {
        var converter = new ArbitraryBaseConverter<int, uint>("01");
        var result = converter.FromBase("101"); // 101 in binary is 5
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void FromBase_HexadecimalConversion_WorksCorrectly()
    {
        var converter = new ArbitraryBaseConverter<int, uint>("0123456789ABCDEF");
        var result = converter.FromBase("FF"); // FF in hex is 255
        Assert.AreEqual(255, result);
    }

    [TestMethod]
    public void RoundTrip_PreservesValue()
    {
        var converter = new ArbitraryBaseConverter<int, uint>("0123456789ABCDEF");
        int original = 12345;
        var encoded = converter.ToBase(original);
        var decoded = converter.FromBase(encoded);
        Assert.AreEqual(original, decoded);
    }
}

