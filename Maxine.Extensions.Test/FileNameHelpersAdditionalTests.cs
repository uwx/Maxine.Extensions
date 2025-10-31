using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class FileNameHelpersAdditionalTests
{
    [TestMethod]
    public void NormalizeFilename_SimpleAscii_ReturnsUnchanged()
    {
        var result = FileNameHelpers.NormalizeFilename("test");
        Assert.AreEqual("test", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithSpaces_PreservesSpaces()
    {
        var result = FileNameHelpers.NormalizeFilename("my file name");
        Assert.AreEqual("my file name", result);
    }

    [TestMethod]
    public void NormalizeFilename_MultipleSpaces_CollapsesToSingle()
    {
        var result = FileNameHelpers.NormalizeFilename("my    file");
        Assert.AreEqual("my file", result);
    }

    [TestMethod]
    public void NormalizeFilename_LeadingSpaces_TrimsStart()
    {
        var result = FileNameHelpers.NormalizeFilename("   file");
        Assert.AreEqual("file", result);
    }

    [TestMethod]
    public void NormalizeFilename_TrailingSpaces_TrimsEnd()
    {
        var result = FileNameHelpers.NormalizeFilename("file   ");
        Assert.AreEqual("file", result);
    }

    [TestMethod]
    public void NormalizeFilename_TrailingDots_TrimsEnd()
    {
        var result = FileNameHelpers.NormalizeFilename("file...");
        Assert.AreEqual("file", result);
    }

    [TestMethod]
    public void NormalizeFilename_TrailingDotsAndSpaces_TrimsBoth()
    {
        var result = FileNameHelpers.NormalizeFilename("file. . ");
        Assert.AreEqual("file", result);
    }

    [TestMethod]
    public void NormalizeFilename_Dash_PreservesDash()
    {
        var result = FileNameHelpers.NormalizeFilename("my-file");
        Assert.AreEqual("my-file", result);
    }

    [TestMethod]
    public void NormalizeFilename_Underscore_PreservesUnderscore()
    {
        var result = FileNameHelpers.NormalizeFilename("my_file");
        Assert.AreEqual("my_file", result);
    }

    [TestMethod]
    public void NormalizeFilename_MultipleUnderscores_CollapsesToSingle()
    {
        var result = FileNameHelpers.NormalizeFilename("my___file");
        Assert.AreEqual("my_file", result);
    }

    [TestMethod]
    public void NormalizeFilename_SingleQuotes_Preserved()
    {
        var result = FileNameHelpers.NormalizeFilename("it's");
        Assert.AreEqual("it's", result);
    }

    [TestMethod]
    public void NormalizeFilename_InvalidFileNameChars_ReplacedWithUnderscore()
    {
        // Contains invalid chars like <, >, :, ", |, ?, *
        var result = FileNameHelpers.NormalizeFilename("file<>:\"|?*");
        Assert.IsFalse(result.Contains('<'));
        Assert.IsFalse(result.Contains('>'));
        Assert.IsFalse(result.Contains(':'));
        Assert.IsFalse(result.Contains('"'));
        Assert.IsFalse(result.Contains('|'));
        Assert.IsFalse(result.Contains('?'));
        Assert.IsFalse(result.Contains('*'));
    }

    [TestMethod]
    public void NormalizeFilename_AccentedCharacters_NormalizesAndConverts()
    {
        // é, à, ñ should be normalized
        var result = FileNameHelpers.NormalizeFilename("café");
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void NormalizeFilename_EmptyString_ReturnsEmpty()
    {
        var result = FileNameHelpers.NormalizeFilename(string.Empty);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void NormalizeFilename_ControlCharacters_Removed()
    {
        var result = FileNameHelpers.NormalizeFilename("file\t\n\rname");
        Assert.IsFalse(result.Contains('\t'));
        Assert.IsFalse(result.Contains('\n'));
        Assert.IsFalse(result.Contains('\r'));
    }

    [TestMethod]
    public void NormalizeFilename_ParagraphSeparator_ReplacedWithSpace()
    {
        var result = FileNameHelpers.NormalizeFilename("line\u2029another");
        Assert.IsTrue(result.Contains(' '));
    }

    [TestMethod]
    public void NormalizeFilename_ConnectorPunctuation_ReplacedWithUnderscore()
    {
        var result = FileNameHelpers.NormalizeFilename("test\u203Ffile"); // ‿ (undertie)
        Assert.IsTrue(result.Contains('_'));
    }

    [TestMethod]
    public void NormalizeFilename_DashPunctuation_ReplacedWithHyphen()
    {
        var result = FileNameHelpers.NormalizeFilename("test\u2013file"); // – (en dash)
        Assert.IsTrue(result.Contains('-'));
    }

    [TestMethod]
    public void NormalizeFilename_ValidAsciiAndInvalidMixed_KeepsValidOnly()
    {
        var result = FileNameHelpers.NormalizeFilename("valid<invalid>valid");
        Assert.IsTrue(result.Contains("valid"));
        Assert.IsFalse(result.Contains('<'));
        Assert.IsFalse(result.Contains('>'));
    }

    [TestMethod]
    public void NormalizeFilename_NumbersAndLetters_Preserved()
    {
        var result = FileNameHelpers.NormalizeFilename("file123ABC");
        Assert.AreEqual("file123ABC", result);
    }

    [TestMethod]
    public void NormalizeFilename_LongString_ProcessedCorrectly()
    {
        var longName = new string('a', 500);
        var result = FileNameHelpers.NormalizeFilename(longName);
        Assert.AreEqual(longName, result);
    }

    [TestMethod]
    public void NormalizeFilename_MixedValidAndSpaces_NormalizesCorrectly()
    {
        var result = FileNameHelpers.NormalizeFilename("  My  Document  2024  ");
        Assert.AreEqual("My Document 2024", result);
    }

    [TestMethod]
    public void NormalizeFilename_OnlyDots_ReturnsEmpty()
    {
        var result = FileNameHelpers.NormalizeFilename("....");
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void NormalizeFilename_OnlySpaces_ReturnsEmpty()
    {
        var result = FileNameHelpers.NormalizeFilename("     ");
        Assert.AreEqual(string.Empty, result);
    }
}
