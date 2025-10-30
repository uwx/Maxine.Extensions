using Maxine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class ImageUtilsTests
{
    // ImageUtils has Windows-specific methods that require System.Drawing
    // These tests are for Windows platform only
    
    [TestMethod]
    public void ImageUtils_ClassExists()
    {
        // Verify the class can be instantiated
        var imageUtils = new ImageUtils();
        Assert.IsNotNull(imageUtils);
    }

    // Note: GetAlpha method requires actual image files and is Windows-specific
    // Full testing would require platform checks and test image files
    // Placeholder test to ensure class compiles
}

