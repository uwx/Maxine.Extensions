namespace Maxine.VFS;

public class HttpFileSystem : ReadOnlyFileSystem
{
    private readonly HttpClient _client;
    private readonly Func<string, Uri> _uriFromFilePathBuilder;

    public HttpFileSystem(HttpClient client, Func<string, Uri> uriFromFilePathBuilder)
    {
        _client = client;
        _uriFromFilePathBuilder = uriFromFilePathBuilder;
    }
    
    public HttpFileSystem(Uri baseUri)
    {
        _client = new HttpClient
        {
            BaseAddress = baseUri
        };
        _uriFromFilePathBuilder = path => new Uri(baseUri, path);
    }
    
    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern,
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return [];
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern,
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return [];
    }

    public override bool FileExists(string path)
    {
        var uri = _uriFromFilePathBuilder(path);
        using var request = new HttpRequestMessage(HttpMethod.Head, uri);
        using var response = _client.Send(request);
        return response.IsSuccessStatusCode;
    }

    public override bool DirectoryExists(string path)
    {
        return false;
    }

    public override Stream OpenRead(string path)
    {
        var uri = _uriFromFilePathBuilder(path);
        var response = _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
    }

    public override async ValueTask<Stream> OpenReadAsync(string path)
    {
        var uri = _uriFromFilePathBuilder(path);
        var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }

    public override FileAttributes GetAttributes(string file)
    {
        return FileAttributes.ReadOnly;
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        base.Dispose();
        _client.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await base.DisposeAsync();
        _client.Dispose();
    }
}