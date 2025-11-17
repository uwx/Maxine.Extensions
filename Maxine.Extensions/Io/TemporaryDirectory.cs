using System.Diagnostics.CodeAnalysis;

namespace Maxine.Extensions;

// https://stackoverflow.com/a/3378474
public class TemporaryDirectory : IDisposable
{
    public TemporaryDirectory() :
        this(Path.GetTempPath())
    {
    }

    public TemporaryDirectory(string directory)
    {
        Create(Path.Combine(directory, Path.GetRandomFileName()));
    }

    ~TemporaryDirectory()
    {
        Delete();
    }

    public void Dispose()
    {
        Delete();
        GC.SuppressFinalize(this);
    }

    public string DirectoryPath { get; private set; }

    [MemberNotNull(nameof(DirectoryPath))]
    private void Create(string path)
    {
        DirectoryPath = path;
        Directory.CreateDirectory(DirectoryPath);
    }

    private void Delete()
    {
        if (DirectoryPath == null!) return;
        Directory.Delete(DirectoryPath, false);
        DirectoryPath = null!;
    }
}