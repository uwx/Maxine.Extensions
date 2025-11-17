using System.Diagnostics.CodeAnalysis;

namespace Maxine.Extensions;

// https://stackoverflow.com/a/3378474
public sealed class TemporaryFile : IDisposable
{
    public TemporaryFile(string extension = ".txt") :
        this(Path.GetTempPath(), extension)
    {
    }

    public TemporaryFile(string directory, string extension = ".txt")
    {
        Create(Path.Combine(directory, Path.GetRandomFileName() + extension));
    }

    ~TemporaryFile()
    {
        Delete();
    }

    public void Dispose()
    {
        Delete();
        GC.SuppressFinalize(this);
    }

    public string FilePath { get; private set; }

    [MemberNotNull(nameof(FilePath))]
    private void Create(string path)
    {
        FilePath = path;
        using (File.Create(FilePath))
        {
        }
    }

    private void Delete()
    {
        if (FilePath == null!) return;
        File.Delete(FilePath);
        FilePath = null!;
    }
}