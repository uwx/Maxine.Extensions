using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Maxine.Extensions.Test;

[TestClass]
public class TypeExtensionsAdditionalTests
{
    [TestMethod]
    public void HumanizeTypeName_NonGenericType_ReturnsFullName()
    {
        var type = typeof(string);
        
        var result = type.HumanizeTypeName();
        
        Assert.AreEqual("System.String", result);
    }

    [TestMethod]
    public void HumanizeTypeName_GenericType_FormatsWithAngleBrackets()
    {
        var type = typeof(List<int>);
        
        var result = type.HumanizeTypeName();
        
        Assert.IsTrue(result.Contains("System.Collections.Generic.List"));
        Assert.IsTrue(result.Contains("<"));
        Assert.IsTrue(result.Contains(">"));
        Assert.IsTrue(result.Contains("System.Int32"));
    }

    [TestMethod]
    public void HumanizeTypeName_NestedGenericType_FormatsCorrectly()
    {
        var type = typeof(Dictionary<string, List<int>>);
        
        var result = type.HumanizeTypeName();
        
        Assert.IsTrue(result.Contains("System.Collections.Generic.Dictionary"));
        Assert.IsTrue(result.Contains("System.String"));
        Assert.IsTrue(result.Contains("System.Collections.Generic.List"));
        Assert.IsTrue(result.Contains("System.Int32"));
    }

    [TestMethod]
    public void HumanizeTypeName_MultipleGenericParameters_SeparatesWithComma()
    {
        var type = typeof(Dictionary<string, int>);
        
        var result = type.HumanizeTypeName();
        
        Assert.IsTrue(result.Contains(", "));
        Assert.IsTrue(result.Contains("System.String"));
        Assert.IsTrue(result.Contains("System.Int32"));
    }

    [TestMethod]
    public void HumanizeTypeName_ComplexNestedGenerics_FormatsReadably()
    {
        var type = typeof(Dictionary<string, Dictionary<int, List<bool>>>);
        
        var result = type.HumanizeTypeName();
        
        Assert.IsTrue(result.Contains("System.Collections.Generic.Dictionary"));
        Assert.IsTrue(result.Contains("System.String"));
        Assert.IsTrue(result.Contains("System.Int32"));
        Assert.IsTrue(result.Contains("System.Collections.Generic.List"));
        Assert.IsTrue(result.Contains("System.Boolean"));
    }

    [TestMethod]
    public void HumanizeTypeName_ValueType_ReturnsFullName()
    {
        var type = typeof(int);
        
        var result = type.HumanizeTypeName();
        
        Assert.AreEqual("System.Int32", result);
    }

    [TestMethod]
    public void HumanizeTypeName_GenericValueType_Formats()
    {
        var type = typeof(Nullable<int>);
        
        var result = type.HumanizeTypeName();
        
        Assert.IsTrue(result.Contains("System.Nullable"));
        Assert.IsTrue(result.Contains("System.Int32"));
    }

    [TestMethod]
    public void HumanizeTypeName_CustomGenericType_FormatsWithFullNamespace()
    {
        var type = typeof(CustomGeneric<string>);
        
        var result = type.HumanizeTypeName();
        
        Assert.IsTrue(result.Contains("CustomGeneric"));
        Assert.IsTrue(result.Contains("System.String"));
    }

    [TestMethod]
    public void HumanizeTypeName_ArrayType_IncludesArrayNotation()
    {
        var type = typeof(int[]);
        
        var result = type.HumanizeTypeName();
        
        Assert.IsTrue(result.Contains("Int32"));
    }

    [TestMethod]
    public void HumanizeTypeName_GenericList_FormatsCorrectly()
    {
        var type = typeof(List<string>);
        
        var result = type.HumanizeTypeName();
        
        Assert.IsTrue(result.Contains("System.Collections.Generic.List"));
        Assert.IsTrue(result.Contains("<System.String>"));
    }

    [TestMethod]
    public void HumanizeTypeName_ThreeGenericParameters_FormatsAll()
    {
        var type = typeof(CustomGeneric3<int, string, bool>);
        
        var result = type.HumanizeTypeName();
        
        Assert.IsTrue(result.Contains("System.Int32"));
        Assert.IsTrue(result.Contains("System.String"));
        Assert.IsTrue(result.Contains("System.Boolean"));
        Assert.IsTrue(result.Contains(", "));
    }
}

// Helper types for testing
public class CustomGeneric<T> { }
public class CustomGeneric3<T1, T2, T3> { }
