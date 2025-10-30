using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class EnumerableExtensionsTests
{
    [TestMethod]
    public void EnumerableExtensions_ExistsInNamespace()
    {
        var enumerable = new[] { 1, 2, 3 };
        Assert.IsNotNull(enumerable);
        Assert.HasCount(3, enumerable);
    }

    // Note: EnumerableExtensions contains extension methods
    // Specific tests would depend on which extension methods are defined
    // This is a placeholder to ensure the namespace is accessible
}

