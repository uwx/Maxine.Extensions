namespace Maxine.Extensions.Test;

[TestClass]
public class WeightedRandomizerTests
{
    [TestMethod]
    public void Constructor_WithEntries_CreatesRandomizer()
    {
        var entries = new[] { ("A", 1), ("B", 2), ("C", 3) };
        var randomizer = new WeightedRandomizer<string>(entries);
        Assert.IsNotNull(randomizer);
        Assert.AreEqual(3, randomizer.Count);
    }

    [TestMethod]
    public void TotalWeight_ReturnsSumOfAllWeights()
    {
        var entries = new[] { ("A", 1), ("B", 2), ("C", 3) };
        var randomizer = new WeightedRandomizer<string>(entries);
        Assert.AreEqual(6, randomizer.TotalWeight);
    }

    [TestMethod]
    public void Next_ReturnsItemFromCollection()
    {
        var entries = new[] { ("A", 1), ("B", 2), ("C", 3) };
        var randomizer = new WeightedRandomizer<string>(entries);
        var result = randomizer.Next();
        Assert.IsTrue(result == "A" || result == "B" || result == "C");
    }

    [TestMethod]
    public void Next_CalledMultipleTimes_ReturnsItems()
    {
        var entries = new[] { ("A", 1), ("B", 2), ("C", 3) };
        var randomizer = new WeightedRandomizer<string>(entries);
        
        for (int i = 0; i < 100; i++)
        {
            var result = randomizer.Next();
            Assert.IsTrue(result == "A" || result == "B" || result == "C");
        }
    }

    [TestMethod]
    public void Constructor_WithWeightFunction_CreatesRandomizer()
    {
        var items = new[] { "A", "B", "C" };
        var randomizer = new WeightedRandomizer<string>(items, s => s == "A" ? 1 : s == "B" ? 2 : 3);
        Assert.AreEqual(3, randomizer.Count);
        Assert.AreEqual(6, randomizer.TotalWeight);
    }

    [TestMethod]
    public void Constructor_WithNegativeWeight_ThrowsException()
    {
        var entries = new[] { ("A", -1), ("B", 2) };
        Assert.Throws<InvalidOperationException>(() =>
        {
            var randomizer = new WeightedRandomizer<string>(entries);
        });
    }

    [TestMethod]
    public void Constructor_WithZeroWeight_ThrowsException()
    {
        var entries = new[] { ("A", 0), ("B", 2) };
        Assert.Throws<InvalidOperationException>(() =>
        {
            var randomizer = new WeightedRandomizer<string>(entries);
        });
    }

    [TestMethod]
    public void GetEnumerator_ReturnsAllItems()
    {
        var entries = new[] { ("A", 1), ("B", 2), ("C", 3) };
        var randomizer = new WeightedRandomizer<string>(entries);
        var items = randomizer.ToList();
        
        Assert.HasCount(3, items);
        CollectionAssert.Contains(items, "A");
        CollectionAssert.Contains(items, "B");
        CollectionAssert.Contains(items, "C");
    }
}

