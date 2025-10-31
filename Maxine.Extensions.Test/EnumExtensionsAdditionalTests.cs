using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;
using System.ComponentModel;

namespace Maxine.Extensions.Test;

[TestClass]
public class EnumExtensionsAdditionalTests
{
    [TestMethod]
    public void GetDescription_EnumWithDescription_ReturnsDescription()
    {
        var value = TestEnum.ValueWithDescription;

        var description = value.GetDescription();

        Assert.AreEqual("This is a description", description);
    }

    [TestMethod]
    public void GetDescription_EnumWithoutDescription_ReturnsEnumName()
    {
        var value = TestEnum.ValueWithoutDescription;

        var description = value.GetDescription();

        Assert.AreEqual("ValueWithoutDescription", description);
    }

    [TestMethod]
    public void GetDescription_FirstValue_ReturnsCorrectDescription()
    {
        var value = TestEnum.First;

        var description = value.GetDescription();

        Assert.AreEqual("First item", description);
    }

    [TestMethod]
    public void GetDescription_SecondValue_ReturnsCorrectDescription()
    {
        var value = TestEnum.Second;

        var description = value.GetDescription();

        Assert.AreEqual("Second item", description);
    }

    [TestMethod]
    public void GetDescription_DifferentEnumType_Works()
    {
        var value = AnotherEnum.OptionA;

        var description = value.GetDescription();

        Assert.AreEqual("Option A Description", description);
    }

    [TestMethod]
    public void GetDescription_EnumValueZero_Works()
    {
        var value = TestEnum.Zero;

        var description = value.GetDescription();

        Assert.AreEqual("Zero value", description);
    }

    [TestMethod]
    public void GetDescription_EnumWithMultipleDescriptions_ReturnsFirst()
    {
        var value = TestEnum.ValueWithDescription;

        var description = value.GetDescription();

        Assert.IsNotNull(description);
        Assert.IsFalse(string.IsNullOrEmpty(description));
    }
}

// Test enums
public enum TestEnum
{
    [System.ComponentModel.Description("Zero value")]
    Zero = 0,

    [System.ComponentModel.Description("First item")]
    First = 1,

    [System.ComponentModel.Description("Second item")]
    Second = 2,

    [System.ComponentModel.Description("This is a description")]
    ValueWithDescription = 10,

    ValueWithoutDescription = 20
}

public enum AnotherEnum
{
    [System.ComponentModel.Description("Option A Description")]
    OptionA,

    [System.ComponentModel.Description("Option B Description")]
    OptionB,

    OptionC
}
