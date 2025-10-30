using Maxine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class FileSizeUtilsTests
{
    [TestMethod]
    public void HumanizeFileSize_ZeroBytes_ReturnsZeroB()
    {
        var result = FileSizeUtils.HumanizeFileSize(0);
        Assert.AreEqual("0 B", result);
    }

    [TestMethod]
    public void HumanizeFileSize_BytesUnder1KB_ReturnsBytesWithB()
    {
        var result = FileSizeUtils.HumanizeFileSize(512);
        Assert.AreEqual("512 B", result);
    }

    [TestMethod]
    public void HumanizeFileSize_1024Bytes_Returns1KiB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024);
        Assert.AreEqual("1 KiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_1024BytesDecimal_Returns1point02KB()
    {
        var result = FileSizeUtils.HumanizeFileSize<double>(1024, isDecimal: true);
        Assert.AreEqual("1.02 KB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_1024BytesDecimal_RoundsIfInt()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024, isDecimal: true);
        Assert.AreEqual("1 KB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_1MB_ReturnsCorrectMiB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024 * 1024);
        Assert.AreEqual("1 MiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_1GBBinary_ReturnsCorrectGiB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024L * 1024 * 1024);
        Assert.AreEqual("1 GiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_1GBDecimal_ReturnsCorrectGB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1_000_000_000L, isDecimal: true);
        Assert.AreEqual("1 GB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_LargeNumber_ReturnsTiB()
    {
        var result = FileSizeUtils.HumanizeFileSize(5L * 1024 * 1024 * 1024 * 1024);
        Assert.AreEqual("5 TiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_FractionalSize_ShowsDecimals()
    {
        var result = FileSizeUtils.HumanizeFileSize<double>(1536); // 1.5 KiB
        Assert.AreEqual("1.5 KiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_FractionalSize_ShowsNoDecimalsIfInt()
    {
        var result = FileSizeUtils.HumanizeFileSize(1536); // 1.5 KiB
        Assert.AreEqual("1 KiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_WithDoubleType_WorksCorrectly()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024.0);
        Assert.AreEqual("1 KiB", result);
    }
}

