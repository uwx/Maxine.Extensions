using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions.Streams;
using System.IO;
using System.Text;

namespace Maxine.Extensions.Test
{
    [TestClass]
    public class StreamsTests
    {
        [TestMethod]
        public void TestNoDisposeStream_DisposeDoesNotCloseStream()
        {
            // Arrange
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Test data"));
            var noDisposeStream = new NoDisposeStream(memoryStream);

            // Act
            noDisposeStream.Dispose();

            // Assert
            Assert.IsTrue(memoryStream.CanRead);
        }

        [TestMethod]
        public void TestSpanReader_ReadSpanCorrectly()
        {
            // Arrange
            var data = Encoding.UTF8.GetBytes("Test data");
            var memoryStream = new MemoryStream(data);
            var spanReader = new SpanReader(memoryStream);

            // Act
            var span = spanReader.ReadSpan(data.Length);

            // Assert
            CollectionAssert.AreEqual(data, span.ToArray());
        }
    }
}
