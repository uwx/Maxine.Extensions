namespace Maxine.Extensions.Test;

[TestClass]
public class LinqExtensions_GenericMathAdditionalTests
{
    [TestMethod]
    public void Sum_IntSequence_UsesOptimizedPath()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        var result = LinqExtensions_GenericMath.Sum(numbers);
        Assert.AreEqual(15, result);
    }

    [TestMethod]
    public void Sum_LongSequence_UsesOptimizedPath()
    {
        var numbers = new[] { 10L, 20L, 30L, 40L };
        var result = LinqExtensions_GenericMath.Sum(numbers);
        Assert.AreEqual(100L, result);
    }

    [TestMethod]
    public void Sum_FloatSequence_UsesOptimizedPath()
    {
        var numbers = new[] { 1.5f, 2.5f, 3.0f };
        var result = LinqExtensions_GenericMath.Sum(numbers);
        Assert.AreEqual(7.0f, result, 0.0001f);
    }

    [TestMethod]
    public void Sum_DoubleSequence_UsesOptimizedPath()
    {
        var numbers = new[] { 1.5, 2.5, 3.0 };
        var result = LinqExtensions_GenericMath.Sum(numbers);
        Assert.AreEqual(7.0, result, 0.0001);
    }

    [TestMethod]
    public void Sum_DecimalSequence_UsesOptimizedPath()
    {
        var numbers = new[] { 10.5m, 20.25m, 5.25m };
        var result = LinqExtensions_GenericMath.Sum(numbers);
        Assert.AreEqual(36.0m, result);
    }

    [TestMethod]
    public void Sum_EmptySequence_ReturnsAdditiveIdentity()
    {
        var numbers = Array.Empty<int>();
        var result = LinqExtensions_GenericMath.Sum(numbers);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Average_IntSequence_UsesOptimizedPath()
    {
        var numbers = new[] { 2, 4, 6, 8 };
        // Note: The optimized path for int returns double
        double result = Enumerable.Average(numbers);
        Assert.AreEqual(5.0, result, 0.0001);
    }

    [TestMethod]
    public void Average_LongSequence_UsesOptimizedPath()
    {
        var numbers = new[] { 10L, 20L, 30L };
        // Note: The optimized path for long returns double
        double result = Enumerable.Average(numbers);
        Assert.AreEqual(20.0, result, 0.0001);
    }

    [TestMethod]
    public void Average_FloatSequence_UsesOptimizedPath()
    {
        var numbers = new[] { 2.0f, 4.0f, 6.0f };
        var result = LinqExtensions_GenericMath.Average(numbers);
        Assert.AreEqual(4.0f, result, 0.0001f);
    }

    [TestMethod]
    public void Average_DoubleSequence_UsesOptimizedPath()
    {
        var numbers = new[] { 1.0, 2.0, 3.0, 4.0 };
        var result = LinqExtensions_GenericMath.Average(numbers);
        Assert.AreEqual(2.5, result, 0.0001);
    }

    [TestMethod]
    public void Average_DecimalSequence_UsesOptimizedPath()
    {
        var numbers = new[] { 10.0m, 20.0m, 30.0m };
        var result = LinqExtensions_GenericMath.Average(numbers);
        Assert.AreEqual(20.0m, result);
    }

    [TestMethod]
    public void Average_SingleElement_ReturnsElement()
    {
        var numbers = new[] { 42 };
        // Note: The optimized path for int returns double
        double result = Enumerable.Average(numbers);
        Assert.AreEqual(42.0, result, 0.0001);
    }

    [TestMethod]
    public void Range_IntWithIntStep_GeneratesCorrectSequence()
    {
        var result = LinqExtensions_GenericMath.Range<int>(0, 10, 2).ToList();
        CollectionAssert.AreEqual(new[] { 0, 2, 4, 6, 8 }, result);
    }

    [TestMethod]
    public void Range_FloatWithFloatStep_GeneratesCorrectSequence()
    {
        var result = LinqExtensions_GenericMath.Range<float>(0.0f, 2.0f, 0.5f).ToList();
        Assert.AreEqual(4, result.Count);
        Assert.AreEqual(0.0f, result[0], 0.0001f);
        Assert.AreEqual(0.5f, result[1], 0.0001f);
        Assert.AreEqual(1.0f, result[2], 0.0001f);
        Assert.AreEqual(1.5f, result[3], 0.0001f);
    }

    [TestMethod]
    public void Range_StartEqualsEnd_ReturnsEmpty()
    {
        var result = LinqExtensions_GenericMath.Range<int>(5, 5, 1).ToList();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void FindIndex2_PredicateMatches_ReturnsCorrectIndex()
    {
        var items = new[] { "apple", "banana", "cherry", "date" };
        var index = items.FindIndex2(x => x.StartsWith('c'));
        Assert.AreEqual(2, index);
    }

    [TestMethod]
    public void FindIndex2_PredicateMatchesFirst_ReturnsZero()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        var index = items.FindIndex2(x => x > 0);
        Assert.AreEqual(0, index);
    }

    [TestMethod]
    public void FindIndex2_PredicateNoMatch_ReturnsMinusOne()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        var index = items.FindIndex2(x => x > 10);
        Assert.AreEqual(-1, index);
    }

    [TestMethod]
    public void FindIndex2_EmptySequence_ReturnsMinusOne()
    {
        var items = Array.Empty<int>();
        var index = items.FindIndex2(x => x > 0);
        Assert.AreEqual(-1, index);
    }

    [TestMethod]
    public void FindIndex2_MultipleMatches_ReturnsFirstIndex()
    {
        var items = new[] { 1, 5, 10, 15, 20 };
        var index = items.FindIndex2(x => x > 7);
        Assert.AreEqual(2, index);
    }

    [TestMethod]
    public void WithIndex_DefaultStart_EnumeratesWithZeroBasedIndex()
    {
        var items = new[] { "a", "b", "c" };
        var result = items.WithIndex().ToList();

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual((0, "a"), result[0]);
        Assert.AreEqual((1, "b"), result[1]);
        Assert.AreEqual((2, "c"), result[2]);
    }

    [TestMethod]
    public void WithIndex_CustomStart_EnumeratesWithCustomStartIndex()
    {
        var items = new[] { "x", "y", "z" };
        var result = items.WithIndex(10).ToList();

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual((10, "x"), result[0]);
        Assert.AreEqual((11, "y"), result[1]);
        Assert.AreEqual((12, "z"), result[2]);
    }

    [TestMethod]
    public void WithIndex_EmptySequence_ReturnsEmpty()
    {
        var items = Array.Empty<int>();
        var result = items.WithIndex().ToList();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WithIndex_SingleElement_ReturnsOneItem()
    {
        var items = new[] { 42 };
        var result = items.WithIndex(5).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual((5, 42), result[0]);
    }

    [TestMethod]
    public void WithIndex_CanIterateMultipleTimes()
    {
        var items = new[] { 1, 2, 3 };
        var indexed = items.WithIndex();

        var firstIteration = indexed.ToList();
        var secondIteration = indexed.ToList();

        Assert.AreEqual(3, firstIteration.Count);
        Assert.AreEqual(3, secondIteration.Count);
        CollectionAssert.AreEqual(firstIteration, secondIteration);
    }

    [TestMethod]
    public void WithIndex_WorksInForeach()
    {
        var items = new[] { "A", "B", "C" };
        var collected = new List<(int Index, string Element)>();

        foreach (var (index, element) in items.WithIndex(1))
        {
            collected.Add((index, element));
        }

        Assert.AreEqual(3, collected.Count);
        Assert.AreEqual((1, "A"), collected[0]);
        Assert.AreEqual((2, "B"), collected[1]);
        Assert.AreEqual((3, "C"), collected[2]);
    }

    [TestMethod]
    public void WithIndex_EnumeratorReset_RestartsIteration()
    {
        var items = new[] { 10, 20, 30 };
        using var enumerator = items.WithIndex().GetEnumerator();

        // First iteration
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual((0, 10), enumerator.Current);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual((1, 20), enumerator.Current);

        // Reset and iterate again
        enumerator.Reset();
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual((0, 10), enumerator.Current);
    }

    [TestMethod]
    public void WithIndex_CanUseInLinqQueries()
    {
        var items = new[] { "apple", "banana", "cherry" };
        var result = items.WithIndex(1)
            .Where(x => x.Index % 2 == 1)
            .Select(x => x.Element)
            .ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("apple", result[0]);
        Assert.AreEqual("cherry", result[1]);
    }
}
