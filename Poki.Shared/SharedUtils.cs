using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;

namespace Poki.Shared;

[PublicAPI]
public static class SharedUtils
{
    private static string? _cachedSolutionDirectory;
    private static string? _cachedProjectDirectory;

    public static string? TryGetSolutionDirectory(string? currentPath = null)
    {
        if (_cachedSolutionDirectory != null)
        {
            return _cachedSolutionDirectory;
        }
        
        var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());
        while (directory != null && !directory.EnumerateFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        return _cachedSolutionDirectory = directory?.ToString();
    }
    
    public static string? TryGetProjectDirectory(string? currentPath = null)
    {
        if (_cachedProjectDirectory != null)
        {
            return _cachedProjectDirectory;
        }
        
        var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());
        while (directory != null && !directory.EnumerateFiles("*.csproj").Any())
        {
            directory = directory.Parent;
        }

        return _cachedProjectDirectory = directory?.ToString();
    }
    
    public static string GetDescription<T>(this T anEnum) where T : Enum
    {
        var fi = typeof(T).GetField(anEnum.ToString());

        if (fi == null)
        {
            throw new InvalidOperationException("enum FieldInfo is null?");
        }

        return fi.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute attr
            ? attr.Description
            : anEnum.ToString();
    }
}