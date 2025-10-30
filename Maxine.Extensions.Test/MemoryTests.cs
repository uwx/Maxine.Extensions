using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Maxine.Extensions.Test;

[TestClass]
public class MemoryTests
{
    [TestMethod]
    public void TestNativeMemoryStream_WriteAndRead()
    {
        // Arrange
        var stream = new NativeMemoryStream(5);
        var data = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        stream.Write(data, 0, data.Length);
        stream.Position = 0;
        var buffer = new byte[data.Length];
        Assert.HasCount(stream.Read(buffer, 0, buffer.Length), buffer);

        // Assert
        CollectionAssert.AreEqual(data, buffer);
    }

    [TestMethod]
    public void TestValueStringBuilder_AppendAndToString()
    {
        // Arrange
        var builder = new ValueStringBuilder();

        // Act
        builder.Append("Hello");
        builder.Append(" ");
        builder.Append("World");
        var result = builder.ToString();

        // Assert
        Assert.AreEqual("Hello World", result);
    }
}