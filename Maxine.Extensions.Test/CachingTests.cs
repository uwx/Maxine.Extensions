using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions.Caching;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Maxine.Extensions.Test;

[TestClass]
public class CachingTests
{
    [TestMethod]
    public void TestCacheEntryExtensions_SetAbsoluteExpiration()
    {
        // Arrange
        var options = new MemoryCacheEntryOptions();

        // Act
        options.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        // Assert
        Assert.AreEqual(TimeSpan.FromMinutes(5), options.AbsoluteExpirationRelativeToNow);
    }

    [TestMethod]
    public void TestMemoryCacheEntryOptions_DefaultValues()
    {
        // Arrange
        var options = new MemoryCacheEntryOptions();

        // Assert
        Assert.IsNull(options.AbsoluteExpiration);
        Assert.IsNull(options.AbsoluteExpirationRelativeToNow);
    }

    [TestMethod]
    public void TestMemoryCache_AddAndRetrieve()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var key = "testKey";
        var value = "testValue";

        // Act
        cache.Set(key, value);
        var retrievedValue = cache.Get<string>(key);

        // Assert
        Assert.AreEqual(value, retrievedValue);
    }
}