namespace Maxine.Extensions.Streams;

public sealed class NoDisposeStream(Stream streamImplementation) : Stream
{
    public override void Flush()
    {
        streamImplementation.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return streamImplementation.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return streamImplementation.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        streamImplementation.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        streamImplementation.Write(buffer, offset, count);
    }

    public override bool CanRead => streamImplementation.CanRead;

    public override bool CanSeek => streamImplementation.CanSeek;

    public override bool CanWrite => streamImplementation.CanWrite;

    public override long Length => streamImplementation.Length;

    public override long Position
    {
        get => streamImplementation.Position;
        set => streamImplementation.Position = value;
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return streamImplementation.BeginRead(buffer, offset, count, callback, state);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return streamImplementation.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void Close()
    {
    }

    public override void CopyTo(Stream destination, int bufferSize)
    {
        streamImplementation.CopyTo(destination, bufferSize);
    }

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        return streamImplementation.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        return streamImplementation.EndRead(asyncResult);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        streamImplementation.EndWrite(asyncResult);
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return streamImplementation.FlushAsync(cancellationToken);
    }

    public override int Read(Span<byte> buffer)
    {
        return streamImplementation.Read(buffer);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return streamImplementation.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
    {
        return streamImplementation.ReadAsync(buffer, cancellationToken);
    }

    public override int ReadByte()
    {
        return streamImplementation.ReadByte();
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        streamImplementation.Write(buffer);
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return streamImplementation.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
    {
        return streamImplementation.WriteAsync(buffer, cancellationToken);
    }

    public override void WriteByte(byte value)
    {
        streamImplementation.WriteByte(value);
    }

    public override bool CanTimeout => streamImplementation.CanTimeout;

    public override int ReadTimeout
    {
        get => streamImplementation.ReadTimeout;
        set => streamImplementation.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
        get => streamImplementation.WriteTimeout;
        set => streamImplementation.WriteTimeout = value;
    }

    [Obsolete("This Remoting API is not supported and throws PlatformNotSupportedException.", DiagnosticId = "SYSLIB0010", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public override object InitializeLifetimeService()
    {
        return streamImplementation.InitializeLifetimeService();
    }

    public override bool Equals(object? obj)
    {
        return streamImplementation.Equals(obj);
    }

    public override int GetHashCode()
    {
        return streamImplementation.GetHashCode();
    }

    public override string? ToString()
    {
        return streamImplementation.ToString();
    }

    protected override void Dispose(bool disposing)
    {
    }

    public override ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}