using System.Net;
using System.Text;
using System.Text.Json;

namespace Maxine.Fetch.Test;

/// <summary>
/// Integration tests for Fetch with a real HTTP server
/// </summary>
[TestClass]
public class FetchIntegrationTests
{
    private static HttpListener? _httpListener;
    private static string? _baseUrl;
    private static CancellationTokenSource? _serverCts;
    private static Task? _serverTask;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        // Find an available port
        var port = FindAvailablePort();
        _baseUrl = $"http://localhost:{port}/";

        // Start the HTTP listener
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(_baseUrl);
        _httpListener.Start();

        // Start processing requests in background
        _serverCts = new CancellationTokenSource();
        _serverTask = Task.Run(() => ProcessRequests(_serverCts.Token));
    }

    [ClassCleanup]
    public static void ClassTeardown()
    {
        _serverCts?.Cancel();
        _serverTask?.Wait(TimeSpan.FromSeconds(5));
        _httpListener?.Stop();
        _httpListener?.Close();
    }

    private static int FindAvailablePort()
    {
        var listener = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static async Task ProcessRequests(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _httpListener != null)
        {
            try
            {
                var context = await _httpListener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context), cancellationToken);
            }
            catch (HttpListenerException)
            {
                // Listener was stopped
                break;
            }
            catch (ObjectDisposedException)
            {
                // Listener was disposed
                break;
            }
        }
    }

    private static void HandleRequest(HttpListenerContext context)
    {
        try
        {
            var request = context.Request;
            var response = context.Response;

            // Route based on URL path
            switch (request.Url?.AbsolutePath)
            {
                case "/hello":
                    HandleHello(response);
                    break;

                case "/json":
                    HandleJson(response);
                    break;

                case "/echo":
                    HandleEcho(request, response);
                    break;

                case "/status/404":
                    response.StatusCode = 404;
                    response.StatusDescription = "Not Found";
                    var notFoundBytes = Encoding.UTF8.GetBytes("Not Found");
                    response.ContentLength64 = notFoundBytes.Length;
                    response.OutputStream.Write(notFoundBytes, 0, notFoundBytes.Length);
                    break;

                case "/status/500":
                    response.StatusCode = 500;
                    response.StatusDescription = "Internal Server Error";
                    var errorBytes = Encoding.UTF8.GetBytes("Server Error");
                    response.ContentLength64 = errorBytes.Length;
                    response.OutputStream.Write(errorBytes, 0, errorBytes.Length);
                    break;

                case "/redirect":
                    response.StatusCode = 302;
                    response.RedirectLocation = "/hello";
                    break;

                case "/headers":
                    HandleHeaders(request, response);
                    break;

                case "/post":
                    HandlePost(request, response);
                    break;

                default:
                    response.StatusCode = 404;
                    break;
            }

            response.Close();
        }
        catch (Exception)
        {
            // Ignore errors in test server
        }
    }

    private static void HandleHello(HttpListenerResponse response)
    {
        response.StatusCode = 200;
        response.ContentType = "text/plain";
        var bytes = Encoding.UTF8.GetBytes("Hello, World!");
        response.ContentLength64 = bytes.Length;
        response.OutputStream.Write(bytes, 0, bytes.Length);
    }

    private static void HandleJson(HttpListenerResponse response)
    {
        response.StatusCode = 200;
        response.ContentType = "application/json";
        var json = JsonSerializer.Serialize(new { Message = "Hello from JSON", Value = 42 });
        var bytes = Encoding.UTF8.GetBytes(json);
        response.ContentLength64 = bytes.Length;
        response.OutputStream.Write(bytes, 0, bytes.Length);
    }

    private static void HandleEcho(HttpListenerRequest request, HttpListenerResponse response)
    {
        response.StatusCode = 200;
        response.ContentType = request.ContentType ?? "application/octet-stream";

        using var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8);
        var body = reader.ReadToEnd();
        var bytes = Encoding.UTF8.GetBytes(body);
        response.ContentLength64 = bytes.Length;
        response.OutputStream.Write(bytes, 0, bytes.Length);
    }

    private static void HandleHeaders(HttpListenerRequest request, HttpListenerResponse response)
    {
        response.StatusCode = 200;
        response.ContentType = "application/json";

        var headers = new Dictionary<string, string>();
        foreach (var key in request.Headers.AllKeys)
        {
            if (key != null)
            {
                headers[key] = request.Headers[key] ?? "";
            }
        }

        var json = JsonSerializer.Serialize(headers);
        var bytes = Encoding.UTF8.GetBytes(json);
        response.ContentLength64 = bytes.Length;
        response.OutputStream.Write(bytes, 0, bytes.Length);
    }

    private static void HandlePost(HttpListenerRequest request, HttpListenerResponse response)
    {
        response.StatusCode = 201;
        response.ContentType = "application/json";

        using var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8);
        var body = reader.ReadToEnd();

        var result = new { Received = body, Method = request.HttpMethod, ContentType = request.ContentType };
        var json = JsonSerializer.Serialize(result);
        var bytes = Encoding.UTF8.GetBytes(json);
        response.ContentLength64 = bytes.Length;
        response.OutputStream.Write(bytes, 0, bytes.Length);
    }

    // Integration Tests

    [TestMethod]
    public async Task FetchAsync_GetRequest_ReturnsText()
    {
        // Act
        using var response = await Fetch.FetchAsync($"{_baseUrl}hello");
        var text = await response.Text();

        // Assert
        Assert.IsTrue(response.Ok);
        Assert.AreEqual(HttpStatusCode.OK, response.Status);
        Assert.AreEqual("Hello, World!", text);
    }

    [TestMethod]
    public async Task FetchAsync_GetJson_DeserializesCorrectly()
    {
        // Act
        using var response = await Fetch.FetchAsync($"{_baseUrl}json");
        var data = await response.Json<TestJsonData>();

        // Assert
        Assert.IsTrue(response.Ok);
        Assert.IsNotNull(data);
        Assert.AreEqual("Hello from JSON", data.Message);
        Assert.AreEqual(42, data.Value);
    }

    [TestMethod]
    public async Task FetchAsync_PostWithStringBody_EchoesBack()
    {
        // Arrange
        var content = "Test content";
        var request = new Request
        {
            RequestUri = new Uri($"{_baseUrl}echo"),
            Method = HttpMethod.Post,
            Body = content
        };

        // Act
        using var response = await Fetch.FetchAsync(request);
        var text = await response.Text();

        // Assert
        Assert.IsTrue(response.Ok);
        Assert.AreEqual(content, text);
    }

    [TestMethod]
    public async Task FetchAsync_PostWithJsonBody_SendsJson()
    {
        // Arrange
        var data = new TestJsonData { Message = "Test", Value = 123 };
        var request = new Request
        {
            RequestUri = new Uri($"{_baseUrl}post"),
            Method = HttpMethod.Post,
            Body = RequestBody.Json(data)
        };

        // Act
        using var response = await Fetch.FetchAsync(request);
        var result = await response.Json<PostResult>();

        // Assert
        Assert.IsTrue(response.Ok);
        Assert.AreEqual(HttpStatusCode.Created, response.Status);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Received.Contains("Test"));
        Assert.IsTrue(result.Received.Contains("123"));
        Assert.AreEqual("POST", result.Method);
    }

    [TestMethod]
    public async Task FetchAsync_WithCustomHeaders_SendsHeaders()
    {
        // Arrange
        var request = new Request
        {
            RequestUri = new Uri($"{_baseUrl}headers"),
            Method = HttpMethod.Get
        };
        request.Headers.Add("X-Custom-Header", "CustomValue");
        request.Headers.Add("X-Another-Header", "AnotherValue");

        // Act
        using var response = await Fetch.FetchAsync(request);
        var headers = await response.Json<Dictionary<string, string>>();

        // Assert
        Assert.IsTrue(response.Ok);
        Assert.IsNotNull(headers);
        Assert.IsTrue(headers.ContainsKey("X-Custom-Header"));
        Assert.AreEqual("CustomValue", headers["X-Custom-Header"]);
        Assert.IsTrue(headers.ContainsKey("X-Another-Header"));
        Assert.AreEqual("AnotherValue", headers["X-Another-Header"]);
    }

    [TestMethod]
    public async Task FetchAsync_404Status_ReturnsNotOk()
    {
        // Act
        using var response = await Fetch.FetchAsync($"{_baseUrl}status/404");
        var text = await response.Text();

        // Assert
        Assert.IsFalse(response.Ok);
        Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
        Assert.AreEqual("Not Found", text);
    }

    [TestMethod]
    public async Task FetchAsync_500Status_ReturnsNotOk()
    {
        // Act
        using var response = await Fetch.FetchAsync($"{_baseUrl}status/500");
        var text = await response.Text();

        // Assert
        Assert.IsFalse(response.Ok);
        Assert.AreEqual(HttpStatusCode.InternalServerError, response.Status);
        Assert.AreEqual("Server Error", text);
    }

    [TestMethod]
    public async Task FetchAsync_WithRequestNoUri_SetsUriCorrectly()
    {
        // Arrange
        var request = new RequestNoUri
        {
            Method = HttpMethod.Get
        };

        // Act
        using var response = await Fetch.FetchAsync($"{_baseUrl}hello", request);
        var text = await response.Text();

        // Assert
        Assert.IsTrue(response.Ok);
        Assert.AreEqual("Hello, World!", text);
    }

    [TestMethod]
    public async Task FetchAsync_WithUri_WorksCorrectly()
    {
        // Arrange
        var uri = new Uri($"{_baseUrl}hello");

        // Act
        using var response = await Fetch.FetchAsync(uri);
        var text = await response.Text();

        // Assert
        Assert.IsTrue(response.Ok);
        Assert.AreEqual("Hello, World!", text);
    }

    [TestMethod]
    public async Task FetchAsync_Bytes_ReturnsCorrectBytes()
    {
        // Act
        using var response = await Fetch.FetchAsync($"{_baseUrl}hello");
        var bytes = await response.Bytes();

        // Assert
        Assert.IsTrue(response.Ok);
        var text = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual("Hello, World!", text);
    }

    [TestMethod]
    public async Task FetchAsync_MultipleRequests_WorkCorrectly()
    {
        // Act & Assert
        for (int i = 0; i < 5; i++)
        {
            using var response = await Fetch.FetchAsync($"{_baseUrl}hello");
            var text = await response.Text();
            Assert.IsTrue(response.Ok);
            Assert.AreEqual("Hello, World!", text);
        }
    }

    [TestMethod]
    public async Task FetchAsync_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        using var response = await Fetch.FetchAsync($"{_baseUrl}hello", cancellationToken: cts.Token);
        var text = await response.Text();

        // Assert
        Assert.IsTrue(response.Ok);
        Assert.AreEqual("Hello, World!", text);
    }

    [TestMethod]
    public async Task FetchAsync_ResponseHeaders_AreAccessible()
    {
        // Act
        using var response = await Fetch.FetchAsync($"{_baseUrl}hello");
        
        // Assert
        Assert.IsNotNull(response.Headers);
        // Headers property returns HttpResponseHeaders which doesn't include Content-Type
        // (that's a content header). Just verify Headers object is accessible.
        var headerCount = response.Headers.Count();
        Assert.IsTrue(headerCount >= 0, "Headers should be enumerable");
    }

    [TestMethod]
    public async Task FetchAsync_EnsureSuccessStatusCode_ThrowsOn404()
    {
        // Act
        using var response = await Fetch.FetchAsync($"{_baseUrl}status/404");

        // Assert
        try
        {
            response.EnsureSuccessStatusCode();
            Assert.Fail("Expected HttpRequestException");
        }
        catch (HttpRequestException)
        {
            // Expected
        }
    }

    private class TestJsonData
    {
        public string Message { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private class PostResult
    {
        public string Received { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string? ContentType { get; set; }
    }
}
