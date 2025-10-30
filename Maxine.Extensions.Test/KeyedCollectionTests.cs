using Maxine.Extensions.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class KeyedCollectionTests
{
    // KeyedCollection is an abstract base class
    // Tests would require a concrete implementation
    
    [TestMethod]
    public void KeyedCollection_CanBeSubclassed()
    {
        // Test with a simple implementation
        var collection = new SimpleKeyedCollection();
        collection.Add(new KeyedItem("key1", "value1"));
        
        Assert.HasCount(1, collection);
        Assert.AreEqual("value1", collection["key1"].Value);
    }

    [TestMethod]
    public void KeyedCollection_Contains_FindsItemByKey()
    {
        var collection = new SimpleKeyedCollection();
        collection.Add(new KeyedItem("key1", "value1"));
        
        Assert.IsTrue(collection.Contains("key1"));
        Assert.IsFalse(collection.Contains("key2"));
    }

    [TestMethod]
    public void KeyedCollection_Remove_RemovesItem()
    {
        var collection = new SimpleKeyedCollection();
        var item = new KeyedItem("key1", "value1");
        collection.Add(item);
        
        collection.Remove("key1");
        
        Assert.IsEmpty(collection);
        Assert.IsFalse(collection.Contains("key1"));
    }

    private class KeyedItem
    {
        public string Key { get; }
        public string Value { get; }

        public KeyedItem(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    private class SimpleKeyedCollection : System.Collections.ObjectModel.KeyedCollection<string, KeyedItem>
    {
        protected override string GetKeyForItem(KeyedItem item) => item.Key;
    }
}

