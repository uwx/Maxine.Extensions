using Maxine.Extensions.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Caching.Memory;

namespace Maxine.Extensions.Test;

[TestClass]
public class MemoryCacheTests
{
    [TestMethod]
    public void MemoryCache_Constructor_CreatesInstance()
    {
        var cache = new MemoryCache<string, int>(new MemoryCacheOptions());
        Assert.IsNotNull(cache);
    }

    [TestMethod]
    public void MemoryCache_Set_StoresValue()
    {
        var cache = new MemoryCache<string, int>(new MemoryCacheOptions());
        cache.Set("key1", 42);
        
        var success = cache.TryGetValue("key1", out var value);
        Assert.IsTrue(success);
        Assert.AreEqual(42, value);
    }

    [TestMethod]
    public void MemoryCache_TryGetValue_NonExistentKey_ReturnsFalse()
    {
        var cache = new MemoryCache<string, int>(new MemoryCacheOptions());
        
        var success = cache.TryGetValue("nonexistent", out var value);
        
        Assert.IsFalse(success);
        Assert.AreEqual(0, value);
    }

    [TestMethod]
    public void MemoryCache_Remove_RemovesEntry()
    {
        var cache = new MemoryCache<string, int>(new MemoryCacheOptions());
        cache.Set("key1", 100);
        
        cache.Remove("key1");
        
        var success = cache.TryGetValue("key1", out _);
        Assert.IsFalse(success);
    }

    [TestMethod]
    public void MemoryCache_GetOrCreate_CreatesIfNotExists()
    {
        var cache = new MemoryCache<string, int>(new MemoryCacheOptions());
        
        var value = cache.GetOrCreate("key1", _ => 42);
        
        Assert.AreEqual(42, value);
        
        // Subsequent call should return cached value
        var value2 = cache.GetOrCreate("key1", _ => 999);
        Assert.AreEqual(42, value2);
    }

    [TestMethod]
    public async Task MemoryCache_GetOrCreateAsync_CreatesIfNotExists()
    {
        var cache = new MemoryCache<string, int>(new MemoryCacheOptions());
        
        var value = await cache.GetOrCreateAsync("key1", _ => Task.FromResult(42));
        
        Assert.AreEqual(42, value);
        
        // Subsequent call should return cached value
        var value2 = await cache.GetOrCreateAsync("key1", _ => Task.FromResult(999));
        Assert.AreEqual(42, value2);
    }

    [TestMethod]
    public void MemoryCache_SetWithOptions_AppliesOptions()
    {
        var cache = new MemoryCache<string, int>(new MemoryCacheOptions());
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1)
        };
        
        cache.Set("key1", 42, options);
        
        var success = cache.TryGetValue("key1", out var value);
        Assert.IsTrue(success);
        Assert.AreEqual(42, value);
    }
}

