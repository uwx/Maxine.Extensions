using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions.Math;
using System;

namespace Maxine.Extensions.Test
{
    [TestClass]
    public class MathTests
    {
        [TestMethod]
        public void TestAngle_ToRadians()
        {
            // Arrange
            var angle = new Angle(180);

            // Act
            var radians = angle.ToRadians();

            // Assert
            Assert.AreEqual(Math.PI, radians, 1e-6);
        }

        [TestMethod]
        public void TestBoundingBox_ContainsPoint()
        {
            // Arrange
            var box = new BoundingBox(0, 0, 10, 10);

            // Act
            var contains = box.Contains(5, 5);

            // Assert
            Assert.IsTrue(contains);
        }

        [TestMethod]
        public void TestRomanUtilities_ToRoman()
        {
            // Arrange
            int number = 1987;

            // Act
            var roman = RomanUtilities.ToRoman(number);

            // Assert
            Assert.AreEqual("MCMLXXXVII", roman);
        }
    }
}
