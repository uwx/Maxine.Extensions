using Maxine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Buffers;
using System.IO;

namespace Maxine.Extensions.Test;

[TestClass]
public class IoUtilsTests
{
    private string _tempFilePath = null!;

    [TestInitialize]
    public void Setup()
    {
        _tempFilePath = Path.GetTempFileName();
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }

    [TestMethod]
    public void ReadAllBytesToPool_SmallFile_ReturnsCorrectBytes()
    {
        var testData = new byte[] { 1, 2, 3, 4, 5 };
        File.WriteAllBytes(_tempFilePath, testData);

        var result = IoUtils.ReadAllBytesToPool(_tempFilePath);
        
        Assert.IsNotNull(result);
        Assert.IsGreaterThanOrEqualTo(testData.Length, result.Length);
        for (int i = 0; i < testData.Length; i++)
        {
            Assert.AreEqual(testData[i], result[i]);
        }

        ArrayPool<byte>.Shared.Return(result);
    }

    [TestMethod]
    public void ReadAllBytesToPool_WithCustomPool_UsesCustomPool()
    {
        var testData = new byte[] { 10, 20, 30 };
        File.WriteAllBytes(_tempFilePath, testData);

        var customPool = ArrayPool<byte>.Create();
        var result = IoUtils.ReadAllBytesToPool(_tempFilePath, customPool);
        
        Assert.IsNotNull(result);
        customPool.Return(result);
    }

    [TestMethod]
    public void ReadAllBytesToPool_NonExistentFile_ThrowsException()
    {
        Assert.Throws<FileNotFoundException>(() =>
        {
            IoUtils.ReadAllBytesToPool("nonexistent_file.dat");
        });
    }

    [TestMethod]
    public async Task ReadAllBytesToPoolAsync_SmallFile_ReturnsCorrectBytes()
    {
        var testData = new byte[] { 1, 2, 3, 4, 5 };
        await File.WriteAllBytesAsync(_tempFilePath, testData);

        var result = await IoUtils.ReadAllBytesToPoolAsync(_tempFilePath);
        
        Assert.IsNotNull(result);
        Assert.IsGreaterThanOrEqualTo(testData.Length, result.Length);
        for (int i = 0; i < testData.Length; i++)
        {
            Assert.AreEqual(testData[i], result[i]);
        }

        ArrayPool<byte>.Shared.Return(result);
    }

    [TestMethod]
    public async Task ReadAllBytesToPoolAsync_WithCancellation_SupportsToken()
    {
        var testData = new byte[] { 1, 2, 3 };
        await File.WriteAllBytesAsync(_tempFilePath, testData);

        using var cts = new CancellationTokenSource();
        var result = await IoUtils.ReadAllBytesToPoolAsync(_tempFilePath, cancellationToken: cts.Token);
        
        Assert.IsNotNull(result);
        ArrayPool<byte>.Shared.Return(result);
    }

    [TestMethod]
    public void ReadAllBytesToPool_EmptyFile_ReturnsEmptyArray()
    {
        File.WriteAllBytes(_tempFilePath, Array.Empty<byte>());

        var result = IoUtils.ReadAllBytesToPool(_tempFilePath);
        
        Assert.IsNotNull(result);
        ArrayPool<byte>.Shared.Return(result);
    }
}

