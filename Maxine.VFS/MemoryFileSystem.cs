using System.Buffers;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Maxine.Extensions;
using Maxine.Extensions.Streams;
using Microsoft.IO;

namespace Maxine.VFS;

public sealed class MemoryFileSystem : BaseFileSystem
{
    /*
    The MIT License (MIT)
    Copyright (c) 2013 Softwarebakery

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
     */

    public override IPath Path => IPath.MemoryPath.Instance;

    private readonly Dictionary<string, MemoryDirectory> _directories = new(StringComparer.OrdinalIgnoreCase)
    {
        [""] = new MemoryDirectory("")
    };

    private readonly Dictionary<string, MemoryFile> _files = new(StringComparer.OrdinalIgnoreCase);

    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);

    private bool HasParent(string path, [NotNullWhen(true)] out MemoryDirectory? parent)
    {
        if (path == "")
        {
            parent = null;
            return false;
        }
            
        path = Path.GetFullPath(path);

        if (Path.GetDirectoryName(path) is { } parentName)
        {
            return _directories.TryGetValue(parentName, out parent);
        }

        parent = _directories[""];
        return true;
    }

    private bool HasParent(string path) => HasParent(path, out _);

    public override ICollection<string> GetFiles(string path)
    {
        return EnumerateFiles(path).ToArray();
    }

    public override ICollection<string> GetDirectories(string path)
    {
        return EnumerateDirectories(path).ToArray();
    }

    public override IEnumerable<string> EnumerateFiles(string path)
    {
        using var _ = _lock.GrabReadLock();

        path = Path.GetFullPath(path);

        return _directories.TryGetValue(path, out var v)
            ? v.Where(static e => !e.IsDirectory).Select(static x => x.FullName)
            : [];
    }

    public override IEnumerable<string> EnumerateDirectories(string path)
    {
        using var _ = _lock.GrabReadLock();

        path = Path.GetFullPath(path);

        return _directories.TryGetValue(path, out var v)
            ? v.Where(static e => e.IsDirectory).Select(static x => x.FullName)
            : [];
    }

    public override ICollection<string> GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateChildren(path, searchPattern, static e => !e.IsDirectory, searchOption).ToArray();
    }

    public override ICollection<string> GetDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateChildren(path, searchPattern, static e => e.IsDirectory, searchOption).ToArray();
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateChildren(path, searchPattern, static e => !e.IsDirectory, searchOption);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateChildren(path, searchPattern, static e => e.IsDirectory, searchOption);
    }

    private IEnumerable<string> EnumerateChildren(string path, string searchPattern, Func<BaseFsEntry, bool> predicate, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        path = Path.GetFullPath(path);

        searchPattern = searchPattern.Replace('/', '\\');

        Func<string, bool> isMatchPattern;
        if (searchPattern is "*" or "*.*")
        {
            isMatchPattern = static _ => true;
        }
        else
        {
            var re2 = new Regex(
                Regex.Escape(searchPattern)
                    .Replace(@"\*", ".*")
                    .Replace(@"\?", "."),
                RegexOptions.IgnoreCase | RegexOptions.Compiled
            );

            isMatchPattern = path1 => re2.IsMatch(path1);
        }

        MemoryDirectory? v;
        using (_lock.GrabReadLock())
        {
            if (!_directories.TryGetValue(path, out v))
                throw new DirectoryNotFoundException();
        }

        if (searchOption == SearchOption.TopDirectoryOnly)
        {
            foreach (var e in v)
            {
                if (predicate(e) && isMatchPattern(e.FullName))
                {
                    yield return e.FullName;
                }
            }
        }
        else
        {
            foreach (var se in RecurseDirectory(v))
            {
                yield return se;
            }
        }

        yield break;

        IEnumerable<string> RecurseDirectory(MemoryDirectory dir)
        {
            foreach (var x in dir)
            {
                if (predicate(x) && isMatchPattern(x.FullName))
                {
                    yield return x.FullName;
                }

                if (x is MemoryDirectory subdir)
                {
                    foreach (var se in RecurseDirectory(subdir))
                    {
                        yield return se;
                    }
                }
            }
        }
    }

    public override bool FileExists(string path) => _files.ContainsKey(Path.GetFullPath(path));

    public override bool DirectoryExists(string path) => _directories.ContainsKey(Path.GetFullPath(path));

    public override Stream OpenFile(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite)
    {
        using var _ = _lock.GrabWriteLock();

        path = Path.GetFullPath(path);
        
        if (!HasParent(path, out var parent)) throw new DirectoryNotFoundException();
        if (_directories.ContainsKey(path)) throw new IOException("Directory already exists");
            
        if (mode is FileMode.CreateNew && _files.ContainsKey(path)) throw new IOException("File already exists");
        if (mode is FileMode.Open && !_files.ContainsKey(path)) throw new FileNotFoundException();
        
        if (mode is FileMode.Create && _files.TryGetValue(path, out var f))
        {
            f.Dispose(); 
        }

        if (mode is FileMode.Create or FileMode.OpenOrCreate or FileMode.CreateNew && !_files.ContainsKey(path))
        {
            var s = _files[path] = new MemoryFile(path);
            parent.Add(s);
                
            return s.Open();
        }

        if (_files.TryGetValue(path, out f))
        {
            var s = f.Open();
            if (mode is FileMode.Truncate)
            {
                s.SetLength(0);
            }
            return s;
        }

        throw new FileNotFoundException();
    }
    
    public override void CreateDirectory(string path)
    {
        using var _ = _lock.GrabWriteLock();
        
        path = Path.GetFullPath(path);

        if (_files.ContainsKey(path)) throw new IOException("File already exists");
        if (_directories.ContainsKey(path)) return;

        var parent = _directories[""];

        var idx = -1;
        char[] anyOf = ['\\', '/'];
        while ((idx = path.IndexOfAny(anyOf, idx + 1)) != -1)
        {
            var p = path[..idx];
            if (!_directories.TryGetValue(p, out var existing))
            {
                if (_files.ContainsKey(p)) throw new IOException("File already exists");

                var dir = new MemoryDirectory(p);
                parent.Add(dir);
                _directories.Add(p, dir);

                parent = dir;
            }
            else
            {
                parent = existing;
            }
        }

        if (!_directories.ContainsKey(path))
        {
            var dir = new MemoryDirectory(path);
            parent.Add(dir);
            _directories.TryAdd(path, dir);
        }
    }

    public override void DeleteFile(string path)
    {
        using var _ = _lock.GrabWriteLock();

        path = Path.GetFullPath(path);

        if (!HasParent(path, out var parent))
        {
            throw new DirectoryNotFoundException();
        }

        if (_files.Remove(path, out var value))
        {
            parent.Remove(value);
            value.Dispose();
        }
    }

    public override void DeleteDirectory(string path, bool recursive = false)
    {
        using var _ = _lock.GrabWriteLock();

        path = Path.GetFullPath(path);

        if (path == "")
        {
            throw new IOException("Cannot delete the root directory");
        }

        if (!HasParent(path, out var parent))
        {
            throw new DirectoryNotFoundException();
        }

        if (recursive)
        {
            foreach (var file in GetFiles(path, "*", SearchOption.AllDirectories))
            {
                DeleteFile(file);
            }

            foreach (var directory in GetDirectories(path, "*", SearchOption.AllDirectories))
            {
                _directories.Remove(directory);
            }
        }
        else
        {
            if (_directories.TryGetValue(path, out var v) && v.Count > 0)
            {
                throw new IOException("Directory wasn't empty");
            }
        }

        if (_directories.Remove(Path.GetFullPath(path), out var value))
        {
            parent.Remove(value);
        }
    }

    public override void CopyFile(string from, string to, bool overwrite = false)
    {
        using var _ = _lock.GrabWriteLock();

        from = Path.GetFullPath(from);
        to = Path.GetFullPath(to);
        
        if (!HasParent(to, out var destParent)) throw new DirectoryNotFoundException();
        if (from == to) throw new IOException("Source and destination are the same");
        if (DirectoryExists(to)) throw new IOException();
        if (FileExists(to) && !overwrite) throw new IOException();

        if (_files.TryGetValue(from, out var f))
        {
            DeleteFile(to);

            var clone = f.Clone(to);
                
            destParent.Add(clone);
            _files[to] = clone;
        }
        else
        {
            throw new FileNotFoundException();
        }
    }

    public override void MoveFile(string from, string to, bool overwrite = false)
    {
        CopyFile(from, to, overwrite);
        DeleteFile(from);
    }

    public override FileAttributes GetAttributes(string file)
    {
        using var _ = _lock.GrabReadLock();

        file = Path.GetFullPath(file);
            
        if (_directories.TryGetValue(file, out var d))
        {
            return d.FileAttributes;
        }
        
        if (_files.TryGetValue(file, out var f))
        {
            return f.FileAttributes;
        }

        throw new FileNotFoundException();
    }

    public override void SetAttributes(string file, FileAttributes attributes)
    {
        using var _ = _lock.GrabReadLock();

        file = Path.GetFullPath(file);

        if (_directories.TryGetValue(file, out var d))
        {
            d.FileAttributes = attributes;
            return;
        }
        
        if (_files.TryGetValue(file, out var f))
        {
            f.FileAttributes = attributes;
            return;
        }
        
        throw new FileNotFoundException();
    }

    public ReadOnlySequence<byte> GetFileSequence(string path)
    {
        using var _ = _lock.GrabReadLock();

        path = Path.GetFullPath(path);
        if (_files.TryGetValue(path, out var f))
        {
            f._content.Position = 0;
            return f._content.GetReadOnlySequence();
        }

        throw new FileNotFoundException();
    }

    public void PutFile(string path, ReadOnlySpan<byte> data)
    {
        using var _ = _lock.GrabWriteLock();

        path = Path.GetFullPath(path);
        if (!HasParent(path, out var parent)) throw new DirectoryNotFoundException();

        if (!_files.TryGetValue(path, out var f))
        {
            _files[path] = f = new MemoryFile(path);
            parent.Add(f);
        }

        f._content.SetLength(0);
        f._content.Write(data);
    }

    public void PutFile(string path, ReadOnlySequence<byte> data)
    {
        using var _ = _lock.GrabWriteLock();

        path = Path.GetFullPath(path);
        if (!HasParent(path, out var parent)) throw new DirectoryNotFoundException();

        if (!_files.TryGetValue(path, out var f))
        {
            _files[path] = f = new MemoryFile(path);
            parent.Add(f);
        }

        f._content.SetLength(0);
        if (data.Length <= int.MaxValue)
        {
            data.CopyTo(f._content.GetSpan((int)data.Length));
        }
        else
        {
            if (f._content.Capacity64 < data.Length)
                f._content.Capacity64 = data.Length;

            foreach (var memory in data)
            {
                f._content.Write(memory.Span);
            }
        }
    }

    public override void Dispose()
    {
        using var _ = _lock.GrabWriteLock();

        foreach (var (_, value) in _files)
        {
            value.Dispose();
        }
        _directories.Clear();
        _files.Clear();
    }

    public override async ValueTask DisposeAsync()
    {
        using var _ = _lock.GrabWriteLock();

        foreach (var (_, value) in _files)
        {
            await value.DisposeAsync();
        }
        _directories.Clear();
        _files.Clear();
    }
}

internal abstract class BaseFsEntry(string fullName) : IDisposable, IAsyncDisposable
{
    public FileAttributes FileAttributes { get; set; } = FileAttributes.Normal;
    public abstract bool IsDirectory { get; }
    public string FullName { get; } = fullName;

    public virtual void Dispose()
    {
    }

    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}

internal sealed class MemoryDirectory(string fullName) : BaseFsEntry(fullName), ISet<BaseFsEntry>
{
    private readonly HashSet<BaseFsEntry> _setImplementation = [];
    public override bool IsDirectory => true;

    #region ISet
    public IEnumerator<BaseFsEntry> GetEnumerator()
    {
        return _setImplementation.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_setImplementation).GetEnumerator();
    }

    void ICollection<BaseFsEntry>.Add(BaseFsEntry item)
    {
        _setImplementation.Add(item);
    }

    public void ExceptWith(IEnumerable<BaseFsEntry> other)
    {
        _setImplementation.ExceptWith(other);
    }

    public void IntersectWith(IEnumerable<BaseFsEntry> other)
    {
        _setImplementation.IntersectWith(other);
    }

    public bool IsProperSubsetOf(IEnumerable<BaseFsEntry> other)
    {
        return _setImplementation.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<BaseFsEntry> other)
    {
        return _setImplementation.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<BaseFsEntry> other)
    {
        return _setImplementation.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<BaseFsEntry> other)
    {
        return _setImplementation.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<BaseFsEntry> other)
    {
        return _setImplementation.Overlaps(other);
    }

    public bool SetEquals(IEnumerable<BaseFsEntry> other)
    {
        return _setImplementation.SetEquals(other);
    }

    public void SymmetricExceptWith(IEnumerable<BaseFsEntry> other)
    {
        _setImplementation.SymmetricExceptWith(other);
    }

    public void UnionWith(IEnumerable<BaseFsEntry> other)
    {
        _setImplementation.UnionWith(other);
    }

    public bool Add(BaseFsEntry item)
    {
        return _setImplementation.Add(item);
    }

    public void Clear()
    {
        _setImplementation.Clear();
    }

    public bool Contains(BaseFsEntry item)
    {
        return _setImplementation.Contains(item);
    }

    public void CopyTo(BaseFsEntry[] array, int arrayIndex)
    {
        _setImplementation.CopyTo(array, arrayIndex);
    }

    public bool Remove(BaseFsEntry item)
    {
        return _setImplementation.Remove(item);
    }

    public int Count => _setImplementation.Count;

    public bool IsReadOnly => false;
    #endregion
}

internal sealed class MemoryFile : BaseFsEntry
{
    internal static readonly RecyclableMemoryStreamManager Manager = new();
        
    public override bool IsDirectory => false;

    private readonly Guid _guid = Guid.NewGuid();
    internal readonly RecyclableMemoryStream _content;

    public MemoryFile(ReadOnlySpan<byte> content, string fullName) : base(fullName)
    {
        _content = new RecyclableMemoryStream(Manager, _guid, fullName, content.Length);
        _content.Write(content);
    }

    private MemoryFile(MemoryFile existing, string fullName) : base(fullName)
    {
        _content = new RecyclableMemoryStream(Manager, _guid, fullName, existing._content.Length);

        foreach (var memory in existing._content.GetReadOnlySequence())
        {
            _content.Write(memory.Span);
        }
    }

    public MemoryFile(string fullName) : base(fullName)
    {
        _content = new RecyclableMemoryStream(Manager, _guid, fullName);
    }

    ~MemoryFile()
    {
        _content.Dispose();
    }

    public Stream Open()
    {
        _content.Position = 0;
        return new NoDisposeStream(_content);
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        _content.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _content.DisposeAsync();
    }

    public MemoryFile Clone(string newFileName)
    {
        return new MemoryFile(this, newFileName);
    }
}