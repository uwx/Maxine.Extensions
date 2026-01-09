using System.Runtime.CompilerServices;
using System.Text;

namespace Maxine.VFS;

public abstract class ReadOnlyFileSystem : IDisposable, IAsyncDisposable
{
    public virtual IPath Path => IPath.IoPath.Instance;

    public virtual ICollection<string> GetFiles(string path) => GetFiles(path, "*");
    public virtual ICollection<string> GetDirectories(string path) => GetDirectories(path, "*");
    public virtual ICollection<string> GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) => EnumerateFiles(path).ToArray();
    public virtual ICollection<string> GetDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) => EnumerateDirectories(path).ToArray();
    public virtual IEnumerable<string> EnumerateFiles(string path) => EnumerateFiles(path, "*");
    public virtual IEnumerable<string> EnumerateDirectories(string path) => EnumerateDirectories(path, "*");
    public abstract IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);
    public abstract IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);

    public virtual bool Exists(string path) => FileExists(path) || DirectoryExists(path);
    public abstract bool FileExists(string path);
    public abstract bool DirectoryExists(string path);
        
    public abstract Stream OpenRead(string path);
        
    public virtual byte[] ReadAllBytes(string path)
    {
        using var stream = OpenRead(path);

        if (stream.CanSeek)
        {
            var bytes = new byte[stream.Length - stream.Position];
            stream.ReadExactly(bytes);
            return bytes;
        }

        var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    public virtual async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = OpenRead(path);

        if (stream.CanSeek)
        {
            var bytes = new byte[stream.Length - stream.Position];
            await stream.ReadExactlyAsync(bytes, cancellationToken);
            return bytes;
        }

        var ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        return ms.ToArray();
    }

    public virtual string ReadAllText(string path, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
            
        return encoding.GetString(ReadAllBytes(path));
    }

    public virtual async Task<string> ReadAllTextAsync(string path, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        encoding ??= Encoding.UTF8;
            
        return encoding.GetString(await ReadAllBytesAsync(path, cancellationToken));
    }

    public virtual IEnumerable<string> ReadAllLines(string path, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        using var sr = new StreamReader(OpenRead(path), encoding);
        while (sr.ReadLine() is { } line)
        {
            yield return line;
        }
    }

    public virtual async IAsyncEnumerable<string> ReadAllLinesAsync(string path, Encoding? encoding = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        encoding ??= Encoding.UTF8;

        using var sr = new StreamReader(OpenRead(path), encoding);
        while (await sr.ReadLineAsync(cancellationToken) is { } line)
        {
            yield return line;
        }
    }
    public virtual Dictionary<string, byte[]> ReadAllFilesToDictionary(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var result = new Dictionary<string, byte[]>();

        foreach (var file in EnumerateFiles(path, searchPattern, searchOption))
        {
            result[file] = ReadAllBytes(file);
        }

        return result;
    }
    
    public virtual async Task<Dictionary<string, byte[]>> ReadAllFilesToDictionaryAsync(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, byte[]>();

        await foreach (var file in EnumerateFiles(path, searchPattern, searchOption).ToAsyncEnumerable().WithCancellation(cancellationToken))
        {
            result[file] = await ReadAllBytesAsync(file, cancellationToken);
        }

        return result;
    }
        
    public abstract FileAttributes GetAttributes(string file);
        
    public virtual void Dispose()
    {
    }

    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
    
public abstract class BaseFileSystem : ReadOnlyFileSystem, IDisposable, IAsyncDisposable
{
    protected static Encoding Utf8NoBom { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    public virtual IPath Path => IPath.IoPath.Instance;

    public virtual Stream CreateFile(string path) => OpenFile(path, FileMode.Create, FileAccess.ReadWrite);
    public abstract Stream OpenFile(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite);
    public virtual Stream OpenWrite(string path) => OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write);
    public override Stream OpenRead(string path) => OpenFile(path, FileMode.Open, FileAccess.Read);

    public abstract void CreateDirectory(string path);
    public abstract void DeleteFile(string path);
    public abstract void DeleteDirectory(string path, bool recursive = false);
    public abstract void CopyFile(string from, string to, bool overwrite = false);
    public abstract void MoveFile(string from, string to, bool overwrite = false);

    public virtual void WriteAllBytes(string path, ReadOnlySpan<byte> bytes)
    {
        using var stream = CreateFile(path);
        stream.Write(bytes);
    }

    public virtual async Task WriteAllBytesAsync(string path, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
    {
        await using var stream = CreateFile(path);
        await stream.WriteAsync(bytes, cancellationToken);
    }

    public virtual void WriteAllText(string path, ReadOnlySpan<char> text, Encoding? encoding = null)
    {
        encoding ??= Utf8NoBom;
            
        using var stream = new StreamWriter(CreateFile(path), encoding);
        stream.Write(text);
    }

    public virtual async Task WriteAllTextAsync(string path, string text, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        encoding ??= Utf8NoBom;
            
        await using var stream = new StreamWriter(CreateFile(path), encoding);
        await stream.WriteAsync(text.AsMemory(), cancellationToken);
    }

    public virtual void WriteAllLines(string path, ReadOnlySpan<string> text, Encoding? encoding = null)
    {
        encoding ??= Utf8NoBom;
            
        using var stream = new StreamWriter(CreateFile(path), encoding);
        for (var i = 0; i < text.Length; i++)
        {
            var line = text[i];

            if (i < text.Length - 1) stream.WriteLine(line);
            else stream.Write(line);
        }
    }

    public virtual async Task WriteAllLinesAsync(string path, ReadOnlyMemory<string> text, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        encoding ??= Utf8NoBom;
            
        await using var stream = new StreamWriter(CreateFile(path), encoding);
        for (var i = 0; i < text.Length; i++)
        {
            var line = text.Span[i];

            if (i < text.Length - 1) await stream.WriteLineAsync(line.AsMemory(), cancellationToken);
            else await stream.WriteAsync(line.AsMemory(), cancellationToken);
        }
    }

    public abstract void SetAttributes(string file, FileAttributes attributes);

    public virtual TextReader OpenText(string file, Encoding? encoding = null)
    {
        return new StreamReader(OpenFile(file, access: FileAccess.Read), encoding ?? Encoding.UTF8);
    }

    public virtual TextWriter CreateText(string file, Encoding? encoding = null)
    {
        return new StreamWriter(OpenFile(file, access: FileAccess.Write), Utf8NoBom);
    }
    
    public virtual void WriteAllFilesFromDictionary(IReadOnlyDictionary<string, byte[]> files)
    {
        foreach (var (path, data) in files)
        {
            if (Path.GetDirectoryName(path) is { } dir && !DirectoryExists(dir))
            {
                CreateDirectory(dir);
            }
            using var stream = OpenFile(path, FileMode.Create, FileAccess.Write);
            stream.Write(data, 0, data.Length);
        }
    }
    
    public virtual async Task WriteAllFilesFromDictionaryAsync(IReadOnlyDictionary<string, byte[]> files, CancellationToken cancellationToken = default)
    {
        foreach (var (path, data) in files)
        {
            if (Path.GetDirectoryName(path) is { } dir && !DirectoryExists(dir))
            {
                CreateDirectory(dir);
            }
            await using var stream = OpenFile(path, FileMode.Create, FileAccess.Write);
            await stream.WriteAsync(data, cancellationToken);
        }
    }
}