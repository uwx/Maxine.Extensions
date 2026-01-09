using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Maxine.Extensions;

namespace Maxine.VFS;

internal static class MemoryVfsHelpers
{
    public static IEnumerable<string> EnumerateFiles(
        IPath pathResolver,
        Dictionary<string, MemoryDirectory> directories,
        string path,
        ReaderWriterLockSlim? @lock = null
    )
    {
        using var _ = @lock?.GrabReadLock();

        path = pathResolver.GetFullPath(path);

        return directories.TryGetValue(path, out var v)
            ? v.Where(static e => !e.IsDirectory).Select(static x => x.FullName)
            : [];
    }

    public static IEnumerable<string> EnumerateDirectories(
        IPath pathResolver,
        Dictionary<string, MemoryDirectory> directories,
        string path,
        ReaderWriterLockSlim? @lock = null
    )
    {
        using var _ = @lock?.GrabReadLock();

        path = pathResolver.GetFullPath(path);

        return directories.TryGetValue(path, out var v)
            ? v.Where(static e => e.IsDirectory).Select(static x => x.FullName)
            : [];
    }

    public static IEnumerable<string> EnumerateChildren(
        IPath pathResolver,
        Dictionary<string, MemoryDirectory> directories,
        string path,
        string searchPattern,
        Func<BaseFsEntry, bool> predicate,
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        ReaderWriterLockSlim? @lock = null
    )
    {
        path = pathResolver.GetFullPath(path);

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
        using (@lock?.GrabReadLock())
        {
            if (!directories.TryGetValue(path, out v))
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

        IEnumerable<string> RecurseDirectory(ISet<BaseFsEntry> dir)
        {
            foreach (var x in dir)
            {
                if (predicate(x) && isMatchPattern(x.FullName))
                {
                    yield return x.FullName;
                }

                if (x is ISet<BaseFsEntry> subdir)
                {
                    foreach (var se in RecurseDirectory(subdir))
                    {
                        yield return se;
                    }
                }
            }
        }
    }
    
    public static bool HasParent(
        Dictionary<string, MemoryDirectory> directories,
        IPath pathResolver,
        string path,
        [NotNullWhen(true)] out MemoryDirectory? parent
    )
    {
        if (path == "")
        {
            parent = null;
            return false;
        }
            
        path = pathResolver.GetFullPath(path);

        if (pathResolver.GetDirectoryName(path) is { } parentName)
        {
            return directories.TryGetValue(parentName, out parent);
        }

        parent = directories[""];
        return true;
    }

    public static bool HasParent(Dictionary<string, MemoryDirectory> directories, IPath pathResolver, string path)
        => HasParent(directories, pathResolver, path, out _);

}