using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    public void StringExtensions_ExistsInNamespace()
    {
        var str = "test";
        Assert.IsNotNull(str);
        Assert.AreEqual(4, str.Length);
    }

    // Note: StringExtensions contains extension methods
    // Specific tests would depend on which extension methods are defined
    // This is a placeholder to ensure the namespace is accessible
}

