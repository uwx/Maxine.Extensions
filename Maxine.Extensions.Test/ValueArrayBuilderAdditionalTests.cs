using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class ValueArrayBuilderAdditionalTests
{
    [TestMethod]
    public void Constructor_WithInitialCapacity_CreatesBuilder()
    {
        var builder = new ValueArrayBuilder<int>(10);

        Assert.AreEqual(0, builder.Length);
        Assert.IsTrue(builder.Capacity >= 10);
    }

    [TestMethod]
    public void Constructor_WithSpan_UsesSpan()
    {
        Span<int> buffer = stackalloc int[5];
        var builder = new ValueArrayBuilder<int>(buffer);

        Assert.AreEqual(0, builder.Length);
        Assert.AreEqual(5, builder.Capacity);
    }

    [TestMethod]
    public void Append_SingleItem_IncreasesLength()
    {
        var builder = new ValueArrayBuilder<int>(10);
        
        builder.Append(42);
        
        Assert.AreEqual(1, builder.Length);
        Assert.AreEqual(42, builder[0]);
    }

    [TestMethod]
    public void Append_MultipleItems_AllAdded()
    {
        var builder = new ValueArrayBuilder<int>(10);
        
        builder.Append(1);
        builder.Append(2);
        builder.Append(3);
        
        Assert.AreEqual(3, builder.Length);
        Assert.AreEqual(1, builder[0]);
        Assert.AreEqual(2, builder[1]);
        Assert.AreEqual(3, builder[2]);
    }

    [TestMethod]
    public void Append_WithCount_AddsMultipleCopies()
    {
        var builder = new ValueArrayBuilder<int>(10);
        
        builder.Append(99, 3);
        
        Assert.AreEqual(3, builder.Length);
        Assert.AreEqual(99, builder[0]);
        Assert.AreEqual(99, builder[1]);
        Assert.AreEqual(99, builder[2]);
    }

    [TestMethod]
    public void Append_ReadOnlySpan_AddsAllElements()
    {
        var builder = new ValueArrayBuilder<int>(10);
        ReadOnlySpan<int> values = stackalloc int[] { 1, 2, 3, 4, 5 };
        
        builder.Append(values);
        
        Assert.AreEqual(5, builder.Length);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, builder.AsSpan().ToArray());
    }

    [TestMethod]
    public void AsSpan_ReturnsCorrectContent()
    {
        var builder = new ValueArrayBuilder<int>(10);
        builder.Append(1);
        builder.Append(2);
        builder.Append(3);
        
        var span = builder.AsSpan();
        
        Assert.AreEqual(3, span.Length);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, span.ToArray());
    }

    [TestMethod]
    public void AsSpan_WithStartAndLength_ReturnsSubset()
    {
        var builder = new ValueArrayBuilder<int>(10);
        builder.Append(1);
        builder.Append(2);
        builder.Append(3);
        builder.Append(4);
        builder.Append(5);
        
        var span = builder.AsSpan(1, 3);
        
        Assert.AreEqual(3, span.Length);
        CollectionAssert.AreEqual(new[] { 2, 3, 4 }, span.ToArray());
    }

    [TestMethod]
    public void Insert_InsertsAtIndex()
    {
        var builder = new ValueArrayBuilder<int>(10);
        builder.Append(1);
        builder.Append(3);
        
        builder.Insert(1, 2, 1);
        
        Assert.AreEqual(3, builder.Length);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, builder.AsSpan().ToArray());
    }

    [TestMethod]
    public void Insert_MultipleValues_InsertsAll()
    {
        var builder = new ValueArrayBuilder<int>(10);
        builder.Append(1);
        builder.Append(5);
        
        var values = new int[] { 2, 3, 4 };
        builder.Insert(1, values);
        
        Assert.AreEqual(5, builder.Length);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, builder.AsSpan().ToArray());
    }

    [TestMethod]
    public void Length_CanBeSet()
    {
        var builder = new ValueArrayBuilder<int>(10);
        builder.Append(1);
        builder.Append(2);
        builder.Append(3);
        
        builder.Length = 2;
        
        Assert.AreEqual(2, builder.Length);
        var span = builder.AsSpan();
        Assert.AreEqual(2, span.Length);
    }

    [TestMethod]
    public void EnsureCapacity_IncreasesCapacity()
    {
        var builder = new ValueArrayBuilder<int>(5);
        
        builder.EnsureCapacity(10);
        
        Assert.IsTrue(builder.Capacity >= 10);
    }

    [TestMethod]
    public void AppendSpan_ReturnsSpanForWriting()
    {
        var builder = new ValueArrayBuilder<int>(10);
        
        var span = builder.AppendSpan(3);
        span[0] = 10;
        span[1] = 20;
        span[2] = 30;
        
        Assert.AreEqual(3, builder.Length);
        CollectionAssert.AreEqual(new[] { 10, 20, 30 }, builder.AsSpan().ToArray());
    }

    [TestMethod]
    public void ToArrayAndDispose_ReturnsArray()
    {
        var builder = new ValueArrayBuilder<int>(10);
        builder.Append(1);
        builder.Append(2);
        builder.Append(3);
        
        var array = builder.ToArrayAndDispose();
        
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, array);
    }

    [TestMethod]
    public void TryCopyTo_Success_CopiesAndReturnsTrue()
    {
        var builder = new ValueArrayBuilder<int>(10);
        builder.Append(1);
        builder.Append(2);
        builder.Append(3);
        
        var destination = new int[5];
        var success = builder.TryCopyTo(destination, out var bytesWritten);
        
        Assert.IsTrue(success);
        Assert.AreEqual(3, bytesWritten);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, destination[..3]);
    }

    [TestMethod]
    public void TryCopyTo_InsufficientSpace_ReturnsFalse()
    {
        var builder = new ValueArrayBuilder<int>(10);
        builder.Append(1);
        builder.Append(2);
        builder.Append(3);
        builder.Append(4);
        builder.Append(5);
        
        var destination = new int[2];
        var success = builder.TryCopyTo(destination, out var bytesWritten);
        
        Assert.IsFalse(success);
        Assert.AreEqual(0, bytesWritten);
    }

    [TestMethod]
    public void Indexer_GetAndSet_Works()
    {
        var builder = new ValueArrayBuilder<int>(10);
        builder.Append(1);
        builder.Append(2);
        builder.Append(3);
        
        builder[1] = 99;
        
        Assert.AreEqual(99, builder[1]);
        CollectionAssert.AreEqual(new[] { 1, 99, 3 }, builder.AsSpan().ToArray());
    }

    [TestMethod]
    public void RawSpan_ReturnsUnderlyingStorage()
    {
        var builder = new ValueArrayBuilder<int>(10);
        builder.Append(1);
        builder.Append(2);
        
        var raw = builder.RawSpan;
        
        Assert.IsTrue(raw.Length >= 2);
        Assert.AreEqual(1, raw[0]);
        Assert.AreEqual(2, raw[1]);
    }

    [TestMethod]
    public void AsSpan_WithTerminate_AddsDefault()
    {
        var builder = new ValueArrayBuilder<int>(10);
        builder.Append(1);
        builder.Append(2);
        
        var span = builder.AsSpan(terminate: true);
        
        Assert.AreEqual(2, span.Length);
        // The terminate parameter ensures there's a default(T) after Length
        Assert.AreEqual(0, builder.RawSpan[2]); // default(int) == 0
    }
}
