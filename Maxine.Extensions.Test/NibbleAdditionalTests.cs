namespace Maxine.Extensions.Test;

[TestClass]
public class NibbleAdditionalTests
{
    [TestMethod]
    public void Nibble_IConvertible_GetTypeCode_ReturnsCorrectValue()
    {
        IConvertible nibble = new Nibble(42);
        var typeCode = nibble.GetTypeCode();
        
        Assert.AreEqual(TypeCode.Byte, typeCode);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToBoolean_WorksCorrectly()
    {
        IConvertible nibbleZero = new Nibble(0);
        IConvertible nibbleNonZero = new Nibble(1);
        
        Assert.IsFalse(nibbleZero.ToBoolean(null));
        Assert.IsTrue(nibbleNonZero.ToBoolean(null));
    }

    [TestMethod]
    public void Nibble_IConvertible_ToChar_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(65);
        var result = nibble.ToChar(null);
        
        Assert.AreEqual('A', result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToInt16_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToInt16(null);
        
        Assert.AreEqual((short)42, result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToInt64_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToInt64(null);
        
        Assert.AreEqual(42L, result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToSByte_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToSByte(null);
        
        Assert.AreEqual((sbyte)42, result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToSingle_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToSingle(null);
        
        Assert.AreEqual(42.0f, result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToDouble_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToDouble(null);
        
        Assert.AreEqual(42.0, result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToDecimal_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToDecimal(null);
        
        Assert.AreEqual(42m, result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToString_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToString(null);
        
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToUInt16_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToUInt16(null);
        
        Assert.AreEqual((ushort)42, result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToUInt32_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToUInt32(null);
        
        Assert.AreEqual(42u, result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToUInt64_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToUInt64(null);
        
        Assert.AreEqual(42ul, result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToType_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToType(typeof(int), null);
        
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void Nibble_IConvertible_ToDateTime_ThrowsException()
    {
        IConvertible nibble = new Nibble(42);
        Assert.Throws<InvalidCastException>(() => nibble.ToDateTime(null));
    }

    [TestMethod]
    public void Nibble_IComparable_CompareTo_WorksCorrectly()
    {
        var nibble1 = new Nibble(5);
        IComparable comparable = nibble1;
        
        // IComparable.CompareTo expects byte not Nibble, as it delegates to byte.CompareTo
        Assert.IsGreaterThan(0, comparable.CompareTo((byte)3));
        Assert.IsLessThan(0, comparable.CompareTo((byte)7));
        Assert.AreEqual(0, comparable.CompareTo((byte)5));
    }

    [TestMethod]
    public void Nibble_Equals_WithByte_WorksCorrectly()
    {
        var nibble = new Nibble(42);
        
        Assert.IsTrue(nibble.Equals((byte)42));
        Assert.IsFalse(nibble.Equals((byte)43));
    }

    [TestMethod]
    public void Nibble_CompareTo_WithByte_WorksCorrectly()
    {
        var nibble = new Nibble(42);
        
        Assert.AreEqual(0, nibble.CompareTo((byte)42));
        Assert.IsLessThan(0, nibble.CompareTo((byte)50));
        Assert.IsGreaterThan(0, nibble.CompareTo((byte)30));
    }

    [TestMethod]
    public void Nibble_BitIndexer_MultipleOperations_WorkCorrectly()
    {
        var nibble = new Nibble(0b1111);
        
        nibble[0] = false;
        nibble[1] = false;
        
        Assert.AreEqual((byte)0b1100, nibble.Value);
        Assert.IsFalse(nibble[0]);
        Assert.IsFalse(nibble[1]);
        Assert.IsTrue(nibble[2]);
        Assert.IsTrue(nibble[3]);
    }

    [TestMethod]
    public void Nibble_Equals_WithObject_DifferentType_ReturnsFalse()
    {
        var nibble = new Nibble(42);
        
        Assert.IsFalse(nibble.Equals("42"));
        Assert.IsFalse(nibble.Equals((object)42));
        Assert.IsFalse(nibble.Equals(null));
    }

    [TestMethod]
    public void Nibble_AdditiveIdentity_ReturnsZero()
    {
        // Test via methods that use the identity
        var nibble = new Nibble(5);
        var result = nibble + (Nibble)0;
        
        Assert.AreEqual((byte)5, result.Value);
    }

    [TestMethod]
    public void Nibble_MultiplicativeIdentity_ReturnsOne()
    {
        // Test via methods that use the identity
        var nibble = new Nibble(5);
        var result = nibble * (Nibble)1;
        
        Assert.AreEqual((byte)5, result.Value);
    }
}
