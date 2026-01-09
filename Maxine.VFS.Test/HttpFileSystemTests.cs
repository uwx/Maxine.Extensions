using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Maxine.VFS.Test;

[TestClass]
public class HttpFileSystemTests
{
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handler(request);
        }
    }

    private static HttpClient CreateMockClient(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        return new HttpClient(new MockHttpMessageHandler(handler))
        {
            BaseAddress = new Uri("http://example.com/")
        };
    }

    [TestMethod]
    public void Constructor_WithBaseUri_CreatesFileSystem()
    {
        var baseUri = new Uri("http://example.com/");
        using var fs = new HttpFileSystem(baseUri);

        Assert.IsNotNull(fs);
    }

    [TestMethod]
    public void Constructor_WithClientAndUriBuilder_CreatesFileSystem()
    {
        var client = new HttpClient();
        var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        Assert.IsNotNull(fs);
        fs.Dispose();
    }

    [TestMethod]
    public void FileExists_WhenResourceExists_ReturnsTrue()
    {
        var client = CreateMockClient(req =>
        {
            if (req.Method == HttpMethod.Head)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        Assert.IsTrue(fs.FileExists("file.txt"));
    }

    [TestMethod]
    public void FileExists_WhenResourceNotFound_ReturnsFalse()
    {
        var client = CreateMockClient(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        Assert.IsFalse(fs.FileExists("nonexistent.txt"));
    }

    [TestMethod]
    public void FileExists_WithVariousStatusCodes_ReturnsCorrectly()
    {
        var statusCodes = new[]
        {
            (HttpStatusCode.OK, true),
            (HttpStatusCode.NotFound, false),
            (HttpStatusCode.Forbidden, false),
            (HttpStatusCode.Unauthorized, false),
            (HttpStatusCode.InternalServerError, false)
        };

        foreach (var (statusCode, expected) in statusCodes)
        {
            var client = CreateMockClient(req => new HttpResponseMessage(statusCode));
            using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

            Assert.AreEqual(expected, fs.FileExists("file.txt"), $"Failed for status code {statusCode}");
        }
    }

    [TestMethod]
    public void DirectoryExists_AlwaysReturnsFalse()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        Assert.IsFalse(fs.DirectoryExists(""));
        Assert.IsFalse(fs.DirectoryExists("dir"));
        Assert.IsFalse(fs.DirectoryExists("dir/subdir"));
    }

    [TestMethod]
    public void OpenRead_ReturnsStreamWithContent()
    {
        var expectedContent = "Hello, World!";
        var client = CreateMockClient(req =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };
            return response;
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));
        using var stream = fs.OpenRead("file.txt");
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        Assert.AreEqual(expectedContent, content);
    }

    [TestMethod]
    public void OpenRead_WithNonExistentFile_ThrowsException()
    {
        var client = CreateMockClient(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        Assert.Throws<HttpRequestException>(() => fs.OpenRead("nonexistent.txt"));
    }

    [TestMethod]
    public void OpenRead_WithBinaryContent_ReturnsCorrectData()
    {
        var expectedData = new byte[] { 1, 2, 3, 4, 5, 255 };
        var client = CreateMockClient(req =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(expectedData)
            };
            return response;
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));
        using var stream = fs.OpenRead("data.bin");
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        var actualData = ms.ToArray();

        CollectionAssert.AreEqual(expectedData, actualData);
    }

    [TestMethod]
    public async Task OpenReadAsync_ReturnsStreamWithContent()
    {
        var expectedContent = "Async Hello!";
        var client = CreateMockClient(req =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };
            return response;
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));
        await using var stream = await fs.OpenReadAsync("file.txt");
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        Assert.AreEqual(expectedContent, content);
    }

    [TestMethod]
    public async Task OpenReadAsync_WithNonExistentFile_ThrowsException()
    {
        var client = CreateMockClient(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        try
        {
            await fs.OpenReadAsync("nonexistent.txt");
            Assert.Fail("Expected HttpRequestException was not thrown");
        }
        catch (HttpRequestException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void ReadAllText_ReadsContentCorrectly()
    {
        var expectedContent = "File content from HTTP";
        var client = CreateMockClient(req =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };
            return response;
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));
        var content = fs.ReadAllText("file.txt");

        Assert.AreEqual(expectedContent, content);
    }

    [TestMethod]
    public void ReadAllBytes_ReadsBinaryContentCorrectly()
    {
        var expectedData = new byte[] { 10, 20, 30, 40, 50 };
        var client = CreateMockClient(req =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(expectedData)
            };
            return response;
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));
        var data = fs.ReadAllBytes("data.bin");

        CollectionAssert.AreEqual(expectedData, data);
    }

    [TestMethod]
    public async Task ReadAllTextAsync_ReadsContentCorrectly()
    {
        var expectedContent = "Async file content";
        var client = CreateMockClient(req =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };
            return response;
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));
        var content = await fs.ReadAllTextAsync("file.txt");

        Assert.AreEqual(expectedContent, content);
    }

    [TestMethod]
    public async Task ReadAllBytesAsync_ReadsBinaryContentCorrectly()
    {
        var expectedData = new byte[] { 100, 200, 255 };
        var client = CreateMockClient(req =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(expectedData)
            };
            return response;
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));
        var data = await fs.ReadAllBytesAsync("data.bin");

        CollectionAssert.AreEqual(expectedData, data);
    }

    [TestMethod]
    public void EnumerateFiles_AlwaysReturnsEmpty()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        var files = fs.EnumerateFiles("").ToList();

        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void EnumerateFiles_WithPatternAndSearchOption_ReturnsEmpty()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        var files = fs.EnumerateFiles("dir", "*.txt", SearchOption.AllDirectories).ToList();

        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void EnumerateDirectories_AlwaysReturnsEmpty()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        var dirs = fs.EnumerateDirectories("").ToList();

        Assert.AreEqual(0, dirs.Count);
    }

    [TestMethod]
    public void EnumerateDirectories_WithPatternAndSearchOption_ReturnsEmpty()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        var dirs = fs.EnumerateDirectories("dir", "*", SearchOption.AllDirectories).ToList();

        Assert.AreEqual(0, dirs.Count);
    }

    [TestMethod]
    public void GetFiles_ReturnsEmptyList()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        var files = fs.GetFiles("");

        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void GetDirectories_ReturnsEmptyList()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        var dirs = fs.GetDirectories("");

        Assert.AreEqual(0, dirs.Count);
    }

    [TestMethod]
    public void GetAttributes_AlwaysReturnsReadOnly()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        var attributes = fs.GetAttributes("file.txt");

        Assert.AreEqual(FileAttributes.ReadOnly, attributes);
    }

    [TestMethod]
    public void UriBuilder_ConstructsCorrectUris()
    {
        Uri? capturedUri = null;
        var client = CreateMockClient(req =>
        {
            capturedUri = req.RequestUri;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("test")
            };
        });

        Func<string, Uri> uriBuilder = path => new Uri($"http://example.com/api/{path}");
        using var fs = new HttpFileSystem(client, uriBuilder);

        fs.ReadAllText("file.txt");

        Assert.IsNotNull(capturedUri);
        Assert.AreEqual("http://example.com/api/file.txt", capturedUri.ToString());
    }

    [TestMethod]
    public void UriBuilder_WithNestedPath_ConstructsCorrectUri()
    {
        Uri? capturedUri = null;
        var client = CreateMockClient(req =>
        {
            capturedUri = req.RequestUri;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("test")
            };
        });

        Func<string, Uri> uriBuilder = path => new Uri($"http://example.com/{path}");
        using var fs = new HttpFileSystem(client, uriBuilder);

        fs.ReadAllText("folder/subfolder/file.txt");

        Assert.IsNotNull(capturedUri);
        Assert.AreEqual("http://example.com/folder/subfolder/file.txt", capturedUri.ToString());
    }

    [TestMethod]
    public void Dispose_DisposesHttpClient()
    {
        var client = new HttpClient(new MockHttpMessageHandler(req => new HttpResponseMessage(HttpStatusCode.OK)));
        var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.Dispose();

        // Attempting to use client after disposal should throw
        Assert.Throws<ObjectDisposedException>(() => client.GetAsync("http://example.com/test").Wait());
    }

    [TestMethod]
    public async Task DisposeAsync_DisposesHttpClient()
    {
        var client = new HttpClient(new MockHttpMessageHandler(req => new HttpResponseMessage(HttpStatusCode.OK)));
        var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        await fs.DisposeAsync();

        // Attempting to use client after disposal should throw
        Assert.Throws<ObjectDisposedException>(() => client.GetAsync("http://example.com/test").Wait());
    }

    [TestMethod]
    public void MultipleRequests_WorkCorrectly()
    {
        var requestCount = 0;
        var client = CreateMockClient(req =>
        {
            requestCount++;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"Response {requestCount}")
            };
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        var content1 = fs.ReadAllText("file1.txt");
        var content2 = fs.ReadAllText("file2.txt");
        var content3 = fs.ReadAllText("file3.txt");

        Assert.AreEqual("Response 1", content1);
        Assert.AreEqual("Response 2", content2);
        Assert.AreEqual("Response 3", content3);
        Assert.AreEqual(3, requestCount);
    }

    [TestMethod]
    public void Exists_ReturnsFalseForNonExistentResource()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.NotFound));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        Assert.IsFalse(fs.Exists("file.txt"));
    }

    [TestMethod]
    public void Exists_ReturnsTrueForExistingResource()
    {
        var client = CreateMockClient(req =>
        {
            if (req.Method == HttpMethod.Head)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        Assert.IsTrue(fs.Exists("file.txt"));
    }

    [TestMethod]
    public void BaseUriConstructor_WorksWithTrailingSlash()
    {
        var baseUri = new Uri("http://example.com/");
        using var fs = new HttpFileSystem(baseUri);

        Assert.IsNotNull(fs);
    }

    [TestMethod]
    public void BaseUriConstructor_WorksWithoutTrailingSlash()
    {
        var baseUri = new Uri("http://example.com");
        using var fs = new HttpFileSystem(baseUri);

        Assert.IsNotNull(fs);
    }

    [TestMethod]
    public void FileExists_UsesHeadRequest()
    {
        HttpMethod? capturedMethod = null;
        var client = CreateMockClient(req =>
        {
            capturedMethod = req.Method;
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));
        fs.FileExists("file.txt");

        Assert.AreEqual(HttpMethod.Head, capturedMethod);
    }

    [TestMethod]
    public void OpenRead_LargeContent_StreamsCorrectly()
    {
        var largeData = new byte[1024 * 1024]; // 1 MB
        for (int i = 0; i < largeData.Length; i++)
        {
            largeData[i] = (byte)(i % 256);
        }

        var client = CreateMockClient(req =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(largeData)
            };
            return response;
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));
        using var stream = fs.OpenRead("largefile.bin");
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        var actualData = ms.ToArray();

        Assert.AreEqual(largeData.Length, actualData.Length);
        CollectionAssert.AreEqual(largeData, actualData);
    }
}
