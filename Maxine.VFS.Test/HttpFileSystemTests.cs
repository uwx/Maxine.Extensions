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
    public void DirectoryExists_ReturnsFalseIfNoKnownFiles()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        Assert.IsFalse(fs.DirectoryExists(""));
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

        var files = fs.EnumerateFiles("", "*.txt", SearchOption.AllDirectories).ToList();

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

        var dirs = fs.EnumerateDirectories("", "*", SearchOption.AllDirectories).ToList();

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

    [TestMethod]
    public void AddKnownFiles_WithSingleFile_AddsFileToFileSystem()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[] { "file.txt" });

        Assert.IsTrue(fs.FileExists("file.txt"));
    }

    [TestMethod]
    public void AddKnownFiles_WithMultipleFiles_AddsAllFiles()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[] { "file1.txt", "file2.txt", "file3.txt" });

        Assert.IsTrue(fs.FileExists("file1.txt"));
        Assert.IsTrue(fs.FileExists("file2.txt"));
        Assert.IsTrue(fs.FileExists("file3.txt"));
    }

    [TestMethod]
    public void AddKnownFiles_WithNestedPaths_CreatesDirectories()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[] { "dir/subdir/file.txt" });

        Assert.IsTrue(fs.DirectoryExists("dir"));
        Assert.IsTrue(fs.DirectoryExists("dir/subdir"));
        Assert.IsTrue(fs.FileExists("dir/subdir/file.txt"));
    }

    [TestMethod]
    public void AddKnownFiles_WithFileInRoot_WorksCorrectly()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[] { "root.txt" });

        Assert.IsTrue(fs.FileExists("root.txt"));
        var files = fs.GetFiles("").ToList();
        Assert.IsTrue(files.Any(f => f.EndsWith("root.txt")));
    }

    [TestMethod]
    public void AddKnownFiles_CreatesParentDirectoriesInOrder()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[] { "a/b/c/d/file.txt" });

        Assert.IsTrue(fs.DirectoryExists("a"));
        Assert.IsTrue(fs.DirectoryExists("a/b"));
        Assert.IsTrue(fs.DirectoryExists("a/b/c"));
        Assert.IsTrue(fs.DirectoryExists("a/b/c/d"));
        Assert.IsTrue(fs.FileExists("a/b/c/d/file.txt"));
    }

    [TestMethod]
    public void AddKnownFiles_AllowsEnumeratingFiles()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[] { "dir/file1.txt", "dir/file2.txt", "dir/subdir/file3.txt" });

        var files = fs.EnumerateFiles("dir").ToList();
        Assert.AreEqual(2, files.Count);
        Assert.IsTrue(files.Any(f => f.EndsWith("file1.txt")));
        Assert.IsTrue(files.Any(f => f.EndsWith("file2.txt")));
    }

    [TestMethod]
    public void AddKnownFiles_AllowsEnumeratingDirectories()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[] { "dir1/file.txt", "dir2/file.txt", "dir3/subdir/file.txt" });

        var dirs = fs.EnumerateDirectories("").ToList();
        Assert.AreEqual(3, dirs.Count);
        Assert.IsTrue(dirs.Any(d => d.EndsWith("dir1")));
        Assert.IsTrue(dirs.Any(d => d.EndsWith("dir2")));
        Assert.IsTrue(dirs.Any(d => d.EndsWith("dir3")));
    }

    [TestMethod]
    public void AddKnownFiles_WithDuplicateFiles_OverwritesEntry()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[] { "file.txt" });
        fs.AddKnownFiles(new[] { "file.txt" }); // Add again

        Assert.IsTrue(fs.FileExists("file.txt"));
        var files = fs.GetFiles("").ToList();
        Assert.AreEqual(1, files.Count(f => f.EndsWith("file.txt")));
    }

    [TestMethod]
    public void AddKnownFiles_AllowsOpeningFiles()
    {
        var requestedPath = "";
        var client = CreateMockClient(req =>
        {
            requestedPath = req.RequestUri?.AbsolutePath ?? "";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Test content")
            };
        });

        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));
        fs.AddKnownFiles(new[] { "dir/file.txt" });

        using var stream = fs.OpenRead("dir/file.txt");
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        Assert.AreEqual("Test content", content);
        Assert.AreEqual("/dir/file.txt", requestedPath);
    }

    [TestMethod]
    public void AddKnownFiles_WithSearchPattern_FiltersCorrectly()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[] { "dir/file1.txt", "dir/file2.doc", "dir/file3.txt" });

        var txtFiles = fs.GetFiles("dir", "*.txt").ToList();
        Assert.AreEqual(2, txtFiles.Count);
        Assert.IsTrue(txtFiles.All(f => f.EndsWith(".txt")));
    }

    [TestMethod]
    public void AddKnownFiles_WithRecursiveSearch_FindsAllFiles()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[]
        {
            "dir/file1.txt",
            "dir/subdir1/file2.txt",
            "dir/subdir2/file3.txt",
            "dir/subdir1/deepdir/file4.txt"
        });

        var allFiles = fs.GetFiles("dir", "*", SearchOption.AllDirectories).ToList();
        Assert.AreEqual(4, allFiles.Count);
    }

    [TestMethod]
    public void AddKnownFiles_WithEmptyList_DoesNotThrow()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(Array.Empty<string>());

        var files = fs.GetFiles("").ToList();
        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void AddKnownFiles_WithMixedPaths_OrganizesCorrectly()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[]
        {
            "root.txt",
            "dir1/file1.txt",
            "dir2/subdir/file2.txt",
            "another.txt"
        });

        Assert.AreEqual(2, fs.GetFiles("").Count);
        Assert.AreEqual(1, fs.GetFiles("dir1").Count);
        Assert.AreEqual(1, fs.GetFiles("dir2/subdir").Count);
        Assert.AreEqual(2, fs.GetDirectories("").Count);
    }

    [TestMethod]
    public void AddKnownFiles_DirectoryExistsForParent_ReturnsTrue()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[] { "parent/child/grandchild/file.txt" });

        Assert.IsTrue(fs.DirectoryExists("parent"));
        Assert.IsTrue(fs.DirectoryExists("parent/child"));
        Assert.IsTrue(fs.DirectoryExists("parent/child/grandchild"));
        Assert.IsFalse(fs.DirectoryExists("parent/nonexistent"));
    }

    [TestMethod]
    public void AddKnownFiles_EnumerateDirectoriesRecursively_FindsAll()
    {
        var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK));
        using var fs = new HttpFileSystem(client, path => new Uri($"http://example.com/{path}"));

        fs.AddKnownFiles(new[]
        {
            "a/b/c/file1.txt",
            "a/d/file2.txt",
            "a/e/f/file3.txt"
        });

        var allDirs = fs.GetDirectories("a", "*", SearchOption.AllDirectories).ToList();
        Assert.IsTrue(allDirs.Count >= 4); // b, c, d, e, f
    }
}
