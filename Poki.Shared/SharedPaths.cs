namespace Poki.Shared;

// TODO document
public static class SharedPaths
{
    public static string ProjectRoot { get; } = SharedUtils.TryGetSolutionDirectory() is {} slnDir
        ? Path.Combine(slnDir!, "src", nameof(Poki))
        : Environment.CurrentDirectory;
}