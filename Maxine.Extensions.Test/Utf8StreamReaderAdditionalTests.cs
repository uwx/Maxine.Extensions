using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using System.Buffers;
using Maxine.Extensions.MemoryPools;

namespace Maxine.Extensions.Test;

[TestClass]
public class Utf8StreamReaderAdditionalTests
{
    [TestMethod]
    public async Task BufferedReadLinesAsync_WithCustomMemoryPool_UsesProvidedPool()
    {
        var text = "line1\nline2\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        using var customPool = new UnmanagedMemoryPool<byte>();

        var lines = new List<string>();
        await foreach (var line in LineReader.BufferedReadLinesAsync(stream, 
            memoryPool: customPool))
        {
            lines.Add(Encoding.UTF8.GetString(line.Span));
        }

        Assert.HasCount(3, lines);
    }

    [TestMethod]
    public async Task BufferedReadLinesAsync_WithCancellation_StopsReading()
    {
        var text = "line1\nline2\nline3\nline4\nline5";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        using var cts = new CancellationTokenSource();

        var lines = new List<string>();
        try
        {
            await foreach (var line in LineReader.BufferedReadLinesAsync(stream, 
                cancellationToken: cts.Token))
            {
                lines.Add(Encoding.UTF8.GetString(line.Span));
                if (lines.Count >= 2)
                {
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        Assert.IsTrue(lines.Count >= 2);
    }

    [TestMethod]
    public async Task BufferedReadLinesAsync_LeaveOpenTrue_DoesNotDisposeStream()
    {
        var text = "line1\nline2";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        await foreach (var _ in LineReader.BufferedReadLinesAsync(stream, leaveOpen: true))
        {
            // Enumerate
        }

        Assert.IsTrue(stream.CanRead);
        stream.Dispose();
    }

    [TestMethod]
    public async Task BufferedReadLinesAsync_LeaveOpenFalse_DisposesStream()
    {
        var text = "line1\nline2";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        await foreach (var _ in LineReader.BufferedReadLinesAsync(stream, leaveOpen: false))
        {
            // Enumerate
        }

        Assert.IsFalse(stream.CanRead);
    }

    [TestMethod]
    public async Task BufferedReadLinesAsync_LineTooLongForInitialBuffer_ResizesBuffer()
    {
        // Create a line that's longer than typical buffer size
        var longLine = new string('a', 5000);
        var text = $"short1\n{longLine}\nshort2";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.BufferedReadLinesAsync(stream))
        {
            lines.Add(Encoding.UTF8.GetString(line.Span));
        }

        Assert.HasCount(3, lines);
        Assert.AreEqual("short1", lines[0]);
        Assert.AreEqual(longLine, lines[1]);
        Assert.AreEqual("short2", lines[2]);
    }

    [TestMethod]
    public async Task BufferedReadLinesAsync_NoTrailingNewline_ReadsLastLine()
    {
        var text = "line1\nline2\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.BufferedReadLinesAsync(stream))
        {
            lines.Add(Encoding.UTF8.GetString(line.Span));
        }

        Assert.HasCount(3, lines);
        Assert.AreEqual("line3", lines[2]);
    }

    [TestMethod]
    public async Task BufferedReadLinesAsync_OnlyCarriageReturn_TrimsToEmpty()
    {
        var text = "line1\n\r\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.BufferedReadLinesAsync(stream, 
            skipEmptyBuffers: false, trimCarriageReturn: true))
        {
            lines.Add(Encoding.UTF8.GetString(line.Span));
        }

        Assert.HasCount(3, lines);
        Assert.AreEqual("line1", lines[0]);
        Assert.AreEqual("", lines[1]);
        Assert.AreEqual("line3", lines[2]);
    }

    [TestMethod]
    public async Task BufferedReadLinesAsync_SkipEmptyAndTrimCR_SkipsEmptyAfterTrim()
    {
        var text = "line1\n\r\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.BufferedReadLinesAsync(stream, 
            skipEmptyBuffers: true, trimCarriageReturn: true))
        {
            lines.Add(Encoding.UTF8.GetString(line.Span));
        }

        Assert.HasCount(2, lines);
        Assert.AreEqual("line1", lines[0]);
        Assert.AreEqual("line3", lines[1]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_WithCallback_NoTrailingNewline()
    {
        var text = "line1\nline2";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> bytes) => Encoding.UTF8.GetString(bytes)))
        {
            lines.Add(line);
        }

        Assert.HasCount(2, lines);
        Assert.AreEqual("line2", lines[1]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_SkipEmptyFalse_IncludesEmptyLines()
    {
        var text = "line1\n\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> bytes) => Encoding.UTF8.GetString(bytes),
            skipEmptyBuffers: false))
        {
            lines.Add(line);
        }

        Assert.HasCount(3, lines);
        Assert.AreEqual("", lines[1]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_TrimCRFalse_PreservesCarriageReturn()
    {
        var text = "line1\r\nline2\r\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> bytes) => Encoding.UTF8.GetString(bytes),
            trimCarriageReturn: false))
        {
            lines.Add(line);
        }

        Assert.HasCount(2, lines);
        Assert.AreEqual("line1\r", lines[0]);
        Assert.AreEqual("line2\r", lines[1]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_OnlyCRLine_TrimsToEmpty()
    {
        var text = "line1\n\r\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> bytes) => Encoding.UTF8.GetString(bytes),
            skipEmptyBuffers: false,
            trimCarriageReturn: true))
        {
            lines.Add(line);
        }

        Assert.HasCount(3, lines);
        Assert.AreEqual("", lines[1]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_SkipEmptyAndTrimCR_SkipsLineWithOnlyCR()
    {
        var text = "line1\n\r\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> bytes) => Encoding.UTF8.GetString(bytes),
            skipEmptyBuffers: true,
            trimCarriageReturn: true))
        {
            lines.Add(line);
        }

        Assert.HasCount(2, lines);
        Assert.AreEqual("line1", lines[0]);
        Assert.AreEqual("line3", lines[1]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_LeaveOpenFalse_DisposesStream()
    {
        var text = "line1\nline2";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        await foreach (var _ in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> bytes) => bytes.Length,
            leaveOpen: false))
        {
            // Enumerate
        }

        Assert.IsFalse(stream.CanRead);
    }

    [TestMethod]
    public async Task ReadLinesAsync_WithCancellation_CancelsReading()
    {
        var text = "line1\nline2\nline3\nline4\nline5";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        using var cts = new CancellationTokenSource();

        var count = 0;
        try
        {
            await foreach (var _ in LineReader.ReadLinesAsync(stream,
                (in ReadOnlySpan<byte> bytes) => bytes.Length,
                cancellationToken: cts.Token))
            {
                count++;
                if (count >= 2)
                {
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        Assert.IsTrue(count >= 2);
    }

    [TestMethod]
    public async Task ReadLinesAsync_VeryLargeFile_StreamsEfficiently()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 50000; i++)
        {
            sb.AppendLine($"Line number {i} with some extra text");
        }

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));

        int count = 0;
        await foreach (var _ in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> bytes) => bytes.Length))
        {
            count++;
        }

        Assert.AreEqual(50000, count);
    }

    [TestMethod]
    public async Task ReadLinesAsync_MultipleEmptyLines_HandlesCorrectly()
    {
        var text = "line1\n\n\n\nline5";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> bytes) => Encoding.UTF8.GetString(bytes),
            skipEmptyBuffers: false))
        {
            lines.Add(line);
        }

        Assert.HasCount(5, lines);
        Assert.AreEqual("line1", lines[0]);
        Assert.AreEqual("", lines[1]);
        Assert.AreEqual("", lines[2]);
        Assert.AreEqual("", lines[3]);
        Assert.AreEqual("line5", lines[4]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_BinaryData_HandlesCorrectly()
    {
        var bytes = new byte[] { 0x00, 0x01, 0x02, (byte)'\n', 0x03, 0x04, (byte)'\n' };
        using var stream = new MemoryStream(bytes);

        var lines = new List<byte[]>();
        await foreach (var line in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> b) => b.ToArray()))
        {
            lines.Add(line);
        }

        Assert.HasCount(2, lines);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x01, 0x02 }, lines[0]);
        CollectionAssert.AreEqual(new byte[] { 0x03, 0x04 }, lines[1]);
    }

    private struct StringLineCallback : LineReader.ILineCallback<string>
    {
        public static string Apply(in ReadOnlySpan<byte> bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }

    [TestMethod]
    public async Task ReadLinesAsync_WithStaticCallback_ProcessesLines()
    {
        var text = "line1\nline2\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync<string, StringLineCallback>(stream))
        {
            lines.Add(line);
        }

        Assert.HasCount(3, lines);
        Assert.AreEqual("line1", lines[0]);
        Assert.AreEqual("line2", lines[1]);
        Assert.AreEqual("line3", lines[2]);
    }

    private struct LengthLineCallback : LineReader.ILineCallback<int>
    {
        public static int Apply(in ReadOnlySpan<byte> bytes)
        {
            return bytes.Length;
        }
    }

    [TestMethod]
    public async Task ReadLinesAsync_WithStaticCallback_ReturnsLengths()
    {
        var text = "a\nbb\nccc";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lengths = new List<int>();
        await foreach (var length in LineReader.ReadLinesAsync<int, LengthLineCallback>(stream))
        {
            lengths.Add(length);
        }

        Assert.HasCount(3, lengths);
        Assert.AreEqual(1, lengths[0]);
        Assert.AreEqual(2, lengths[1]);
        Assert.AreEqual(3, lengths[2]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_StaticCallback_SkipsEmptyLines()
    {
        var text = "line1\n\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync<string, StringLineCallback>(
            stream, skipEmptyBuffers: true))
        {
            lines.Add(line);
        }

        Assert.HasCount(2, lines);
    }

    [TestMethod]
    public async Task ReadLinesAsync_StaticCallback_IncludesEmptyLines()
    {
        var text = "line1\n\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync<string, StringLineCallback>(
            stream, skipEmptyBuffers: false))
        {
            lines.Add(line);
        }

        Assert.HasCount(3, lines);
        Assert.AreEqual("", lines[1]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_StaticCallback_TrimsCR()
    {
        var text = "line1\r\nline2\r\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync<string, StringLineCallback>(
            stream, trimCarriageReturn: true))
        {
            lines.Add(line);
        }

        Assert.HasCount(2, lines);
        Assert.AreEqual("line1", lines[0]);
        Assert.AreEqual("line2", lines[1]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_StaticCallback_PreservesCR()
    {
        var text = "line1\r\nline2\r\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync<string, StringLineCallback>(
            stream, trimCarriageReturn: false))
        {
            lines.Add(line);
        }

        Assert.HasCount(2, lines);
        Assert.AreEqual("line1\r", lines[0]);
        Assert.AreEqual("line2\r", lines[1]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_StaticCallback_LeaveOpenTrue()
    {
        var text = "line1\nline2";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        await foreach (var _ in LineReader.ReadLinesAsync<string, StringLineCallback>(
            stream, leaveOpen: true))
        {
            // Enumerate
        }

        Assert.IsTrue(stream.CanRead);
        stream.Dispose();
    }

    [TestMethod]
    public async Task ReadLinesAsync_StaticCallback_LeaveOpenFalse()
    {
        var text = "line1\nline2";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        await foreach (var _ in LineReader.ReadLinesAsync<string, StringLineCallback>(
            stream, leaveOpen: false))
        {
            // Enumerate
        }

        Assert.IsFalse(stream.CanRead);
    }

    [TestMethod]
    public async Task ReadLinesAsync_StaticCallback_NoTrailingNewline()
    {
        var text = "line1\nline2";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync<string, StringLineCallback>(stream))
        {
            lines.Add(line);
        }

        Assert.HasCount(2, lines);
        Assert.AreEqual("line2", lines[1]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_StaticCallback_EmptyStream()
    {
        using var stream = new MemoryStream();

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync<string, StringLineCallback>(stream))
        {
            lines.Add(line);
        }

        Assert.IsEmpty(lines);
    }

    [TestMethod]
    public async Task ReadLinesAsync_StaticCallback_WithCancellation()
    {
        var text = "line1\nline2\nline3\nline4\nline5";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        using var cts = new CancellationTokenSource();

        var count = 0;
        try
        {
            await foreach (var _ in LineReader.ReadLinesAsync<string, StringLineCallback>(
                stream, cancellationToken: cts.Token))
            {
                count++;
                if (count >= 2)
                {
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        Assert.IsTrue(count >= 2);
    }

    [TestMethod]
    public async Task ReadLinesAsync_StaticCallback_SkipsEmptyAfterTrimCR()
    {
        var text = "line1\n\r\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync<string, StringLineCallback>(
            stream, skipEmptyBuffers: true, trimCarriageReturn: true))
        {
            lines.Add(line);
        }

        Assert.HasCount(2, lines);
        Assert.AreEqual("line1", lines[0]);
        Assert.AreEqual("line3", lines[1]);
    }
}
