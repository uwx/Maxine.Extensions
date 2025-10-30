using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class TypeExtensionsTests
{
    [TestMethod]
    public void HumanizeTypeName_NonGenericType_ReturnsFullName()
    {
        var type = typeof(string);
        var result = type.HumanizeTypeName();
        Assert.AreEqual("System.String", result);
    }

    [TestMethod]
    public void HumanizeTypeName_GenericType_ReturnsHumanizedName()
    {
        var type = typeof(List<int>);
        var result = type.HumanizeTypeName();
        Assert.Contains("List", result);
        Assert.Contains("<", result);
        Assert.Contains(">", result);
        Assert.Contains("Int32", result);
    }

    [TestMethod]
    public void HumanizeTypeName_NestedGenericType_ReturnsHumanizedName()
    {
        var type = typeof(Dictionary<string, List<int>>);
        var result = type.HumanizeTypeName();
        Assert.Contains("Dictionary", result);
        Assert.Contains("String", result);
        Assert.Contains("List", result);
        Assert.Contains("Int32", result);
    }

    [TestMethod]
    public void HumanizeTypeName_GenericTypeWithMultipleParameters_FormatsCorrectly()
    {
        var type = typeof(Dictionary<int, string>);
        var result = type.HumanizeTypeName();
        Assert.Contains("Dictionary", result);
        Assert.Contains("Int32", result);
        Assert.Contains("String", result);
        Assert.Contains(",", result);
    }

    [TestMethod]
    public void HumanizeTypeName_PrimitiveType_ReturnsFullName()
    {
        var type = typeof(int);
        var result = type.HumanizeTypeName();
        Assert.AreEqual("System.Int32", result);
    }

    [TestMethod]
    public void HumanizeTypeName_CustomGenericType_WorksCorrectly()
    {
        var type = typeof(Tuple<int, string, bool>);
        var result = type.HumanizeTypeName();
        Assert.Contains("Tuple", result);
        Assert.Contains("Int32", result);
        Assert.Contains("String", result);
        Assert.Contains("Boolean", result);
    }

    [TestMethod]
    public void HumanizeTypeName_ArrayType_ReturnsCorrectName()
    {
        var type = typeof(int[]);
        var result = type.HumanizeTypeName();
        Assert.Contains("Int32", result);
    }

    [TestMethod]
    public void HumanizeTypeName_Nullable_ReturnsHumanizedName()
    {
        var type = typeof(int?);
        var result = type.HumanizeTypeName();
        Assert.Contains("Nullable", result);
        Assert.Contains("Int32", result);
    }

    [TestMethod]
    public void HumanizeTypeName_ValueTuple_ReturnsHumanizedName()
    {
        var type = typeof((int, string));
        var result = type.HumanizeTypeName();
        Assert.Contains("ValueTuple", result);
        Assert.Contains("Int32", result);
        Assert.Contains("String", result);
    }
}

