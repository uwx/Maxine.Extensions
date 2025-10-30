using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Maxine.Extensions.Test;

[TestClass]
public class MathTests
{
    [TestMethod]
    public void TestAngle_ToRadians()
    {
        // Arrange
        var angle = Angle<double>.FromDegrees(180);

        // Act
        var radians = angle.Radians;

        // Assert
        Assert.AreEqual(Math.PI, radians, 1e-6);
    }

    [TestMethod]
    public void TestBoundingBox_ContainsPoint()
    {
        // Arrange
        var box = new BoundingBox<double>(0, 0, 10, 10);

        // Act
        var contains = BoundingBox<double>.Contains(box, 5, 5);

        // Assert
        Assert.IsTrue(contains);
    }

    [TestMethod]
    public void TestRomanUtilities_ToRoman()
    {
        // Arrange
        int number = 1987;

        // Act
        var roman = RomanUtilities.ToRomanNumeral(number);

        // Assert
        Assert.AreEqual("MCMLXXXVII", roman);
    }
}