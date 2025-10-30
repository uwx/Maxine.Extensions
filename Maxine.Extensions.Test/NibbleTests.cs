namespace Maxine.Extensions.Test;

[TestClass]
public class NibbleTests
{
    [TestMethod]
    public void Constructor_WithByte_CreatesNibble()
    {
        var nibble = new Nibble(5);
        Assert.AreEqual((byte)5, nibble.Value);
    }

    [TestMethod]
    public void Value_GetSet_WorksCorrectly()
    {
        var nibble = new Nibble(3);
        Assert.AreEqual((byte)3, nibble.Value);
        
        nibble.Value = 7;
        Assert.AreEqual((byte)7, nibble.Value);
    }

    [TestMethod]
    public void Nibble_SupportsEquality()
    {
        var nibble1 = new Nibble(5);
        var nibble2 = new Nibble(5);
        var nibble3 = new Nibble(3);
        
        Assert.IsTrue(nibble1.Equals(nibble2));
        Assert.IsFalse(nibble1.Equals(nibble3));
    }

    [TestMethod]
    public void Nibble_SupportsComparison()
    {
        var nibble1 = new Nibble(5);
        var nibble2 = new Nibble(3);
        
        Assert.IsGreaterThan(0, nibble1.CompareTo(nibble2));
        Assert.IsLessThan(0, nibble2.CompareTo(nibble1));
    }

    [TestMethod]
    public void Nibble_AdditionOperator_WorksCorrectly()
    {
        var nibble1 = new Nibble(3);
        var nibble2 = new Nibble(2);
        var result = nibble1 + nibble2;
        
        Assert.AreEqual((byte)5, result.Value);
    }

    [TestMethod]
    public void Nibble_SubtractionOperator_WorksCorrectly()
    {
        var nibble1 = new Nibble(5);
        var nibble2 = new Nibble(2);
        var result = nibble1 - nibble2;
        
        Assert.AreEqual((byte)3, result.Value);
    }
}

