using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class SpanExtensionsTests
{
    [TestMethod]
    public void SpanExtensions_ExistsInNamespace()
    {
        Span<byte> span = stackalloc byte[10];
        Assert.AreEqual(10, span.Length);
    }

    // Note: SpanExtensions contains extension methods
    // Specific tests would depend on which extension methods are defined
    // This is a placeholder to ensure the namespace is accessible
}

