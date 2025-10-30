using System.Buffers;

namespace Maxine.Extensions;

public class IoUtils
{
    public static byte[] ReadAllBytesToPool(string file, ArrayPool<byte>? pool = null)
    {
        pool ??= ArrayPool<byte>.Shared;
        
        using var fs = File.OpenRead(file);
        if (fs.Length > int.MaxValue)
        {
            throw new IOException("File is too large.");
        }

        var buffer = pool.Rent((int)fs.Length);
        try
        {
            fs.ReadExactly(buffer);
            return buffer;
        }
        catch
        {
            pool.Return(buffer);
            throw;
        }
    }
    
    public static async ValueTask<byte[]> ReadAllBytesToPoolAsync(string file, ArrayPool<byte>? pool = null, CancellationToken cancellationToken = default)
    {
        pool ??= ArrayPool<byte>.Shared;
        
        await using var fs = File.OpenRead(file);
        if (fs.Length > int.MaxValue)
        {
            throw new IOException("File is too large.");
        }

        var buffer = pool.Rent((int)fs.Length);
        try
        {
            await fs.ReadExactlyAsync(buffer, cancellationToken);
            return buffer;
        }
        catch
        {
            pool.Return(buffer);
            throw;
        }
    }
}