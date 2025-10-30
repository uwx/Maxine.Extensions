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
    public void Nibble_EqualityOperators_WorkCorrectly()
    {
        var nibble1 = new Nibble(5);
        var nibble2 = new Nibble(5);
        var nibble3 = new Nibble(3);
        
        Assert.IsTrue(nibble1 == nibble2);
        Assert.IsFalse(nibble1 == nibble3);
        Assert.IsTrue(nibble1 != nibble3);
        Assert.IsFalse(nibble1 != nibble2);
    }

    [TestMethod]
    public void Nibble_SupportsComparison()
    {
        var nibble1 = new Nibble(5);
        var nibble2 = new Nibble(3);
        
        Assert.IsGreaterThan(0, nibble1.CompareTo(nibble2));
        Assert.IsLessThan(0, nibble2.CompareTo(nibble1));
        Assert.AreEqual(0, nibble1.CompareTo(new Nibble(5)));
    }
    
    [TestMethod]
    public void Nibble_ComparisonOperators_WorkCorrectly()
    {
        var nibble1 = new Nibble(5);
        var nibble2 = new Nibble(3);
        
        Assert.IsTrue(nibble1 > nibble2);
        Assert.IsTrue(nibble1 >= nibble2);
        Assert.IsTrue(nibble2 < nibble1);
        Assert.IsTrue(nibble2 <= nibble1);
        Assert.IsTrue(nibble1 >= new Nibble(5));
        Assert.IsTrue(nibble1 <= new Nibble(5));
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
    
    [TestMethod]
    public void Nibble_MultiplicationOperator_WorksCorrectly()
    {
        var nibble1 = new Nibble(3);
        var nibble2 = new Nibble(4);
        var result = nibble1 * nibble2;
        
        Assert.AreEqual((byte)12, result.Value);
    }
    
    [TestMethod]
    public void Nibble_DivisionOperator_WorksCorrectly()
    {
        var nibble1 = new Nibble(12);
        var nibble2 = new Nibble(3);
        var result = nibble1 / nibble2;
        
        Assert.AreEqual((byte)4, result.Value);
    }
    
    [TestMethod]
    public void Nibble_ModulusOperator_WorksCorrectly()
    {
        var nibble1 = new Nibble(13);
        var nibble2 = new Nibble(5);
        var result = nibble1 % nibble2;
        
        Assert.AreEqual((byte)3, result.Value);
    }
    
    [TestMethod]
    public void Nibble_BitwiseAndOperator_WorksCorrectly()
    {
        var nibble1 = new Nibble(0b1101);
        var nibble2 = new Nibble(0b1011);
        var result = nibble1 & nibble2;
        
        Assert.AreEqual((byte)0b1001, result.Value);
    }
    
    [TestMethod]
    public void Nibble_BitwiseOrOperator_WorksCorrectly()
    {
        var nibble1 = new Nibble(0b1100);
        var nibble2 = new Nibble(0b0011);
        var result = nibble1 | nibble2;
        
        Assert.AreEqual((byte)0b1111, result.Value);
    }
    
    [TestMethod]
    public void Nibble_BitwiseXorOperator_WorksCorrectly()
    {
        var nibble1 = new Nibble(0b1100);
        var nibble2 = new Nibble(0b1010);
        var result = nibble1 ^ nibble2;
        
        Assert.AreEqual((byte)0b0110, result.Value);
    }
    
    [TestMethod]
    public void Nibble_BitwiseNotOperator_WorksCorrectly()
    {
        var nibble = new Nibble(0b1010);
        var result = ~nibble;
        
        Assert.AreEqual(unchecked((byte)~0b1010), result.Value);
    }
    
    [TestMethod]
    public void Nibble_LeftShiftOperator_WorksCorrectly()
    {
        var nibble = new Nibble(0b0011);
        var result = nibble << 2;
        
        Assert.AreEqual((byte)0b1100, result.Value);
    }
    
    [TestMethod]
    public void Nibble_RightShiftOperator_WorksCorrectly()
    {
        var nibble = new Nibble(0b1100);
        var result = nibble >> 2;
        
        Assert.AreEqual((byte)0b0011, result.Value);
    }
    
    [TestMethod]
    public void Nibble_UnsignedRightShiftOperator_WorksCorrectly()
    {
        var nibble = new Nibble(0b1100);
        var result = nibble >>> 2;
        
        Assert.AreEqual((byte)0b0011, result.Value);
    }
    
    [TestMethod]
    public void Nibble_IncrementOperator_WorksCorrectly()
    {
        var nibble = new Nibble(5);
        var result = ++nibble;
        
        Assert.AreEqual((byte)6, result.Value);
    }
    
    [TestMethod]
    public void Nibble_DecrementOperator_WorksCorrectly()
    {
        var nibble = new Nibble(5);
        var result = --nibble;
        
        Assert.AreEqual((byte)4, result.Value);
    }
    
    [TestMethod]
    public void Nibble_UnaryPlusOperator_WorksCorrectly()
    {
        var nibble = new Nibble(5);
        var result = +nibble;
        
        Assert.AreEqual((byte)5, result.Value);
    }
    
    [TestMethod]
    public void Nibble_ImplicitConversionFromByte_WorksCorrectly()
    {
        Nibble nibble = (byte)7;
        
        Assert.AreEqual((byte)7, nibble.Value);
    }
    
    [TestMethod]
    public void Nibble_ImplicitConversionToByte_WorksCorrectly()
    {
        var nibble = new Nibble(7);
        byte value = nibble;
        
        Assert.AreEqual((byte)7, value);
    }
    
    [TestMethod]
    public void Nibble_Parse_WorksCorrectly()
    {
        var nibble = Nibble.Parse("42", null);
        
        Assert.AreEqual((byte)42, nibble.Value);
    }
    
    [TestMethod]
    public void Nibble_TryParse_ValidString_ReturnsTrue()
    {
        var success = Nibble.TryParse("42", null, out var nibble);
        
        Assert.IsTrue(success);
        Assert.AreEqual((byte)42, nibble.Value);
    }
    
    [TestMethod]
    public void Nibble_TryParse_InvalidString_ReturnsFalse()
    {
        var success = Nibble.TryParse("invalid", null, out var nibble);
        
        Assert.IsFalse(success);
    }
    
    [TestMethod]
    public void Nibble_ParseSpan_WorksCorrectly()
    {
        ReadOnlySpan<char> span = "42".AsSpan();
        var nibble = Nibble.Parse(span, null);
        
        Assert.AreEqual((byte)42, nibble.Value);
    }
    
    [TestMethod]
    public void Nibble_TryParseSpan_ValidSpan_ReturnsTrue()
    {
        ReadOnlySpan<char> span = "42".AsSpan();
        var success = Nibble.TryParse(span, null, out var nibble);
        
        Assert.IsTrue(success);
        Assert.AreEqual((byte)42, nibble.Value);
    }
    
    [TestMethod]
    public void Nibble_ToString_ReturnsCorrectString()
    {
        var nibble = new Nibble(5);
        var result = nibble.ToString();
        
        Assert.Contains("5", result);
    }
    
    [TestMethod]
    public void Nibble_ToStringWithFormat_WorksCorrectly()
    {
        var nibble = new Nibble(15);
        var result = nibble.ToString("X", null);
        
        Assert.AreEqual("F", result);
    }
    
    [TestMethod]
    public void Nibble_TryFormat_WorksCorrectly()
    {
        var nibble = new Nibble(42);
        Span<char> buffer = stackalloc char[10];
        
        var success = nibble.TryFormat(buffer, out var charsWritten, default, null);
        
        Assert.IsTrue(success);
        Assert.IsGreaterThan(0, charsWritten);
    }
    
    [TestMethod]
    public void Nibble_TryFormatUtf8_WorksCorrectly()
    {
        var nibble = new Nibble(42);
        Span<byte> buffer = stackalloc byte[10];
        
        var success = nibble.TryFormat(buffer, out var bytesWritten, default, null);
        
        Assert.IsTrue(success);
        Assert.IsGreaterThan(0, bytesWritten);
    }
    
    [TestMethod]
    public void Nibble_GetHashCode_WorksCorrectly()
    {
        var nibble1 = new Nibble(5);
        var nibble2 = new Nibble(5);
        var nibble3 = new Nibble(3);
        
        Assert.AreEqual(nibble1.GetHashCode(), nibble2.GetHashCode());
        Assert.AreNotEqual(nibble1.GetHashCode(), nibble3.GetHashCode());
    }
    
    [TestMethod]
    public void Nibble_MinMaxValue_AreCorrect()
    {
        Assert.AreEqual((byte)0, Nibble.MinValue.Value);
        Assert.AreEqual(byte.MaxValue, Nibble.MaxValue.Value);
    }
    
    [TestMethod]
    public void Nibble_IConvertible_ToByte_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToByte(null);
        
        Assert.AreEqual((byte)42, result);
    }
    
    [TestMethod]
    public void Nibble_IConvertible_ToInt32_WorksCorrectly()
    {
        IConvertible nibble = new Nibble(42);
        var result = nibble.ToInt32(null);
        
        Assert.AreEqual(42, result);
    }
    
    [TestMethod]
    public void Nibble_BitIndexer_Get_WorksCorrectly()
    {
        var nibble = new Nibble(0b1010);
        
        Assert.IsFalse(nibble[0]); // LSB
        Assert.IsTrue(nibble[1]);
        Assert.IsFalse(nibble[2]);
        Assert.IsTrue(nibble[3]);
    }
    
    [TestMethod]
    public void Nibble_BitIndexer_Set_WorksCorrectly()
    {
        var nibble = new Nibble(0b0000);
        
        nibble[1] = true;
        nibble[3] = true;
        
        Assert.AreEqual((byte)0b1010, nibble.Value);
    }
}

