namespace Maxine.Extensions.Test;

[TestClass]
public class LinqMathExtensionsAdditionalTests
{
    [TestMethod]
    public void Sum_IntSequence_ReturnsCorrectSum()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        var result = LinqMathExtensions.Sum(numbers);
        Assert.AreEqual(15, result);
    }

    [TestMethod]
    public void Sum_DoubleSequence_ReturnsCorrectSum()
    {
        var numbers = new[] { 1.5, 2.5, 3.0 };
        var result = LinqMathExtensions.Sum(numbers);
        Assert.AreEqual(7.0, result, 0.0001);
    }

    [TestMethod]
    public void Sum_EmptySequence_ReturnsAdditiveIdentity()
    {
        var numbers = Array.Empty<int>();
        var result = LinqMathExtensions.Sum(numbers);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Sum_SingleElement_ReturnsElement()
    {
        var numbers = new[] { 42 };
        var result = LinqMathExtensions.Sum(numbers);
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void Sum_DecimalSequence_ReturnsCorrectSum()
    {
        var numbers = new[] { 10.5m, 20.25m, 5.25m };
        var result = LinqMathExtensions.Sum(numbers);
        Assert.AreEqual(36.0m, result);
    }

    [TestMethod]
    public void Range_IntWithIntStep_GeneratesCorrectSequence()
    {
        var result = LinqMathExtensions.Range<int>(0, 10, 2).ToList();
        CollectionAssert.AreEqual(new[] { 0, 2, 4, 6, 8 }, result);
    }

    [TestMethod]
    public void Range_FloatWithFloatStep_GeneratesCorrectSequence()
    {
        var result = LinqMathExtensions.Range<float>(0.0f, 2.0f, 0.5f).ToList();
        Assert.AreEqual(4, result.Count);
        Assert.AreEqual(0.0f, result[0], 0.0001f);
        Assert.AreEqual(0.5f, result[1], 0.0001f);
        Assert.AreEqual(1.0f, result[2], 0.0001f);
        Assert.AreEqual(1.5f, result[3], 0.0001f);
    }

    [TestMethod]
    public void Range_StartEqualsEnd_ReturnsEmpty()
    {
        var result = LinqMathExtensions.Range<int>(5, 5, 1).ToList();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Range_StartGreaterThanEnd_ReturnsEmpty()
    {
        var result = LinqMathExtensions.Range<int>(10, 5, 1).ToList();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Range_SingleStepReachesEnd_ExcludesEnd()
    {
        var result = LinqMathExtensions.Range<int>(0, 3, 1).ToList();
        CollectionAssert.AreEqual(new[] { 0, 1, 2 }, result);
    }

    [TestMethod]
    public void Average_IntSequence_ReturnsCorrectAverage()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        var result = LinqMathExtensions.Average(numbers);
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void Average_SingleElement_ReturnsElement()
    {
        var numbers = new[] { 42 };
        var result = LinqMathExtensions.Average(numbers);
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void Average_MixedPositiveNegative_ReturnsCorrectAverage()
    {
        var numbers = new[] { -10, 0, 10, 20 };
        var result = LinqMathExtensions.Average(numbers);
        Assert.AreEqual(5, result);
    }
}
