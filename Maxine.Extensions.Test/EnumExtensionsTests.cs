using Maxine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace Maxine.Extensions.Test;

[TestClass]
public class EnumExtensionsTests
{
    private enum TestEnumWithDescriptions
    {
        [System.ComponentModel.Description("First value")]
        First,
        
        [System.ComponentModel.Description("Second value")]
        Second,
        
        Third // No description attribute
    }

    [TestMethod]
    public void GetDescription_WithDescriptionAttribute_ReturnsDescription()
    {
        var result = TestEnumWithDescriptions.First.GetDescription();
        
        Assert.AreEqual("First value", result);
    }

    [TestMethod]
    public void GetDescription_WithoutDescriptionAttribute_ReturnsEnumName()
    {
        var result = TestEnumWithDescriptions.Third.GetDescription();
        
        Assert.AreEqual("Third", result);
    }

    [TestMethod]
    public void GetDescription_MultipleValues_ReturnsCorrectDescriptions()
    {
        Assert.AreEqual("First value", TestEnumWithDescriptions.First.GetDescription());
        Assert.AreEqual("Second value", TestEnumWithDescriptions.Second.GetDescription());
        Assert.AreEqual("Third", TestEnumWithDescriptions.Third.GetDescription());
    }
}
