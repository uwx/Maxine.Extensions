namespace Maxine.Extensions.Test;

[TestClass]
public class Skip32CipherTests
{
    [TestMethod]
    public void Constructor_WithKey_CreatesCipher()
    {
        var key = new byte[10];
        var cipher = new Skip32Cipher(key);
        Assert.IsNotNull(cipher);
    }

    [TestMethod]
    public void EncryptDecrypt_RoundTrip_ReturnsOriginal()
    {
        var key = new byte[10];
        for (int i = 0; i < key.Length; i++)
            key[i] = (byte)i;
        
        var cipher = new Skip32Cipher(key);
        uint value = 123456;
        
        var encrypted = cipher.Encrypt(value);
        var decrypted = cipher.Decrypt(encrypted);
        
        Assert.AreEqual(value, decrypted);
    }

    [TestMethod]
    public void Encrypt_DifferentValues_ProducesDifferentResults()
    {
        var key = new byte[10];
        var cipher = new Skip32Cipher(key);
        
        var encrypted1 = cipher.Encrypt(12345);
        var encrypted2 = cipher.Encrypt(54321);
        
        Assert.AreNotEqual(encrypted1, encrypted2);
    }

    [TestMethod]
    public void Encrypt_SameValue_ProducesSameResult()
    {
        var key = new byte[10];
        var cipher = new Skip32Cipher(key);
        
        var encrypted1 = cipher.Encrypt(12345);
        var encrypted2 = cipher.Encrypt(12345);
        
        Assert.AreEqual(encrypted1, encrypted2);
    }
}

