using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;

namespace Maxine.Extensions.Test;

[TestClass]
public class Utf8StreamReaderTests
{
    [TestMethod]
    public async Task BufferedReadLinesAsync_SimpleText_ReadsLines()
    {
        var text = "line1\nline2\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.BufferedReadLinesAsync(stream))
        {
            lines.Add(Encoding.UTF8.GetString(line.Span));
        }

        Assert.HasCount(3, lines);
        Assert.AreEqual("line1", lines[0]);
        Assert.AreEqual("line2", lines[1]);
        Assert.AreEqual("line3", lines[2]);
    }

    [TestMethod]
    public async Task BufferedReadLinesAsync_WithCarriageReturn_TrimsCarriageReturn()
    {
        var text = "line1\r\nline2\r\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.BufferedReadLinesAsync(stream, trimCarriageReturn: true))
        {
            lines.Add(Encoding.UTF8.GetString(line.Span));
        }

        Assert.HasCount(3, lines);
        Assert.AreEqual("line1", lines[0]);
        Assert.AreEqual("line2", lines[1]);
        Assert.AreEqual("line3", lines[2]);
    }

    [TestMethod]
    public async Task BufferedReadLinesAsync_EmptyLines_SkipsWhenConfigured()
    {
        var text = "line1\n\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.BufferedReadLinesAsync(stream, skipEmptyBuffers: true))
        {
            lines.Add(Encoding.UTF8.GetString(line.Span));
        }

        Assert.HasCount(2, lines);
        Assert.AreEqual("line1", lines[0]);
        Assert.AreEqual("line3", lines[1]);
    }

    [TestMethod]
    public async Task BufferedReadLinesAsync_EmptyLines_IncludesWhenConfigured()
    {
        var text = "line1\n\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<string>();
        await foreach (var line in LineReader.BufferedReadLinesAsync(stream, skipEmptyBuffers: false))
        {
            lines.Add(Encoding.UTF8.GetString(line.Span));
        }

        Assert.HasCount(3, lines);
        Assert.AreEqual("line1", lines[0]);
        Assert.AreEqual("", lines[1]);
        Assert.AreEqual("line3", lines[2]);
    }

    [TestMethod]
    public async Task ReadLinesAsync_WithCallback_ProcessesLines()
    {
        var text = "line1\nline2\nline3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var lines = new List<int>();
        await foreach (var length in LineReader.ReadLinesAsync(stream, 
            (in ReadOnlySpan<byte> bytes) => bytes.Length))
        {
            lines.Add(length);
        }

        Assert.HasCount(3, lines);
        Assert.AreEqual(5, lines[0]); // "line1"
        Assert.AreEqual(5, lines[1]); // "line2"
        Assert.AreEqual(5, lines[2]); // "line3"
    }

    [TestMethod]
    public async Task ReadLinesAsync_LargeFile_HandlesCorrectly()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 10000; i++)
        {
            sb.AppendLine($"Line {i}");
        }
        
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));

        int count = 0;
        await foreach (var line in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> bytes) => bytes.Length))
        {
            count++;
        }

        Assert.AreEqual(10000, count);
    }

    [TestMethod]
    public async Task ReadLinesAsync_EmptyStream_ReturnsNoLines()
    {
        using var stream = new MemoryStream();

        var lines = new List<string>();
        await foreach (var line in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> bytes) => Encoding.UTF8.GetString(bytes)))
        {
            lines.Add(line);
        }

        Assert.IsEmpty(lines);
    }

    [TestMethod]
    public async Task ReadLinesAsync_LeaveOpen_DoesNotDisposeStream()
    {
        var text = "line1\nline2";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        await foreach (var _ in LineReader.ReadLinesAsync(stream,
            (in ReadOnlySpan<byte> bytes) => bytes.Length,
            leaveOpen: true))
        {
            // Just enumerate
        }

        // Stream should still be usable
        Assert.IsTrue(stream.CanRead);
        stream.Dispose();
    }

    [TestMethod]
    public async Task BufferedReadLinesAsync_CancellationToken_StopsReading()
    {
        var text = string.Join("\n", Enumerable.Range(0, 1000).Select(i => $"line{i}"));
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        using var cts = new CancellationTokenSource();

        var lines = new List<string>();
        try
        {
            await foreach (var line in LineReader.BufferedReadLinesAsync(stream, 
                cancellationToken: cts.Token))
            {
                lines.Add(Encoding.UTF8.GetString(line.Span));
                if (lines.Count == 10)
                {
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        Assert.IsLessThanOrEqualTo(10, lines.Count);
    }
}

