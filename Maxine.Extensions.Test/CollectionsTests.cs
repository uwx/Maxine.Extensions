using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Maxine.Extensions.Test;

[TestClass]
public class CollectionsTests
{
    [TestMethod]
    public void TestArrayOfVector2_AddAndRetrieve()
    {
        // Arrange
        var array = new ArrayOfVector2(10);
        array[0] = new Vector2(1, 2);

        // Act
        var value = array[0];

        // Assert
        Assert.AreEqual(new Vector2(1, 2), value);
    }

    [TestMethod]
    public void TestChunkedSequence_AddAndEnumerate()
    {
        // Arrange
        var sequence = new ChunkedSequence<int>();
        sequence.Append(new[] {1});
        sequence.Append(new[] {2});
        sequence.Append(new[] {3});
        sequence.Append(new[] {4});

        // Act
        var result = new List<int>();
        foreach (var item in sequence.GetSequence())
        {
            result.AddRange(item.Span);
        }

        // Assert
        CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4 }, result);
    }
}