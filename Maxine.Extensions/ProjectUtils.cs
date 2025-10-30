using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;

namespace Poki.Shared;

[PublicAPI]
public static class ProjectUtils
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
}