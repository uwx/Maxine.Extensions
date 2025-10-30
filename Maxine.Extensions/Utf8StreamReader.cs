using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using Maxine.Extensions.MemoryPools;

namespace Maxine.Extensions;

public delegate T LineCallback<out T>(in ReadOnlySpan<byte> bytes);

public static class LineReader
{
    public static async IAsyncEnumerable<ReadOnlyMemory<byte>> BufferedReadLinesAsync(
        Stream stream,
        bool skipEmptyBuffers = true,
        bool leaveOpen = false,
        bool trimCarriageReturn = true,
        MemoryPool<byte>? memoryPool = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = new()
    )
    {
        UnmanagedMemoryPool<byte>? poolToDispose = null;
        var pool = memoryPool ?? (poolToDispose = new UnmanagedMemoryPool<byte>());

        var mem = pool.Rent(pool.MaxBufferSize);

        try
        {
            await foreach (var line in ReadLinesAsync(stream,
                               (in span) =>
                               {
                                   if (!span.TryCopyTo(mem.Memory.Span))
                                   {
                                       mem.Dispose();
                                       mem = pool.Rent(span.Length);

                                       span.CopyTo(mem.Memory.Span);
                                   }

                                   return mem.Memory[..span.Length];
                               }, skipEmptyBuffers, leaveOpen, trimCarriageReturn, cancellationToken))
            {
                yield return line;
            }
        }
        finally
        {
            mem.Dispose();
            poolToDispose?.Dispose();
        }
    }
    
    public static async IAsyncEnumerable<T> ReadLinesAsync<T>(
        Stream stream,
        LineCallback<T> lineCallback,
        bool skipEmptyBuffers = true,
        bool leaveOpen = false,
        bool trimCarriageReturn = true,
        [EnumeratorCancellation] CancellationToken cancellationToken = new())
    {
        var pipe = new Pipe();
        
        var reader = pipe.Reader;
        
        var writing = FillPipeAsync(stream, pipe.Writer);

        while (true)
        {
            var result = await reader.ReadAsync(cancellationToken);
            var buffer = result.Buffer;

            while (TryReadLine(ref buffer, out var lineBytes) && ProcessLine(lineBytes) is { ShouldReturn: true, Result: var t })
            {
                yield return t!;
            }
            
            reader.AdvanceTo(buffer.Start, buffer.End);
            
            if (result.IsCompleted)
            {
                // Process any remaining data in the buffer as the final line (no trailing newline)
                if (buffer.Length > 0 && ProcessLine(buffer.ToArray()) is { ShouldReturn: true, Result: var finalLine })
                {
                    yield return finalLine!;
                }
                break;
            }
        }

        await reader.CompleteAsync();

        await writing;

        if (!leaveOpen)
        {
            await stream.DisposeAsync();
        }

        yield break;

        (bool ShouldReturn, T? Result) ProcessLine(ReadOnlySpan<byte> bytes)
        {
            if (!skipEmptyBuffers || bytes.Length > 0)
            {
                if (trimCarriageReturn && bytes.Length > 0 && bytes[^1] == '\r')
                {
                    bytes = bytes[..^1];

                    if (skipEmptyBuffers && bytes.Length == 0)
                    {
                        return (false, default);
                    }
                }

                return (true, lineCallback(bytes));
            }
            else
            {
                return (false, default);
            }
        }
    }

    public interface ILineCallback<out T>
    {
        public static abstract T Apply(in ReadOnlySpan<byte> bytes);
    }

    public static async IAsyncEnumerable<T> ReadLinesAsync<T, TCallback>(
        Stream stream,
        bool skipEmptyBuffers = true,
        bool leaveOpen = false,
        bool trimCarriageReturn = true,
        [EnumeratorCancellation] CancellationToken cancellationToken = new())
    where TCallback : ILineCallback<T>
    {
        var pipe = new Pipe();
        
        var reader = pipe.Reader;
        
        var writing = FillPipeAsync(stream, pipe.Writer);

        while (true)
        {
            var result = await reader.ReadAsync(cancellationToken);
            var buffer = result.Buffer;

            while (TryReadLine(ref buffer, out var lineBytes) && ProcessLine(lineBytes) is { ShouldReturn: true, Result: var t })
            {
                yield return t!;
            }
            
            reader.AdvanceTo(buffer.Start, buffer.End);
            
            if (result.IsCompleted)
            {
                // Process any remaining data in the buffer as the final line (no trailing newline)
                if (buffer.Length > 0 && ProcessLine(buffer.ToArray()) is { ShouldReturn: true, Result: var finalLine })
                {
                    yield return finalLine!;
                }
                break;
            }
        }

        await reader.CompleteAsync();

        await writing;

        if (!leaveOpen)
        {
            await stream.DisposeAsync();
        }

        yield break;

        (bool ShouldReturn, T? Result) ProcessLine(ReadOnlySpan<byte> bytes)
        {
            if (!skipEmptyBuffers || bytes.Length > 0)
            {
                if (trimCarriageReturn && bytes.Length > 0 && bytes[^1] == '\r')
                {
                    bytes = bytes[..^1];

                    if (skipEmptyBuffers && bytes.Length == 0)
                    {
                        return (false, default);
                    }
                }

                return (true, TCallback.Apply(bytes));
            }
            else
            {
                return (false, default);
            }
        }
    }

    // public static async IAsyncEnumerable<T> Deserialize2<T>(string path)
    // {
    //     using var reader = new StreamReader(File.OpenRead(path));
    //     while (await reader.ReadLineAsync() is {} line)
    //     {
    //         yield return JsonSerializer.Deserialize<T>(line, JsonSerializerOptions)!;
    //     }
    // }
    
    // public static async IAsyncEnumerable<T> Deserialize<T>(string path)
    // {
    //     await using var stream = File.OpenRead(path);
    //     
    //     var pipe = new Pipe();
    //     var reader = pipe.Reader;
    //     
    //     var writing = FillPipeAsync(stream, pipe.Writer);
    //
    //     while (true)
    //     {
    //         var result = await reader.ReadAsync();
    //         var buffer = result.Buffer;
    //
    //         while (ConditionalDeserialize<T>(ReadLine(ref buffer), out var b) is var t && b)
    //         {
    //             yield return t!;
    //         }
    //         
    //         reader.AdvanceTo(buffer.Start, buffer.End);
    //         
    //         if (result.IsCompleted) break;
    //     }
    //
    //     await reader.CompleteAsync();
    //
    //     await writing;
    // }

    private static async ValueTask FillPipeAsync(Stream stream, PipeWriter writer)
    {
        const int minimumBufferSize = 1024*1024;

        while (true)
        {
            // Allocate at least 512 bytes from the PipeWriter
            var memory = writer.GetMemory(minimumBufferSize);
            
            var bytesRead = await stream.ReadAsync(memory);
            if (bytesRead == 0)
            {
                break;
            }
            // Tell the PipeWriter how much was read from the Socket
            writer.Advance(bytesRead);

            // Make the data available to the PipeReader
            var result = await writer.FlushAsync();

            if (result.IsCompleted)
            {
                break;
            }
        }

        // Tell the PipeReader that there's no more data coming
        await writer.CompleteAsync();
    }

    private static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySpan<byte> line)
    {
        var reader = new SequenceReader<byte>(buffer);

        if (reader.TryReadTo(out line, (byte)'\n'))
        {
            buffer = buffer.Slice(reader.Position);
            return true;
        }

        line = default;
        return false;
    }

    // public static async IAsyncEnumerable<T> DeserializeInZip<T>(string path)
    // {
    //     using var zipFile = new ZipArchive(File.OpenRead(path), ZipArchiveMode.Read, false);
    //
    //     await using var stream = zipFile.Entries[0].Open();
    //
    //     var pipe = new Pipe();
    //     var reader = pipe.Reader;
    //     
    //     var writing = FillPipeAsync(stream, pipe.Writer);
    //
    //     while (true)
    //     {
    //         var result = await reader.ReadAsync();
    //         var buffer = result.Buffer;
    //
    //         while (ConditionalDeserialize<T>(ReadLine(ref buffer), out var b) is var t && b)
    //         {
    //             yield return t!;
    //         }
    //         
    //         reader.AdvanceTo(buffer.Start, buffer.End);
    //         
    //         if (result.IsCompleted) break;
    //     }
    //
    //     await reader.CompleteAsync();
    //
    //     await writing;
    // }
    //
    // private static T? ConditionalDeserialize<T>(in ReadOnlySpan<byte> bytes, out bool b)
    // {
    //     if (bytes.Length > 0)
    //     {
    //         b = true;
    //         return JsonSerializer.Deserialize<T>(bytes, JsonSerializerOptions);
    //     }
    //     else
    //     {
    //         b = false;
    //         return default;
    //     }
    // }
}