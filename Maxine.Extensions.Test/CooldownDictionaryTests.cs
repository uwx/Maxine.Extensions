using Maxine.Extensions.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class CooldownDictionaryTests
{
    [TestMethod]
    public void CooldownDictionary_Constructor_CreatesInstance()
    {
        var dict = new CooldownDictionary<string>(TimeSpan.FromSeconds(1));
        Assert.IsNotNull(dict);
    }

    [TestMethod]
    public void CheckOrUpdateCooldown_FirstTime_ReturnsTrue()
    {
        var dict = new CooldownDictionary<string>(TimeSpan.FromSeconds(10));
        
        var result = dict.CheckOrUpdateCooldown("key1");
        
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckOrUpdateCooldown_ImmediatelyAfterFirst_ReturnsFalse()
    {
        var dict = new CooldownDictionary<string>(TimeSpan.FromSeconds(10));
        
        dict.CheckOrUpdateCooldown("key1");
        var result = dict.CheckOrUpdateCooldown("key1");
        
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task CheckOrUpdateCooldown_AfterCooldownExpires_ReturnsTrue()
    {
        var dict = new CooldownDictionary<string>(TimeSpan.FromMilliseconds(100));
        
        dict.CheckOrUpdateCooldown("key1");
        Assert.IsFalse(dict.CheckOrUpdateCooldown("key1"));
        
        await Task.Delay(150);
        
        var result = dict.CheckOrUpdateCooldown("key1");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckOrUpdateCooldown_DifferentKeys_AreIndependent()
    {
        var dict = new CooldownDictionary<string>(TimeSpan.FromSeconds(10));
        
        var result1 = dict.CheckOrUpdateCooldown("key1");
        var result2 = dict.CheckOrUpdateCooldown("key2");
        
        Assert.IsTrue(result1);
        Assert.IsTrue(result2);
    }

    [TestMethod]
    public void CheckOrUpdateCooldown_SameKey_MultipleAttempts_OnlyFirstPasses()
    {
        var dict = new CooldownDictionary<string>(TimeSpan.FromSeconds(10));
        
        Assert.IsTrue(dict.CheckOrUpdateCooldown("key1"));
        Assert.IsFalse(dict.CheckOrUpdateCooldown("key1"));
        Assert.IsFalse(dict.CheckOrUpdateCooldown("key1"));
    }
}

