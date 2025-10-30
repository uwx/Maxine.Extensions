using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Maxine.Extensions.Test
{
    [TestClass]
    public class ImageUtilsTests
    {
        [TestMethod]
        public void TestGetAlpha_WithAlphaChannel()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            using (var bmp = new Bitmap(10, 10, PixelFormat.Format32bppArgb))
            {
                bmp.Save(tempFile);
            }

            // Act
            ImageUtils.GetAlpha(tempFile, out var cutout, out var trans);

            // Assert
            Assert.IsFalse(cutout);
            Assert.IsTrue(trans);

            // Cleanup
            File.Delete(tempFile);
        }

        [TestMethod]
        public void TestGetAlpha_WithoutAlphaChannel()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            using (var bmp = new Bitmap(10, 10, PixelFormat.Format24bppRgb))
            {
                bmp.Save(tempFile);
            }

            // Act
            ImageUtils.GetAlpha(tempFile, out var cutout, out var trans);

            // Assert
            Assert.IsFalse(cutout);
            Assert.IsFalse(trans);

            // Cleanup
            File.Delete(tempFile);
        }
    }
}
