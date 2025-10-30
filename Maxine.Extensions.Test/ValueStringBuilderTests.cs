using Maxine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Maxine.Extensions.Test;

[TestClass]
public class ValueStringBuilderTests
{
    [TestMethod]
    public void Constructor_WithStackBuffer_CreatesBuilder()
    {
        Span<char> buffer = stackalloc char[100];
        var builder = new ValueStringBuilder(buffer);
        
        Assert.AreEqual(0, builder.Length);
        Assert.AreEqual(100, builder.Capacity);
    }

    [TestMethod]
    public void Constructor_WithCapacity_CreatesBuilder()
    {
        var builder = new ValueStringBuilder(50);
        
        Assert.AreEqual(0, builder.Length);
        Assert.IsGreaterThanOrEqualTo(50, builder.Capacity);
    }

    [TestMethod]
    public void Append_SingleChar_AddsToBuilder()
    {
        Span<char> buffer = stackalloc char[10];
        var builder = new ValueStringBuilder(buffer);
        
        builder.Append('A');
        
        Assert.AreEqual(1, builder.Length);
        Assert.AreEqual('A', builder[0]);
    }

    [TestMethod]
    public void Append_String_AddsToBuilder()
    {
        Span<char> buffer = stackalloc char[20];
        var builder = new ValueStringBuilder(buffer);
        
        builder.Append("Hello");
        
        Assert.AreEqual(5, builder.Length);
    }

    [TestMethod]
    public void ToString_ReturnsBuiltString()
    {
        Span<char> buffer = stackalloc char[20];
        var builder = new ValueStringBuilder(buffer);
        
        builder.Append("Test");
        var result = builder.ToString();
        
        Assert.AreEqual("Test", result);
    }

    [TestMethod]
    public void Length_CanBeSet()
    {
        Span<char> buffer = stackalloc char[20];
        var builder = new ValueStringBuilder(buffer);
        
        builder.Append("Hello");
        builder.Length = 3;
        
        Assert.AreEqual(3, builder.Length);
        Assert.AreEqual("Hel", builder.ToString());
    }

    [TestMethod]
    public void EnsureCapacity_GrowsBuffer()
    {
        var builder = new ValueStringBuilder(10);
        var originalCapacity = builder.Capacity;
        
        builder.EnsureCapacity(50);
        
        Assert.IsGreaterThanOrEqualTo(50, builder.Capacity);
        Assert.IsGreaterThan(originalCapacity, builder.Capacity);
    }

    [TestMethod]
    public void Indexer_GetChar_ReturnsCorrectChar()
    {
        Span<char> buffer = stackalloc char[10];
        var builder = new ValueStringBuilder(buffer);
        
        builder.Append("ABC");
        
        Assert.AreEqual('A', builder[0]);
        Assert.AreEqual('B', builder[1]);
        Assert.AreEqual('C', builder[2]);
    }

    [TestMethod]
    public void AppendLine_AddsNewLine()
    {
        Span<char> buffer = stackalloc char[20];
        var builder = new ValueStringBuilder(buffer);
        
        builder.AppendLine("Test");
        var result = builder.ToString();
        
        Assert.Contains("Test", result);
        Assert.Contains(Environment.NewLine, result);
    }

    [TestMethod]
    public void Length_CanBeSetToZero_EffectivelyClearsBuilder()
    {
        Span<char> buffer = stackalloc char[20];
        var builder = new ValueStringBuilder(buffer);
        
        builder.Append("Test");
        builder.Length = 0;
        
        Assert.AreEqual(0, builder.Length);
    }
}

