using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions.Streams;
using System.IO;
using System.Text;

namespace Maxine.Extensions.Test;

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
    public void NoDisposeStream_Read_ReadsFromUnderlyingStream()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Hello World");
        var memoryStream = new MemoryStream(data);
        var noDisposeStream = new NoDisposeStream(memoryStream);
            
        // Act
        var buffer = new byte[5];
        var bytesRead = noDisposeStream.Read(buffer, 0, 5);
            
        // Assert
        Assert.AreEqual(5, bytesRead);
        Assert.AreEqual("Hello", Encoding.UTF8.GetString(buffer));
    }
        
    [TestMethod]
    public void NoDisposeStream_Write_WritesToUnderlyingStream()
    {
        // Arrange
        var memoryStream = new MemoryStream();
        var noDisposeStream = new NoDisposeStream(memoryStream);
        var data = Encoding.UTF8.GetBytes("Test");
            
        // Act
        noDisposeStream.Write(data, 0, data.Length);
        noDisposeStream.Flush();
            
        // Assert
        var written = memoryStream.ToArray();
        Assert.AreEqual("Test", Encoding.UTF8.GetString(written));
    }
        
    [TestMethod]
    public void NoDisposeStream_Seek_SeeksInUnderlyingStream()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("0123456789");
        var memoryStream = new MemoryStream(data);
        var noDisposeStream = new NoDisposeStream(memoryStream);
            
        // Act
        var newPosition = noDisposeStream.Seek(5, SeekOrigin.Begin);
        var buffer = new byte[1];
        noDisposeStream.Read(buffer, 0, 1);
            
        // Assert
        Assert.AreEqual(5, newPosition);
        Assert.AreEqual((byte)'5', buffer[0]);
    }
        
    [TestMethod]
    public void NoDisposeStream_SetLength_SetsLengthOfUnderlyingStream()
    {
        // Arrange
        var memoryStream = new MemoryStream();
        var noDisposeStream = new NoDisposeStream(memoryStream);
            
        // Act
        noDisposeStream.SetLength(100);
            
        // Assert
        Assert.AreEqual(100, memoryStream.Length);
    }
        
    [TestMethod]
    public void NoDisposeStream_Flush_FlushesUnderlyingStream()
    {
        // Arrange
        var memoryStream = new MemoryStream();
        var noDisposeStream = new NoDisposeStream(memoryStream);
        var data = Encoding.UTF8.GetBytes("Test");
            
        // Act
        noDisposeStream.Write(data, 0, data.Length);
        noDisposeStream.Flush();
            
        // Assert - data should be flushed to underlying stream
        Assert.IsGreaterThan(0, memoryStream.Position);
    }
        
    [TestMethod]
    public void NoDisposeStream_CanRead_ReflectsUnderlyingStream()
    {
        // Arrange
        var memoryStream = new MemoryStream(new byte[10]);
        var noDisposeStream = new NoDisposeStream(memoryStream);
            
        // Assert
        Assert.AreEqual(memoryStream.CanRead, noDisposeStream.CanRead);
    }
        
    [TestMethod]
    public void NoDisposeStream_CanWrite_ReflectsUnderlyingStream()
    {
        // Arrange
        var memoryStream = new MemoryStream();
        var noDisposeStream = new NoDisposeStream(memoryStream);
            
        // Assert
        Assert.AreEqual(memoryStream.CanWrite, noDisposeStream.CanWrite);
    }
        
    [TestMethod]
    public void NoDisposeStream_CanSeek_ReflectsUnderlyingStream()
    {
        // Arrange
        var memoryStream = new MemoryStream();
        var noDisposeStream = new NoDisposeStream(memoryStream);
            
        // Assert
        Assert.AreEqual(memoryStream.CanSeek, noDisposeStream.CanSeek);
    }
        
    [TestMethod]
    public void NoDisposeStream_Length_ReflectsUnderlyingStream()
    {
        // Arrange
        var data = new byte[42];
        var memoryStream = new MemoryStream(data);
        var noDisposeStream = new NoDisposeStream(memoryStream);
            
        // Assert
        Assert.AreEqual(42, noDisposeStream.Length);
    }
        
    [TestMethod]
    public void NoDisposeStream_Position_GetSet_WorksCorrectly()
    {
        // Arrange
        var memoryStream = new MemoryStream(new byte[100]);
        var noDisposeStream = new NoDisposeStream(memoryStream);
            
        // Act
        noDisposeStream.Position = 50;
            
        // Assert
        Assert.AreEqual(50, noDisposeStream.Position);
        Assert.AreEqual(50, memoryStream.Position);
    }
        
    [TestMethod]
    public async Task NoDisposeStream_ReadAsync_ReadsFromUnderlyingStream()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Async Read Test");
        var memoryStream = new MemoryStream(data);
        var noDisposeStream = new NoDisposeStream(memoryStream);
            
        // Act
        var buffer = new byte[5];
        var bytesRead = await noDisposeStream.ReadAsync(buffer, 0, 5);
            
        // Assert
        Assert.AreEqual(5, bytesRead);
        Assert.AreEqual("Async", Encoding.UTF8.GetString(buffer));
    }
        
    [TestMethod]
    public async Task NoDisposeStream_WriteAsync_WritesToUnderlyingStream()
    {
        // Arrange
        var memoryStream = new MemoryStream();
        var noDisposeStream = new NoDisposeStream(memoryStream);
        var data = Encoding.UTF8.GetBytes("Async Write");
            
        // Act
        await noDisposeStream.WriteAsync(data, 0, data.Length);
        await noDisposeStream.FlushAsync();
            
        // Assert
        var written = memoryStream.ToArray();
        Assert.AreEqual("Async Write", Encoding.UTF8.GetString(written));
    }
        
    [TestMethod]
    public void NoDisposeStream_MultipleDispose_DoesNotThrow()
    {
        // Arrange
        var memoryStream = new MemoryStream();
        var noDisposeStream = new NoDisposeStream(memoryStream);
            
        // Act & Assert - Should not throw
        noDisposeStream.Dispose();
        noDisposeStream.Dispose();
            
        Assert.IsTrue(memoryStream.CanRead); // Underlying stream still usable
    }

    [TestMethod]
    public void TestSpanReader_ReadSpanCorrectly()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data");
        var spanReader = new SpanReader(data);
            
        // Act
        var span = spanReader.ReadSpan(data.Length);
            
        // Assert
        CollectionAssert.AreEqual(data, span.ToArray());
    }
}