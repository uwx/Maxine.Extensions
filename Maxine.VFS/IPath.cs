namespace Maxine.VFS;

public interface IPath
{
    internal class IoPath : IPath
    {
        public static IoPath Instance { get; } = new();
        
        public char DirectorySeparatorChar => Path.AltDirectorySeparatorChar;

        public string? GetDirectoryName(string path) => Path.GetDirectoryName(path);

        public string Combine(params string[] paths) => Path.Combine(paths);
        public string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);
        public string GetFileName(string path) => Path.GetFileName(path);
        public string GetExtension(string path) => Path.GetExtension(path);

        public string ChangeExtension(string path, string newExtension) => Path.ChangeExtension(path, newExtension);

        public string GetFullPath(string path) => Path.GetFullPath(path);
    }

    public class MemoryPath : IPath
    {
        private const char DirectorySeparatorCharConst = '\\';

        public static MemoryPath Instance { get; } = new();

        public char DirectorySeparatorChar => DirectorySeparatorCharConst;

        public string? GetDirectoryName(string path)
        {
            var span = path.AsSpan();

            return span.LastIndexOfAny(DirectorySeparatorCharConst, '/') is var idx && idx != -1
                ? new string(span[..idx])
                : null;
        }

        public string Combine(params string[] paths)
            => string.Join(DirectorySeparatorCharConst, paths);

        public string GetFileNameWithoutExtension(string path)
        {
            var span = path.AsSpan();

            var lastIdx = span.LastIndexOfAny(DirectorySeparatorCharConst, '/');

            var dot = span[(lastIdx + 1)..].LastIndexOf('.');
            return dot != -1
                ? new string(span[(lastIdx + 1)..(lastIdx + 1 + dot)])
                : new string(span[(lastIdx + 1)..]);
        }

        public string GetFileName(string path)
        {
            var span = path.AsSpan();

            var lastIdx = span.LastIndexOfAny(DirectorySeparatorCharConst, '/');

            if (lastIdx != -1)
            {
                return new string(span[(lastIdx + 1)..]);
            }

            return path;
        }

        public string GetExtension(string path)
        {
            return path.LastIndexOf('.') is var idx && idx != -1
                ? path[idx..]
                : "";
        }

        public string ChangeExtension(string path, string newExtension)
        {
            return path.LastIndexOf('.') is var idx && idx != -1
                ? path[..idx] + newExtension
                : path + newExtension;
        }

        public string GetFullPath(string path)
        {
            path = path.Replace('/', '\\').TrimEnd('\\');

            if (path.StartsWith(@".\"))
            {
                path = path[2..];
            }

            return path;
        }
    }

    char DirectorySeparatorChar { get; }
    bool IsCaseSensitive => false;

    string? GetDirectoryName(string path);
    string Combine(string path, string path2) => Combine([path, path2]);
    string Combine(string path, string path2, string path3) => Combine([path, path2, path3]);
    string Combine(string path, string path2, string path3, string path4) => Combine([path, path2, path3, path4]);
    string Combine(params string[] paths);
    
    string GetFileNameWithoutExtension(string path);
    string GetFileName(string path);
    string GetExtension(string path);
    string ChangeExtension(string path, string newExtension);

    string GetFullPath(string path);
    
    bool PathEquals(string path1, string path2)
    {
        return string.Equals(
            GetFullPath(path1).TrimEnd(DirectorySeparatorChar),
            GetFullPath(path2).TrimEnd(DirectorySeparatorChar),
            IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase
        );
    }
}