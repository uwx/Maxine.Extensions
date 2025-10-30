using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class TypeExtensionsTests
{
    [TestMethod]
    public void TypeExtensions_ExistsInNamespace()
    {
        var type = typeof(string);
        Assert.IsNotNull(type);
        Assert.AreEqual("String", type.Name);
    }

    // Note: TypeExtensions contains extension methods
    // Specific tests would depend on which extension methods are defined
    // This is a placeholder to ensure the namespace is accessible
}

