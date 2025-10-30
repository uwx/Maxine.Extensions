using Maxine.Extensions.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class EnumMapTests
{
    private enum TestEnum
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Five = 5,
        Ten = 10
    }

    [TestMethod]
    public void Constructor_Default_CreatesEmptyMap()
    {
        var map = new EnumMap<TestEnum, string>();
        
        Assert.IsFalse(map.ContainsKey(TestEnum.Zero));
        Assert.IsFalse(map.ContainsKey(TestEnum.One));
    }

    [TestMethod]
    public void Constructor_WithKeyValuePairs_InitializesMap()
    {
        var entries = new List<KeyValuePair<TestEnum, string>>
        {
            new(TestEnum.One, "one"),
            new(TestEnum.Two, "two")
        };
        
        var map = new EnumMap<TestEnum, string>(entries);
        
        Assert.IsTrue(map.ContainsKey(TestEnum.One));
        Assert.AreEqual("one", map[TestEnum.One]);
        Assert.IsTrue(map.ContainsKey(TestEnum.Two));
        Assert.AreEqual("two", map[TestEnum.Two]);
    }

    [TestMethod]
    public void Constructor_WithTuples_InitializesMap()
    {
        var entries = new List<(TestEnum, string)>
        {
            (TestEnum.Zero, "zero"),
            (TestEnum.Five, "five")
        };
        
        var map = new EnumMap<TestEnum, string>(entries);
        
        Assert.IsTrue(map.ContainsKey(TestEnum.Zero));
        Assert.AreEqual("zero", map[TestEnum.Zero]);
        Assert.IsTrue(map.ContainsKey(TestEnum.Five));
        Assert.AreEqual("five", map[TestEnum.Five]);
    }

    [TestMethod]
    public void Constructor_WithParamsTuples_InitializesMap()
    {
        var map = new EnumMap<TestEnum, string>(
            (TestEnum.One, "one"),
            (TestEnum.Ten, "ten")
        );
        
        Assert.IsTrue(map.ContainsKey(TestEnum.One));
        Assert.AreEqual("one", map[TestEnum.One]);
        Assert.IsTrue(map.ContainsKey(TestEnum.Ten));
        Assert.AreEqual("ten", map[TestEnum.Ten]);
    }

    [TestMethod]
    public void Indexer_Set_StoresValue()
    {
        var map = new EnumMap<TestEnum, string>();
        
        map[TestEnum.Two] = "two";
        
        Assert.IsTrue(map.ContainsKey(TestEnum.Two));
        Assert.AreEqual("two", map[TestEnum.Two]);
    }

    [TestMethod]
    public void Indexer_Get_ReturnsValue()
    {
        var map = new EnumMap<TestEnum, string>((TestEnum.Five, "five"));
        
        var value = map[TestEnum.Five];
        
        Assert.AreEqual("five", value);
    }

    [TestMethod]
    public void Indexer_Get_NonExistentKey_ThrowsKeyNotFoundException()
    {
        var map = new EnumMap<TestEnum, string>();
        
        Assert.Throws<KeyNotFoundException>(() => map[TestEnum.One]);
    }

    [TestMethod]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        var map = new EnumMap<TestEnum, string>((TestEnum.One, "one"));
        
        Assert.IsTrue(map.ContainsKey(TestEnum.One));
    }

    [TestMethod]
    public void ContainsKey_NonExistentKey_ReturnsFalse()
    {
        var map = new EnumMap<TestEnum, string>((TestEnum.One, "one"));
        
        Assert.IsFalse(map.ContainsKey(TestEnum.Two));
    }

    [TestMethod]
    public void TryGetValue_ExistingKey_ReturnsTrueWithValue()
    {
        var map = new EnumMap<TestEnum, string>((TestEnum.Two, "two"));
        
        var result = map.TryGetValue(TestEnum.Two, out var value);
        
        Assert.IsTrue(result);
        Assert.AreEqual("two", value);
    }

    [TestMethod]
    public void TryGetValue_NonExistentKey_ReturnsFalseWithDefault()
    {
        var map = new EnumMap<TestEnum, string>((TestEnum.One, "one"));
        
        var result = map.TryGetValue(TestEnum.Five, out var value);
        
        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void Map_AllEnumValues_CanBeUsedAsKeys()
    {
        var map = new EnumMap<TestEnum, string>(
            (TestEnum.Zero, "zero"),
            (TestEnum.One, "one"),
            (TestEnum.Two, "two"),
            (TestEnum.Five, "five"),
            (TestEnum.Ten, "ten")
        );
        
        Assert.AreEqual("zero", map[TestEnum.Zero]);
        Assert.AreEqual("one", map[TestEnum.One]);
        Assert.AreEqual("two", map[TestEnum.Two]);
        Assert.AreEqual("five", map[TestEnum.Five]);
        Assert.AreEqual("ten", map[TestEnum.Ten]);
    }

    [TestMethod]
    public void Map_OverwriteValue_UpdatesCorrectly()
    {
        var map = new EnumMap<TestEnum, string>((TestEnum.One, "original"));
        
        map[TestEnum.One] = "updated";
        
        Assert.AreEqual("updated", map[TestEnum.One]);
    }
}
