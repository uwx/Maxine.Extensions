namespace Maxine.Extensions.Test;

[TestClass]
public class Skip32CipherAdditionalTests
{
    [TestMethod]
    public void Constructor_NullKey_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new Skip32Cipher(null!));
    }

    [TestMethod]
    public void Constructor_KeyTooShort_ThrowsException()
    {
        var key = new byte[5]; // Should be 10
        Assert.Throws<ArgumentOutOfRangeException>(() => new Skip32Cipher(key));
    }

    [TestMethod]
    public void Constructor_KeyTooLong_ThrowsException()
    {
        var key = new byte[15]; // Should be 10
        Assert.Throws<ArgumentOutOfRangeException>(() => new Skip32Cipher(key));
    }

    [TestMethod]
    public void EncryptInPlace_ByteArray_WorksCorrectly()
    {
        var key = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var cipher = new Skip32Cipher(key);
        
        Span<byte> data = stackalloc byte[4] { 0x12, 0x34, 0x56, 0x78 };
        var original = data.ToArray();
        
        cipher.EncryptInPlace(data);
        
        // Should be modified
        Assert.IsFalse(data.SequenceEqual(original));
    }

    [TestMethod]
    public void DecryptInPlace_ByteArray_WorksCorrectly()
    {
        var key = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var cipher = new Skip32Cipher(key);
        
        Span<byte> data = stackalloc byte[4] { 0x12, 0x34, 0x56, 0x78 };
        var original = data.ToArray();
        
        cipher.EncryptInPlace(data);
        cipher.DecryptInPlace(data);
        
        // Should be back to original
        Assert.IsTrue(data.SequenceEqual(original));
    }

    [TestMethod]
    public void EncryptInPlace_InvalidSize_ThrowsException()
    {
        var key = new byte[10];
        var cipher = new Skip32Cipher(key);
        
        Span<byte> data = stackalloc byte[3]; // Wrong size
        try
        {
            cipher.EncryptInPlace(data);
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void DecryptInPlace_InvalidSize_ThrowsException()
    {
        var key = new byte[10];
        var cipher = new Skip32Cipher(key);
        
        Span<byte> data = stackalloc byte[5]; // Wrong size
        try
        {
            cipher.DecryptInPlace(data);
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Encrypt_GenericInt32_WorksCorrectly()
    {
        var key = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var cipher = new Skip32Cipher(key);
        
        int value = 123456;
        var encrypted = cipher.Encrypt(value);
        
        Assert.AreNotEqual(value, encrypted);
    }

    [TestMethod]
    public void Decrypt_GenericInt32_WorksCorrectly()
    {
        var key = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var cipher = new Skip32Cipher(key);
        
        int value = 123456;
        var encrypted = cipher.Encrypt(value);
        var decrypted = cipher.Decrypt(encrypted);
        
        Assert.AreEqual(value, decrypted);
    }

    [TestMethod]
    public void Encrypt_GenericUInt32_WorksCorrectly()
    {
        var key = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var cipher = new Skip32Cipher(key);
        
        uint value = 4294967295u; // Max uint
        var encrypted = cipher.Encrypt(value);
        var decrypted = cipher.Decrypt(encrypted);
        
        Assert.AreEqual(value, decrypted);
    }

    [TestMethod]
    public void Encrypt_DifferentKeys_ProduceDifferentResults()
    {
        var key1 = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var key2 = new byte[10] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        
        var cipher1 = new Skip32Cipher(key1);
        var cipher2 = new Skip32Cipher(key2);
        
        uint value = 12345;
        var encrypted1 = cipher1.Encrypt(value);
        var encrypted2 = cipher2.Encrypt(value);
        
        Assert.AreNotEqual(encrypted1, encrypted2);
    }

    [TestMethod]
    public void Encrypt_ZeroValue_WorksCorrectly()
    {
        var key = new byte[10];
        var cipher = new Skip32Cipher(key);
        
        uint value = 0;
        var encrypted = cipher.Encrypt(value);
        var decrypted = cipher.Decrypt(encrypted);
        
        Assert.AreEqual(value, decrypted);
    }

    [TestMethod]
    public void Encrypt_MaxValue_WorksCorrectly()
    {
        var key = new byte[10];
        var cipher = new Skip32Cipher(key);
        
        uint value = uint.MaxValue;
        var encrypted = cipher.Encrypt(value);
        var decrypted = cipher.Decrypt(encrypted);
        
        Assert.AreEqual(value, decrypted);
    }

    [TestMethod]
    public void EncryptInPlace_MultipleOperations_WorksCorrectly()
    {
        var key = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var cipher = new Skip32Cipher(key);
        
        Span<byte> data = stackalloc byte[4] { 0xAA, 0xBB, 0xCC, 0xDD };
        var original = data.ToArray();
        
        // Encrypt twice
        cipher.EncryptInPlace(data);
        cipher.EncryptInPlace(data);
        
        // Decrypt twice
        cipher.DecryptInPlace(data);
        cipher.DecryptInPlace(data);
        
        Assert.IsTrue(data.SequenceEqual(original));
    }
}
