using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class LinqExtensionsAdditionalTests
{
    // Note: ChunkAsync and SelectMany are in AsyncEnumerableExtensions and already tested

    [TestMethod]
    public void ToOrderedDictionary_CreatesOrderedDictionary()
    {
        var items = new[] { ("a", 1), ("b", 2), ("c", 3) };

        var dict = items.ToOrderedDictionary(x => x.Item1, x => x.Item2);

        Assert.AreEqual(3, dict.Count);
        Assert.AreEqual(1, dict["a"]);
        Assert.AreEqual(2, dict["b"]);
        Assert.AreEqual(3, dict["c"]);
    }

    [TestMethod]
    public void ToOrderedDictionary_EmptySequence_ReturnsEmptyDictionary()
    {
        var items = Array.Empty<(string, int)>();

        var dict = items.ToOrderedDictionary(x => x.Item1, x => x.Item2);

        Assert.AreEqual(0, dict.Count);
    }

    [TestMethod]
    public void ToOrderedDictionary_DuplicateKeys_LastValueWins()
    {
        var items = new[] { ("a", 1), ("a", 2), ("b", 3) };

        var dict = items.ToOrderedDictionary(x => x.Item1, x => x.Item2);

        Assert.AreEqual(2, dict.Count);
        Assert.AreEqual(2, dict["a"]); // Last value for "a"
        Assert.AreEqual(3, dict["b"]);
    }

    [TestMethod]
    public void ToOrderedDictionary_ComplexKey_Works()
    {
        var items = new[] { (1, "one"), (2, "two"), (3, "three") };

        var dict = items.ToOrderedDictionary(x => x.Item1, x => x.Item2);

        Assert.AreEqual(3, dict.Count);
        Assert.AreEqual("one", dict[1]);
        Assert.AreEqual("two", dict[2]);
        Assert.AreEqual("three", dict[3]);
    }

    [TestMethod]
    public void ToOrderedDictionary_ComplexValue_Stores()
    {
        var items = new[] { ("a", new List<int> { 1, 2 }), ("b", new List<int> { 3, 4 }) };

        var dict = items.ToOrderedDictionary(x => x.Item1, x => x.Item2);

        Assert.AreEqual(2, dict.Count);
        CollectionAssert.AreEqual(new[] { 1, 2 }, dict["a"]);
        CollectionAssert.AreEqual(new[] { 3, 4 }, dict["b"]);
    }
}
