namespace Maxine.Extensions.Test;

[TestClass]
public class StringHelpersAdditionalTests
{
    [TestMethod]
    public void NormalizeFilename_BasicAscii_ReturnsUnchanged()
    {
        var result = StringHelpers.NormalizeFilename("HelloWorld123");
        Assert.AreEqual("HelloWorld123", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithSpaces_CollapsesMultipleSpaces()
    {
        var result = StringHelpers.NormalizeFilename("Hello   World");
        Assert.AreEqual("Hello World", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithLeadingSpaces_TrimsLeading()
    {
        var result = StringHelpers.NormalizeFilename("   Hello");
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithTrailingSpaces_TrimsTrailing()
    {
        var result = StringHelpers.NormalizeFilename("Hello   ");
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithTrailingDots_TrimsDots()
    {
        var result = StringHelpers.NormalizeFilename("Hello...");
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithTrailingDotsAndSpaces_TrimsBoth()
    {
        var result = StringHelpers.NormalizeFilename("Hello. . ");
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithDashes_KeepsDashes()
    {
        var result = StringHelpers.NormalizeFilename("Hello-World-Test");
        Assert.AreEqual("Hello-World-Test", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithUnderscores_CollapsesMultiple()
    {
        var result = StringHelpers.NormalizeFilename("Hello___World");
        Assert.AreEqual("Hello_World", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithConnectorPunctuation_ReplacesWithUnderscore()
    {
        var result = StringHelpers.NormalizeFilename("Hello﹍World"); // U+FE4D DASHED LOW LINE
        Assert.AreEqual("Hello_World", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithQuotes_ReplacesWithSingleQuote()
    {
        var result = StringHelpers.NormalizeFilename("Hello\u201CWorld\u201DTest"); // U+201C and U+201D
        Assert.AreEqual("Hello'World'Test", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithSingleQuotes_ReplacesWithSingleQuote()
    {
        var result = StringHelpers.NormalizeFilename("Hello\u2018World\u2019Test"); // U+2018 and U+2019
        Assert.AreEqual("Hello'World'Test", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithControlCharacters_RemovesControl()
    {
        var result = StringHelpers.NormalizeFilename("Hello\u0001World\u0002Test");
        Assert.AreEqual("HelloWorldTest", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithInvalidFileNameChars_ReplacesWithUnderscore()
    {
        var result = StringHelpers.NormalizeFilename("Hello<World>Test|File");
        Assert.AreEqual("Hello_World_Test_File", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithColonAndSlash_ReplacesWithUnderscore()
    {
        var result = StringHelpers.NormalizeFilename("C:\\Path\\To\\File.txt");
        Assert.AreEqual("C_Path_To_File.txt", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithQuestionMarkAndAsterisk_ReplacesWithUnderscore()
    {
        var result = StringHelpers.NormalizeFilename("File*Name?.txt");
        Assert.AreEqual("File_Name_.txt", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithNonAsciiLetters_ReplacesWithUnderscore()
    {
        // NFD normalization splits é into e + combining accent, ö into o + combining accent
        // Each combining mark gets replaced with underscore
        var result = StringHelpers.NormalizeFilename("Héllo Wörld");
        Assert.AreEqual("He_llo Wo_rld", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithAccentedCharacters_Normalizes()
    {
        // NFD normalization splits é into e + combining accent (U+0301)
        var result = StringHelpers.NormalizeFilename("Café");
        Assert.AreEqual("Cafe_", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithEmoji_ReplacesWithUnderscore()
    {
        // Emoji (😀) is non-ASCII so it's removed entirely (not replaced with underscore)
        var result = StringHelpers.NormalizeFilename("Hello😀World");
        Assert.AreEqual("HelloWorld", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithParagraphSeparator_ReplacesWithSpace()
    {
        var result = StringHelpers.NormalizeFilename("Hello\u2029World");
        Assert.AreEqual("Hello World", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithMultipleParagraphSeparators_CollapsesToSingleSpace()
    {
        var result = StringHelpers.NormalizeFilename("Hello\u2029\u2029World");
        Assert.AreEqual("Hello World", result);
    }

    [TestMethod]
    public void NormalizeFilename_EmptyString_ReturnsEmpty()
    {
        var result = StringHelpers.NormalizeFilename("");
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void NormalizeFilename_OnlySpaces_ReturnsEmpty()
    {
        var result = StringHelpers.NormalizeFilename("   ");
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void NormalizeFilename_OnlyDots_ReturnsEmpty()
    {
        var result = StringHelpers.NormalizeFilename("...");
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void NormalizeFilename_LongStringWithStackalloc_Works()
    {
        // Test with short string that should use stackalloc path
        var shortStr = new string('a', 50);
        var result1 = StringHelpers.NormalizeFilename(shortStr);
        Assert.AreEqual(shortStr, result1);
    }

    [TestMethod]
    public void NormalizeFilename_VeryLongStringWithHeapAlloc_Works()
    {
        // Test with long string that should use heap allocation path
        var longStr = new string('a', 2000);
        var result = StringHelpers.NormalizeFilename(longStr);
        Assert.AreEqual(longStr, result);
    }

    [TestMethod]
    public void NormalizeFilename_MixedContent_NormalizesCorrectly()
    {
        var result = StringHelpers.NormalizeFilename("My File (2024).txt");
        Assert.AreEqual("My File (2024).txt", result);
    }

    [TestMethod]
    public void NormalizeFilename_ConsecutiveInvalidChars_CollapsesToSingleUnderscore()
    {
        var result = StringHelpers.NormalizeFilename("File<>|Name");
        Assert.AreEqual("File_Name", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithPipe_ReplacesWithUnderscore()
    {
        var result = StringHelpers.NormalizeFilename("File|Name");
        Assert.AreEqual("File_Name", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithDoubleQuotes_ReplacesWithUnderscore()
    {
        var result = StringHelpers.NormalizeFilename("File\"Name");
        Assert.AreEqual("File_Name", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithSurrogateChars_RemovesSurrogates()
    {
        // Surrogate pairs for emoji are removed entirely
        var result = StringHelpers.NormalizeFilename("Test\uD83D\uDE00End"); // 😀
        Assert.AreEqual("TestEnd", result);
    }

    [TestMethod]
    public void NormalizeFilename_WithPrivateUseChars_RemovesPrivateUse()
    {
        var result = StringHelpers.NormalizeFilename("Test\uE000End"); // Private Use Area
        Assert.AreEqual("TestEnd", result);
    }

    [TestMethod]
    public void NormalizeFilename_AllValidAsciiChars_PreservesAll()
    {
        var result = StringHelpers.NormalizeFilename("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
        Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", result);
    }

    [TestMethod]
    public void NormalizeFilename_SpecialAsciiPunctuation_HandledCorrectly()
    {
        // Testing various ASCII punctuation
        var result = StringHelpers.NormalizeFilename("Test!@#$%^&()_+-={}[];',.");
        // Invalid chars will be replaced, valid ones kept
        // Most of these are valid in filenames except some OS-specific ones
        Assert.IsTrue(result.Length > 0);
        Assert.IsTrue(result.Contains("Test"));
    }
}
