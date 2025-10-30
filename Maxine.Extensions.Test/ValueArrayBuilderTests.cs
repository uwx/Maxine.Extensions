using Maxine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class ValueArrayBuilderTests
{
    [TestMethod]
    public void Constructor_WithStackBuffer_CreatesBuilder()
    {
        Span<int> buffer = stackalloc int[10];
        var builder = new ValueArrayBuilder<int>(buffer);
        
        Assert.AreEqual(0, builder.Length);
        Assert.AreEqual(10, builder.Capacity);
    }

    [TestMethod]
    public void Constructor_WithCapacity_CreatesBuilder()
    {
        var builder = new ValueArrayBuilder<int>(20);
        
        Assert.AreEqual(0, builder.Length);
        Assert.IsGreaterThanOrEqualTo(20, builder.Capacity);
    }

    [TestMethod]
    public void Append_SingleItem_AddsToBuilder()
    {
        Span<int> buffer = stackalloc int[10];
        var builder = new ValueArrayBuilder<int>(buffer);
        
        builder.Append(42);
        
        Assert.AreEqual(1, builder.Length);
        Assert.AreEqual(42, builder[0]);
    }

    [TestMethod]
    public void Append_MultipleItems_AddsToBuilder()
    {
        Span<int> buffer = stackalloc int[10];
        var builder = new ValueArrayBuilder<int>(buffer);
        
        builder.Append(1);
        builder.Append(2);
        builder.Append(3);
        
        Assert.AreEqual(3, builder.Length);
        Assert.AreEqual(1, builder[0]);
        Assert.AreEqual(2, builder[1]);
        Assert.AreEqual(3, builder[2]);
    }

    [TestMethod]
    public void AsSpan_ReturnsCorrectSpan()
    {
        Span<int> buffer = stackalloc int[10];
        var builder = new ValueArrayBuilder<int>(buffer);
        
        builder.Append(10);
        builder.Append(20);
        
        var span = builder.AsSpan();
        
        Assert.AreEqual(2, span.Length);
        Assert.AreEqual(10, span[0]);
        Assert.AreEqual(20, span[1]);
    }

    [TestMethod]
    public void Length_CanBeSet()
    {
        Span<int> buffer = stackalloc int[10];
        var builder = new ValueArrayBuilder<int>(buffer);
        
        builder.Append(1);
        builder.Append(2);
        builder.Append(3);
        builder.Length = 2;
        
        Assert.AreEqual(2, builder.Length);
    }

    [TestMethod]
    public void EnsureCapacity_GrowsBuffer()
    {
        var builder = new ValueArrayBuilder<int>(5);
        var originalCapacity = builder.Capacity;
        
        builder.EnsureCapacity(20);
        
        Assert.IsGreaterThanOrEqualTo(20, builder.Capacity);
        Assert.IsGreaterThan(originalCapacity, builder.Capacity);
    }

    [TestMethod]
    public void Indexer_GetItem_ReturnsCorrectItem()
    {
        Span<int> buffer = stackalloc int[10];
        var builder = new ValueArrayBuilder<int>(buffer);
        
        builder.Append(100);
        builder.Append(200);
        builder.Append(300);
        
        Assert.AreEqual(100, builder[0]);
        Assert.AreEqual(200, builder[1]);
        Assert.AreEqual(300, builder[2]);
    }

    [TestMethod]
    public void Indexer_SetItem_ModifiesItem()
    {
        Span<int> buffer = stackalloc int[10];
        var builder = new ValueArrayBuilder<int>(buffer);
        
        builder.Append(1);
        builder[0] = 999;
        
        Assert.AreEqual(999, builder[0]);
    }

    [TestMethod]
    public void Append_AddsMultipleItemsFromSpan()
    {
        Span<int> buffer = stackalloc int[20];
        var builder = new ValueArrayBuilder<int>(buffer);
        
        ReadOnlySpan<int> items = [1, 2, 3, 4, 5];
        foreach (var item in items)
        {
            builder.Append(item);
        }
        
        Assert.AreEqual(5, builder.Length);
        Assert.AreEqual(1, builder[0]);
        Assert.AreEqual(5, builder[4]);
    }

    [TestMethod]
    public void ToString_ReturnsStringRepresentation()
    {
        Span<int> buffer = stackalloc int[10];
        var builder = new ValueArrayBuilder<int>(buffer);
        
        builder.Append(1);
        builder.Append(2);
        
        var result = builder.ToString();
        Assert.IsNotNull(result);
    }
}

