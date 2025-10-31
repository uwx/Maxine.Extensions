using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class FileSizeUtilsAdditionalTests
{
    [TestMethod]
    public void HumanizeFileSize_ZeroBytes_Binary_ReturnsZeroB()
    {
        var result = FileSizeUtils.HumanizeFileSize(0L, false);
        Assert.AreEqual("0 B", result);
    }

    [TestMethod]
    public void HumanizeFileSize_ZeroBytes_Decimal_ReturnsZeroB()
    {
        var result = FileSizeUtils.HumanizeFileSize(0L, true);
        Assert.AreEqual("0 B", result);
    }

    [TestMethod]
    public void HumanizeFileSize_OneKibibyte_Binary_Returns1KiB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024L, false);
        Assert.AreEqual("1 KiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_OneKilobyte_Decimal_Returns1KB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1000L, true);
        Assert.AreEqual("1 KB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_OneMebibyte_Binary_Returns1MiB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024L * 1024, false);
        Assert.AreEqual("1 MiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_OneMegabyte_Decimal_Returns1MB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1000L * 1000, true);
        Assert.AreEqual("1 MB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_OneGibibyte_Binary_Returns1GiB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024L * 1024 * 1024, false);
        Assert.AreEqual("1 GiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_OneGigabyte_Decimal_Returns1GB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1000L * 1000 * 1000, true);
        Assert.AreEqual("1 GB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_OneTebibyte_Binary_Returns1TiB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024L * 1024 * 1024 * 1024, false);
        Assert.AreEqual("1 TiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_OneTerabyte_Decimal_Returns1TB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1000L * 1000 * 1000 * 1000, true);
        Assert.AreEqual("1 TB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_OnePebibyte_Binary_Returns1PiB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024L * 1024 * 1024 * 1024 * 1024, false);
        Assert.AreEqual("1 PiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_OnePetabyte_Decimal_Returns1PB()
    {
        var result = FileSizeUtils.HumanizeFileSize(1000L * 1000 * 1000 * 1000 * 1000, true);
        Assert.AreEqual("1 PB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_FractionalKibibyte_Binary_ShowsCorrectUnit()
    {
        var result = FileSizeUtils.HumanizeFileSize(1536L, false); // 1.5 KiB
        Assert.IsTrue(result.Contains("KiB"));
    }

    [TestMethod]
    public void HumanizeFileSize_FractionalMegabyte_Decimal_ShowsCorrectUnit()
    {
        var result = FileSizeUtils.HumanizeFileSize(1500000L, true); // 1.5 MB
        Assert.IsTrue(result.Contains("MB"));
    }

    [TestMethod]
    public void HumanizeFileSize_SmallBytes_Binary_ReturnsBytes()
    {
        var result = FileSizeUtils.HumanizeFileSize(512L, false);
        Assert.AreEqual("512 B", result);
    }

    [TestMethod]
    public void HumanizeFileSize_SmallBytes_Decimal_ReturnsBytes()
    {
        var result = FileSizeUtils.HumanizeFileSize(999L, true);
        Assert.AreEqual("999 B", result);
    }

    [TestMethod]
    public void HumanizeFileSize_IntType_Binary_WorksCorrectly()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024, false);
        Assert.AreEqual("1 KiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_DoubleType_Decimal_WorksCorrectly()
    {
        var result = FileSizeUtils.HumanizeFileSize(1000.0, true);
        Assert.AreEqual("1 KB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_DecimalType_Binary_WorksCorrectly()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024m, false);
        Assert.AreEqual("1 KiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_FloatType_Decimal_WorksCorrectly()
    {
        var result = FileSizeUtils.HumanizeFileSize(1000.0f, true);
        Assert.AreEqual("1 KB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_LargeFractional_ShowsCorrectUnit()
    {
        var result = FileSizeUtils.HumanizeFileSize(1234567L, false); // ~1.177 MiB
        Assert.IsTrue(result.Contains("MiB"));
    }

    [TestMethod]
    public void HumanizeFileSize_DefaultParameter_UsesBinary()
    {
        var result = FileSizeUtils.HumanizeFileSize(1024L);
        Assert.AreEqual("1 KiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_VeryLargeNumber_Binary_HandlesPetabytes()
    {
        var result = FileSizeUtils.HumanizeFileSize(2L * 1024 * 1024 * 1024 * 1024 * 1024, false);
        Assert.AreEqual("2 PiB", result);
    }

    [TestMethod]
    public void HumanizeFileSize_VeryLargeNumber_Decimal_HandlesPetabytes()
    {
        var result = FileSizeUtils.HumanizeFileSize(3L * 1000 * 1000 * 1000 * 1000 * 1000, true);
        Assert.AreEqual("3 PB", result);
    }
}
