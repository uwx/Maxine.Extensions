using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions.Collections;
using System.Collections.Generic;

namespace Maxine.Extensions.Test
{
    [TestClass]
    public class CollectionsTests
    {
        [TestMethod]
        public void TestArrayOfVector2_AddAndRetrieve()
        {
            // Arrange
            var array = new ArrayOfVector2(10);
            array[0] = (1, 2);

            // Act
            var value = array[0];

            // Assert
            Assert.AreEqual((1, 2), value);
        }

        [TestMethod]
        public void TestChunkedSequence_AddAndEnumerate()
        {
            // Arrange
            var sequence = new ChunkedSequence<int>(3);
            sequence.Add(1);
            sequence.Add(2);
            sequence.Add(3);
            sequence.Add(4);

            // Act
            var result = new List<int>();
            foreach (var item in sequence)
            {
                result.Add(item);
            }

            // Assert
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4 }, result);
        }

        [TestMethod]
        public void TestCooldownDictionary_AddAndCheckCooldown()
        {
            // Arrange
            var dictionary = new CooldownDictionary<string, int>(1000);
            dictionary["key"] = 42;

            // Act
            var value = dictionary["key"];
            var isOnCooldown = dictionary.IsOnCooldown("key");

            // Assert
            Assert.AreEqual(42, value);
            Assert.IsTrue(isOnCooldown);
        }
    }
}
