using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions.Streams;
using System.Text;

namespace Maxine.Extensions.Test;

[TestClass]
public class NoDisposeStreamAdditionalTests
{
    [TestMethod]
    public void NoDisposeStream_Read_ReadsFromUnderlying()
    {
        var data = Encoding.UTF8.GetBytes("Hello World");
        var baseStream = new MemoryStream(data);
        var noDispose = new NoDisposeStream(baseStream);

        var buffer = new byte[5];
        var bytesRead = noDispose.Read(buffer, 0, 5);

        Assert.AreEqual(5, bytesRead);
        Assert.AreEqual("Hello", Encoding.UTF8.GetString(buffer));
    }

    [TestMethod]
    public void NoDisposeStream_Write_WritesToUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        var data = Encoding.UTF8.GetBytes("Test");
        noDispose.Write(data, 0, data.Length);

        Assert.AreEqual(4, baseStream.Length);
        Assert.AreEqual("Test", Encoding.UTF8.GetString(baseStream.ToArray()));
    }

    [TestMethod]
    public void NoDisposeStream_Seek_SeeksUnderlying()
    {
        var data = Encoding.UTF8.GetBytes("Hello World");
        var baseStream = new MemoryStream(data);
        var noDispose = new NoDisposeStream(baseStream);

        var newPos = noDispose.Seek(6, SeekOrigin.Begin);

        Assert.AreEqual(6, newPos);
        Assert.AreEqual(6, baseStream.Position);
    }

    [TestMethod]
    public void NoDisposeStream_Position_GetSet_ReflectsUnderlying()
    {
        var baseStream = new MemoryStream(new byte[100]);
        var noDispose = new NoDisposeStream(baseStream);

        noDispose.Position = 42;

        Assert.AreEqual(42, noDispose.Position);
        Assert.AreEqual(42, baseStream.Position);
    }

    [TestMethod]
    public void NoDisposeStream_Length_ReflectsUnderlying()
    {
        var data = new byte[50];
        var baseStream = new MemoryStream(data);
        var noDispose = new NoDisposeStream(baseStream);

        Assert.AreEqual(50, noDispose.Length);
    }

    [TestMethod]
    public void NoDisposeStream_SetLength_SetsUnderlyingLength()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        noDispose.SetLength(100);

        Assert.AreEqual(100, baseStream.Length);
        Assert.AreEqual(100, noDispose.Length);
    }

    [TestMethod]
    public void NoDisposeStream_Flush_FlushesUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        noDispose.Write(new byte[] { 1, 2, 3 }, 0, 3);
        noDispose.Flush();

        // Verify flush was called (no exception thrown)
        Assert.AreEqual(3, baseStream.Length);
    }

    [TestMethod]
    public void NoDisposeStream_CanRead_ReflectsUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        Assert.AreEqual(baseStream.CanRead, noDispose.CanRead);
    }

    [TestMethod]
    public void NoDisposeStream_CanWrite_ReflectsUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        Assert.AreEqual(baseStream.CanWrite, noDispose.CanWrite);
    }

    [TestMethod]
    public void NoDisposeStream_CanSeek_ReflectsUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        Assert.AreEqual(baseStream.CanSeek, noDispose.CanSeek);
    }

    [TestMethod]
    public void NoDisposeStream_Dispose_DoesNotDisposeUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        noDispose.Dispose();

        // Underlying stream should still be usable
        Assert.IsTrue(baseStream.CanWrite);
        baseStream.WriteByte(42);
        Assert.AreEqual(1, baseStream.Length);
    }

    [TestMethod]
    public void NoDisposeStream_Close_DoesNotCloseUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        noDispose.Close();

        // Underlying stream should still be usable
        Assert.IsTrue(baseStream.CanWrite);
    }

    [TestMethod]
    public async Task NoDisposeStream_DisposeAsync_DoesNotDisposeUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        await noDispose.DisposeAsync();

        // Underlying stream should still be usable
        Assert.IsTrue(baseStream.CanWrite);
        baseStream.WriteByte(42);
        Assert.AreEqual(1, baseStream.Length);
    }

    [TestMethod]
    public void NoDisposeStream_ReadByte_ReadsFromUnderlying()
    {
        var baseStream = new MemoryStream(new byte[] { 42, 99 });
        var noDispose = new NoDisposeStream(baseStream);

        var value = noDispose.ReadByte();

        Assert.AreEqual(42, value);
    }

    [TestMethod]
    public void NoDisposeStream_WriteByte_WritesToUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        noDispose.WriteByte(42);

        Assert.AreEqual(1, baseStream.Length);
        Assert.AreEqual(42, baseStream.ToArray()[0]);
    }

    [TestMethod]
    public void NoDisposeStream_ReadSpan_ReadsFromUnderlying()
    {
        var data = Encoding.UTF8.GetBytes("Test");
        var baseStream = new MemoryStream(data);
        var noDispose = new NoDisposeStream(baseStream);

        Span<byte> buffer = stackalloc byte[4];
        var bytesRead = noDispose.Read(buffer);

        Assert.AreEqual(4, bytesRead);
        Assert.AreEqual("Test", Encoding.UTF8.GetString(buffer));
    }

    [TestMethod]
    public void NoDisposeStream_WriteSpan_WritesToUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        ReadOnlySpan<byte> data = Encoding.UTF8.GetBytes("Test");
        noDispose.Write(data);

        Assert.AreEqual(4, baseStream.Length);
        Assert.AreEqual("Test", Encoding.UTF8.GetString(baseStream.ToArray()));
    }

    [TestMethod]
    public async Task NoDisposeStream_ReadAsync_ReadsFromUnderlying()
    {
        var data = Encoding.UTF8.GetBytes("Async Test");
        var baseStream = new MemoryStream(data);
        var noDispose = new NoDisposeStream(baseStream);

        var buffer = new byte[5];
        var bytesRead = await noDispose.ReadAsync(buffer, 0, 5);

        Assert.AreEqual(5, bytesRead);
        Assert.AreEqual("Async", Encoding.UTF8.GetString(buffer));
    }

    [TestMethod]
    public async Task NoDisposeStream_WriteAsync_WritesToUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        var data = Encoding.UTF8.GetBytes("Async");
        await noDispose.WriteAsync(data, 0, data.Length);

        Assert.AreEqual(5, baseStream.Length);
        Assert.AreEqual("Async", Encoding.UTF8.GetString(baseStream.ToArray()));
    }

    [TestMethod]
    public async Task NoDisposeStream_FlushAsync_FlushesUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        await noDispose.WriteAsync(new byte[] { 1, 2, 3 }, 0, 3);
        await noDispose.FlushAsync();

        Assert.AreEqual(3, baseStream.Length);
    }

    [TestMethod]
    public void NoDisposeStream_GetHashCode_ReflectsUnderlying()
    {
        var baseStream = new MemoryStream();
        var noDispose = new NoDisposeStream(baseStream);

        Assert.AreEqual(baseStream.GetHashCode(), noDispose.GetHashCode());
    }
}
