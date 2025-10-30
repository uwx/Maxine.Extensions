using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;

namespace Maxine.Extensions.Test;

[TestClass]
[SupportedOSPlatform("windows")]
public class ImageUtilsTests
{
    [TestMethod]
    public void TestGetAlpha_NoAlphaFormat_ReturnsFalseFalse()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + ".bmp";
        try
        {
            using (var bmp = new Bitmap(10, 10, PixelFormat.Format24bppRgb))
            {
                // Fill with solid color (no alpha)
                using (var g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Red);
                }
                bmp.Save(tempFile, ImageFormat.Bmp);
            }

            // Act
            ImageUtils.GetAlpha(tempFile, out var cutout, out var trans);

            // Assert - no alpha channel, so both should be false
            Assert.IsFalse(cutout);
            Assert.IsFalse(trans);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void TestGetAlpha_FullyOpaque_ReturnsFalseFalse()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + ".png";
        try
        {
            using (var bmp = new Bitmap(10, 10, PixelFormat.Format32bppArgb))
            {
                // Fill with fully opaque pixels (alpha = 255)
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(255, 255, 0, 0)); // Fully opaque red
                    }
                }
                bmp.Save(tempFile, ImageFormat.Png);
            }

            // Act
            ImageUtils.GetAlpha(tempFile, out var cutout, out var trans);

            // Assert - all pixels fully opaque
            Assert.IsFalse(cutout);
            Assert.IsFalse(trans);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void TestGetAlpha_WithTransparency_ReturnsFalseTrue()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + ".png";
        try
        {
            using (var bmp = new Bitmap(10, 10, PixelFormat.Format32bppArgb))
            {
                // Fill most with opaque, but add semi-transparent pixel
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(255, 255, 0, 0)); // Opaque
                    }
                }
                // Add one semi-transparent pixel (alpha between 0 and 255)
                bmp.SetPixel(5, 5, Color.FromArgb(128, 255, 0, 0)); // Semi-transparent
                    
                bmp.Save(tempFile, ImageFormat.Png);
            }

            // Act
            ImageUtils.GetAlpha(tempFile, out var cutout, out var trans);

            // Assert - has transparency (alpha not 0 or 255)
            Assert.IsFalse(cutout); // No fully transparent pixels
            Assert.IsTrue(trans);   // Has semi-transparent pixels
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void TestGetAlpha_WithCutout_ReturnsTrueFalse()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + ".png";
        try
        {
            using (var bmp = new Bitmap(10, 10, PixelFormat.Format32bppArgb))
            {
                // Fill with opaque pixels
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(255, 255, 0, 0)); // Opaque
                    }
                }
                // Add fully transparent pixel (alpha = 0)
                bmp.SetPixel(5, 5, Color.FromArgb(0, 255, 0, 0)); // Fully transparent
                    
                bmp.Save(tempFile, ImageFormat.Png);
            }

            // Act
            ImageUtils.GetAlpha(tempFile, out var cutout, out var trans);

            // Assert - has cutout (fully transparent) but no semi-transparency
            Assert.IsTrue(cutout);  // Has fully transparent pixels
            Assert.IsFalse(trans);  // No semi-transparent pixels detected yet
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void TestGetAlpha_WithBothCutoutAndTransparency_ReturnsTrueTrue()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + ".png";
        try
        {
            using (var bmp = new Bitmap(10, 10, PixelFormat.Format32bppArgb))
            {
                // Fill with opaque pixels
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(255, 255, 0, 0)); // Opaque
                    }
                }
                // Add fully transparent pixel
                bmp.SetPixel(3, 3, Color.FromArgb(0, 255, 0, 0)); // Fully transparent
                // Add semi-transparent pixel
                bmp.SetPixel(5, 5, Color.FromArgb(128, 255, 0, 0)); // Semi-transparent
                    
                bmp.Save(tempFile, ImageFormat.Png);
            }

            // Act
            ImageUtils.GetAlpha(tempFile, out var cutout, out var trans);

            // Assert - has both types of alpha
            Assert.IsTrue(cutout);  // Has fully transparent pixels
            Assert.IsTrue(trans);   // Has semi-transparent pixels
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}