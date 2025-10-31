using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;
using System.Buffers;

namespace Maxine.Extensions.Test;

[TestClass]
public class IoUtilsAdditionalTests
{
    private string _testFilePath = null!;

    [TestInitialize]
    public void Setup()
    {
        _testFilePath = Path.GetTempFileName();
        File.WriteAllBytes(_testFilePath, [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [TestMethod]
    public void ReadAllBytesToPool_ReadsFileCorrectly()
    {
        var pool = ArrayPool<byte>.Shared;
        var buffer = IoUtils.ReadAllBytesToPool(_testFilePath, pool);

        try
        {
            Assert.AreEqual(10, File.ReadAllBytes(_testFilePath).Length);
            Assert.AreEqual(1, buffer[0]);
            Assert.AreEqual(10, buffer[9]);
        }
        finally
        {
            pool.Return(buffer);
        }
    }

    [TestMethod]
    public void ReadAllBytesToPool_UsesDefaultPool_WhenNullProvided()
    {
        var buffer = IoUtils.ReadAllBytesToPool(_testFilePath, null);

        try
        {
            Assert.IsNotNull(buffer);
            Assert.AreEqual(1, buffer[0]);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    [TestMethod]
    public void ReadAllBytesToPool_EmptyFile_ReturnsEmptyBuffer()
    {
        var emptyFile = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(emptyFile, []);
            
            var pool = ArrayPool<byte>.Shared;
            var buffer = IoUtils.ReadAllBytesToPool(emptyFile, pool);

            try
            {
                Assert.IsNotNull(buffer);
            }
            finally
            {
                pool.Return(buffer);
            }
        }
        finally
        {
            File.Delete(emptyFile);
        }
    }

    [TestMethod]
    public void ReadAllBytesToPool_FileNotFound_ThrowsException()
    {
        try
        {
            IoUtils.ReadAllBytesToPool("nonexistent_file.txt");
            Assert.Fail("Expected FileNotFoundException");
        }
        catch (FileNotFoundException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task ReadAllBytesToPoolAsync_ReadsFileCorrectly()
    {
        var pool = ArrayPool<byte>.Shared;
        var buffer = await IoUtils.ReadAllBytesToPoolAsync(_testFilePath, pool);

        try
        {
            Assert.AreEqual(1, buffer[0]);
            Assert.AreEqual(10, buffer[9]);
        }
        finally
        {
            pool.Return(buffer);
        }
    }

    [TestMethod]
    public async Task ReadAllBytesToPoolAsync_UsesDefaultPool_WhenNullProvided()
    {
        var buffer = await IoUtils.ReadAllBytesToPoolAsync(_testFilePath, null);

        try
        {
            Assert.IsNotNull(buffer);
            Assert.AreEqual(1, buffer[0]);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    [TestMethod]
    public async Task ReadAllBytesToPoolAsync_EmptyFile_ReturnsEmptyBuffer()
    {
        var emptyFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllBytesAsync(emptyFile, []);
            
            var pool = ArrayPool<byte>.Shared;
            var buffer = await IoUtils.ReadAllBytesToPoolAsync(emptyFile, pool);

            try
            {
                Assert.IsNotNull(buffer);
            }
            finally
            {
                pool.Return(buffer);
            }
        }
        finally
        {
            File.Delete(emptyFile);
        }
    }

    [TestMethod]
    public async Task ReadAllBytesToPoolAsync_FileNotFound_ThrowsException()
    {
        try
        {
            await IoUtils.ReadAllBytesToPoolAsync("nonexistent_file.txt");
            Assert.Fail("Expected FileNotFoundException");
        }
        catch (FileNotFoundException)
        {
            // Expected
        }
    }

    [TestMethod]
    public async Task ReadAllBytesToPoolAsync_Cancellation_ThrowsOperationCanceledException()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        try
        {
            await IoUtils.ReadAllBytesToPoolAsync(_testFilePath, cancellationToken: cts.Token);
            Assert.Fail("Expected OperationCanceledException");
        }
        catch (OperationCanceledException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void ReadAllBytesToPool_CustomPool_UsesProvidedPool()
    {
        var customPool = ArrayPool<byte>.Create();
        var buffer = IoUtils.ReadAllBytesToPool(_testFilePath, customPool);

        try
        {
            Assert.IsNotNull(buffer);
            Assert.AreEqual(1, buffer[0]);
        }
        finally
        {
            customPool.Return(buffer);
        }
    }

    [TestMethod]
    public async Task ReadAllBytesToPoolAsync_CustomPool_UsesProvidedPool()
    {
        var customPool = ArrayPool<byte>.Create();
        var buffer = await IoUtils.ReadAllBytesToPoolAsync(_testFilePath, customPool);

        try
        {
            Assert.IsNotNull(buffer);
            Assert.AreEqual(1, buffer[0]);
        }
        finally
        {
            customPool.Return(buffer);
        }
    }
}
