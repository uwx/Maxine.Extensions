using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Maxine.Fetch;

public class Fetch
{
    private static readonly Lazy<HttpClient> HttpClient = new(() => new HttpClient());
    
    public static Task<Response> FetchAsync(Uri uri, RequestNoUri request, CancellationToken cancellationToken = default) => FetchAsync(new Request(request, uri), cancellationToken);
    public static Task<Response> FetchAsync(string uri, RequestNoUri request, CancellationToken cancellationToken = default) => FetchAsync(new Request(request, new Uri(uri)), cancellationToken);

    public static Task<Response> FetchAsync(Uri uri, CancellationToken cancellationToken = default) => FetchAsync(new Request
    {
        Method = HttpMethod.Get,
        RequestUri = uri
    }, cancellationToken);
    public static Task<Response> FetchAsync(string uri, CancellationToken cancellationToken = default) => FetchAsync(new Request
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri(uri)
    }, cancellationToken);

    public static async Task<Response> FetchAsync(Request request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.Value.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        return new Response(response);
    }

    private static async Task test()
    {
        var response = await FetchAsync(new Request
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://example.com"),
            Headers =
            {
                {"Content-Type", "application/json"},
            }
        });
        var text = response.Text();
    }
}

public class RequestNoUri() : HttpRequestMessage(HttpMethod.Get, (Uri?)null)
{
    public new required HttpMethod Method
    {
        set => base.Method = value;
    }
    
    public new RequestBody? Body
    {
        set
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (value != null)
                Content = value;
            else
                Content = null;
        }
    }
}

public class Request() : RequestNoUri
{
    public new required Uri RequestUri
    {
        set => base.RequestUri = value;
    }

    [SetsRequiredMembers]
    internal Request(RequestNoUri request, Uri requestUri) : this()
    {
        RequestUri = requestUri;
        Method = ((HttpRequestMessage)request).Method;
        Content = request.Content;
        foreach (var (name, values) in request.Headers)
        {
            if (!Headers.TryAddWithoutValidation(name, values))
            {
                throw new ArgumentException("Headers are invalid", nameof(request));
            }
        }
    }
}

public class RequestBody(HttpContent content)
{
    protected HttpContent Content { get; } = content;

    public static RequestBody Json<T>(T inputValue, MediaTypeHeaderValue? mediaType = null, JsonSerializerOptions? options = null) =>
        new(JsonContent.Create(inputValue, mediaType, options));
    
    public static RequestBody Json(object? inputValue, Type inputType, MediaTypeHeaderValue? mediaType = null, JsonSerializerOptions? options = null) =>
        new(JsonContent.Create(inputValue, inputType, mediaType, options));

    public static RequestBody Json<T>(T? inputValue, JsonTypeInfo<T> jsonTypeInfo, MediaTypeHeaderValue? mediaType = null) =>
        new(JsonContent.Create(inputValue, jsonTypeInfo, mediaType));
    
    public static RequestBody Json(object? inputValue, JsonTypeInfo jsonTypeInfo, MediaTypeHeaderValue? mediaType = null) =>
        new(JsonContent.Create(inputValue, jsonTypeInfo, mediaType));

    public static implicit operator RequestBody(string value) => new(new StringContent(value));
    public static implicit operator RequestBody(byte[] value) => new(new ByteArrayContent(value));
    public static implicit operator RequestBody(ReadOnlyMemory<byte> value) => new(new ReadOnlyMemoryContent(value));
    public static implicit operator RequestBody(Stream value) => new(new StreamContent(value));

    public static implicit operator HttpContent(RequestBody value) => value.Content;
}

public class FormData() : RequestBody(new MultipartFormDataContent())
{
    public FormData Append(string name, string value, string? fileName = null)
    {
        if (fileName == null)
        {
            ((MultipartFormDataContent)Content).Add(new StringContent(value), name);
        }
        else
        {
            ((MultipartFormDataContent)Content).Add(new StringContent(value), name, fileName);
        }

        return this;
    }
    
    public FormData Append(string name, byte[] value, string? fileName = null)
    {
        if (fileName == null)
        {
            ((MultipartFormDataContent)Content).Add(new ByteArrayContent(value), name);
        }
        else
        {
            ((MultipartFormDataContent)Content).Add(new ByteArrayContent(value), name, fileName);
        }

        return this;
    }
    
    public FormData Append(string name, ReadOnlyMemory<byte> value, string? fileName = null)
    {
        if (fileName == null)
        {
            ((MultipartFormDataContent)Content).Add(new ReadOnlyMemoryContent(value), name);
        }
        else
        {
            ((MultipartFormDataContent)Content).Add(new ReadOnlyMemoryContent(value), name, fileName);
        }

        return this;
    }
    
    public FormData Append(string name, Stream value, string? fileName = null)
    {
        if (fileName == null)
        {
            ((MultipartFormDataContent)Content).Add(new StreamContent(value), name);
        }
        else
        {
            ((MultipartFormDataContent)Content).Add(new StreamContent(value), name, fileName);
        }

        return this;
    }
}

public class Response(HttpResponseMessage response) : IDisposable
{
    public bool Ok => response.IsSuccessStatusCode;
    public bool Redirected => (int)response.StatusCode >= 300 && (int)response.StatusCode < 400;
    public HttpStatusCode Status => response.StatusCode;
    public string StatusText => response.ReasonPhrase ?? string.Empty;
    public Uri? Url => response.RequestMessage?.RequestUri;
    public HttpResponseHeaders Headers => response.Headers;
    
    public bool BodyUsed { get; private set; }

    public void EnsureSuccessStatusCode()
    {
        response.EnsureSuccessStatusCode();
    }
 
    public async Task<Stream> Body()
    {
        using (UseBody())
            return await response.Content.ReadAsStreamAsync();
    }

    public async Task<byte[]> Bytes()
    {
        using (UseBody())
            return await response.Content.ReadAsByteArrayAsync();
    }

    [RequiresUnreferencedCode("The type T may require types that cannot be statically analyzed.")]
    public async Task<T?> Json<T>(JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        using (UseBody())
            return await response.Content.ReadFromJsonAsync<T>(options, cancellationToken);
    }

    public async Task<T?> Json<T>(JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken = default)
    {
        using (UseBody())
            return await response.Content.ReadFromJsonAsync(typeInfo, cancellationToken);
    }
    
    public async Task<string> Text()
    {
        using (UseBody())
            return await response.Content.ReadAsStringAsync();
    }

    private BodyDisposable UseBody()
    {
        if (BodyUsed)
        {
            throw new InvalidOperationException("Response body is already in use");
        }

        BodyUsed = true;

        return new BodyDisposable(this);
    }

    private readonly struct BodyDisposable(Response response) : IDisposable
    {
        public void Dispose() => response.Dispose();
    }

    public void Dispose()
    {
        response.Dispose();
    }
}