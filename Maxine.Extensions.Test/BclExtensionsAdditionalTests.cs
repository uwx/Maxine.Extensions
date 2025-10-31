using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class BclExtensionsAdditionalTests
{
    [TestMethod]
    public void ConvertAll_ConvertsArrayElements()
    {
        var input = new[] { 1, 2, 3, 4, 5 };
        
        var result = input.ConvertAll(x => x.ToString());
        
        Assert.AreEqual(5, result.Length);
        CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "5" }, result);
    }

    [TestMethod]
    public void ConvertAll_EmptyArray_ReturnsEmptyArray()
    {
        var input = Array.Empty<int>();
        
        var result = input.ConvertAll(x => x.ToString());
        
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void ConvertAll_ComplexConversion_Works()
    {
        var input = new[] { "1", "2", "3" };
        
        var result = input.ConvertAll(int.Parse);
        
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
    }

    [TestMethod]
    public void Upcast_StringArrayToObjectArray()
    {
        var input = new[] { "hello", "world" };
        
        var result = input.Upcast<string, object>();
        
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("hello", result[0]);
        Assert.AreEqual("world", result[1]);
    }

    [TestMethod]
    public void Upcast_DerivedClassToBaseClass()
    {
        var input = new[] { new DerivedClass() };
        
        var result = input.Upcast<DerivedClass, BaseClass>();
        
        Assert.AreEqual(1, result.Length);
        Assert.IsInstanceOfType<BaseClass>(result[0]);
    }

    [TestMethod]
    public void Upcast_EmptyArray_ReturnsEmptyArray()
    {
        var input = Array.Empty<string>();
        
        var result = input.Upcast<string, object>();
        
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void Fill_FillsArrayWithValue()
    {
        var array = new int[5];
        
        var result = array.Fill(42);
        
        Assert.AreSame(array, result);
        foreach (var value in result)
        {
            Assert.AreEqual(42, value);
        }
    }

    [TestMethod]
    public void Fill_ReturnsModifiedArray()
    {
        var array = new[] { 1, 2, 3 };
        
        var result = array.Fill(99);
        
        Assert.AreSame(array, result);
        CollectionAssert.AreEqual(new[] { 99, 99, 99 }, result);
    }

    [TestMethod]
    public void PadAndTruncateToLength_PadsShortSequence()
    {
        var input = new[] { 1, 2, 3 };
        
        var result = input.PadAndTruncateToLength(5, 0).ToArray();
        
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 0, 0 }, result);
    }

    [TestMethod]
    public void PadAndTruncateToLength_TruncatesLongSequence()
    {
        var input = new[] { 1, 2, 3, 4, 5 };
        
        var result = input.PadAndTruncateToLength(3, 0).ToArray();
        
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
    }

    [TestMethod]
    public void PadAndTruncateToLength_ExactLength_NoChange()
    {
        var input = new[] { 1, 2, 3 };
        
        var result = input.PadAndTruncateToLength(3, 0).ToArray();
        
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
    }

    [TestMethod]
    public void PadAndTruncateToLength_EmptySequence_PadsToLength()
    {
        var input = Array.Empty<int>();
        
        var result = input.PadAndTruncateToLength(3, 99).ToArray();
        
        CollectionAssert.AreEqual(new[] { 99, 99, 99 }, result);
    }

    [TestMethod]
    public void Except_RemovesSpecificValue()
    {
        var input = new[] { 1, 2, 3, 2, 4, 2, 5 };
        
        var result = input.Except(2).ToArray();
        
        CollectionAssert.AreEqual(new[] { 1, 3, 4, 5 }, result);
    }

    [TestMethod]
    public void Except_ValueNotPresent_ReturnsAll()
    {
        var input = new[] { 1, 2, 3 };
        
        var result = input.Except(99).ToArray();
        
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
    }

    [TestMethod]
    public void Except_CustomComparer_UsesComparer()
    {
        var input = new[] { "hello", "WORLD", "test" };
        
        var result = input.Except("world", StringComparer.OrdinalIgnoreCase).ToArray();
        
        Assert.AreEqual(2, result.Length);
        CollectionAssert.AreEqual(new[] { "hello", "test" }, result);
    }

    [TestMethod]
    public void Except_AllElementsMatch_ReturnsEmpty()
    {
        var input = new[] { 5, 5, 5, 5 };
        
        var result = input.Except(5).ToArray();
        
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void SetCurrentStackTrace_ModifiesStackTrace()
    {
        var ex = new InvalidOperationException("Test exception");
        
        var result = ex.SetCurrentStackTrace();
        
        Assert.AreSame(ex, result);
        Assert.IsNotNull(result.StackTrace);
    }

    [TestMethod]
    public void ThrowWithExistingStackTrace_PreservesStackTrace()
    {
        var ex = CreateExceptionWithStackTrace();
        var originalStackTrace = ex.StackTrace;

        try
        {
            ex.ThrowWithExistingStackTrace();
            Assert.Fail("Expected exception to be thrown");
        }
        catch (InvalidOperationException thrown)
        {
            Assert.AreSame(ex, thrown);
            Assert.IsNotNull(thrown.StackTrace);
            Assert.IsTrue(thrown.StackTrace!.Contains(nameof(CreateExceptionWithStackTrace)));
        }
    }

    private InvalidOperationException CreateExceptionWithStackTrace()
    {
        try
        {
            throw new InvalidOperationException("Original exception");
        }
        catch (InvalidOperationException ex)
        {
            return ex;
        }
    }
}

// Helper classes for Upcast tests
public class BaseClass { }
public class DerivedClass : BaseClass { }
