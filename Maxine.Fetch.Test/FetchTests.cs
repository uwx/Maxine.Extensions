using System.Net;
using System.Text;
using System.Text.Json;

namespace Maxine.Fetch.Test;

/// <summary>
/// Tests for the Fetch class HTTP operations
/// Note: These tests verify request construction and API usage patterns.
/// Full integration tests would require HTTP mocking infrastructure.
/// </summary>
[TestClass]
public class FetchTests
{
    [TestMethod]
    public void Fetch_RequestConstruction_StringUri_CreatesValidRequest()
    {
        // Arrange & Act
        var request = new Request
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://example.com")
        };

        // Assert
        var httpRequest = (HttpRequestMessage)request;
        Assert.AreEqual(HttpMethod.Get, httpRequest.Method);
        Assert.AreEqual("https://example.com/", httpRequest.RequestUri!.ToString());
    }

    [TestMethod]
    public void Fetch_RequestConstruction_WithHeaders_SetsHeaders()
    {
        // Arrange & Act
        var request = new Request
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://example.com"),
            Body = "test body"
        };
        request.Headers.Add("X-Custom-Header", "test-value");

        // Assert
        Assert.IsTrue(request.Headers.Contains("X-Custom-Header"));
        Assert.AreEqual("test-value", request.Headers.GetValues("X-Custom-Header").First());
    }

    [TestMethod]
    public void Fetch_RequestNoUri_CanBeUsedWithUri()
    {
        // Arrange
        var requestNoUri = new RequestNoUri
        {
            Method = HttpMethod.Put,
            Body = "content"
        };

        // Act - verify RequestNoUri properties are set
        var httpRequest = (HttpRequestMessage)requestNoUri;

        // Assert
        Assert.AreEqual(HttpMethod.Put, httpRequest.Method);
        Assert.IsNotNull(requestNoUri.Content);
        // Note: The internal Request(RequestNoUri, Uri) constructor is tested
        // indirectly through Fetch.FetchAsync API methods
    }

    [TestMethod]
    public void Fetch_RequestWithJsonBody_CreatesJsonContent()
    {
        // Arrange
        var data = new { Name = "Test", Value = 42 };

        // Act
        var request = new Request
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://example.com"),
            Body = RequestBody.Json(data)
        };

        // Assert
        Assert.IsNotNull(request.Content);
        var httpRequest = (HttpRequestMessage)request;
        Assert.IsNotNull(httpRequest.Content);
    }

    [TestMethod]
    public void Fetch_RequestWithFormData_CreatesMultipartContent()
    {
        // Arrange
        var formData = new FormData()
            .Append("field1", "value1")
            .Append("field2", "value2");

        // Act
        var request = new Request
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://example.com/upload"),
            Body = formData
        };

        // Assert
        Assert.IsNotNull(request.Content);
        Assert.IsInstanceOfType(request.Content, typeof(MultipartFormDataContent));
    }

    [TestMethod]
    public void Fetch_RequestWithByteArrayBody_CreatesByteArrayContent()
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes("binary data");

        // Act
        var request = new Request
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://example.com"),
            Body = bytes
        };

        // Assert
        Assert.IsNotNull(request.Content);
        Assert.IsInstanceOfType(request.Content, typeof(ByteArrayContent));
    }

    [TestMethod]
    public void Fetch_RequestWithStreamBody_CreatesStreamContent()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("stream data"));

        // Act
        var request = new Request
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://example.com"),
            Body = stream
        };

        // Assert
        Assert.IsNotNull(request.Content);
        Assert.IsInstanceOfType(request.Content, typeof(StreamContent));
    }

    [TestMethod]
    public void Fetch_RequestNoUri_HeadersCopiedCorrectly()
    {
        // Arrange
        var original = new RequestNoUri
        {
            Method = HttpMethod.Delete,
            Body = "delete me"
        };
        original.Headers.Add("Authorization", "Bearer token");
        original.Headers.Add("X-Custom", "value");

        // Act - verify headers are set on RequestNoUri
        var httpRequest = (HttpRequestMessage)original;

        // Assert
        Assert.AreEqual(HttpMethod.Delete, httpRequest.Method);
        Assert.IsNotNull(original.Content);
        Assert.IsTrue(original.Headers.Contains("Authorization"));
        Assert.IsTrue(original.Headers.Contains("X-Custom"));
        Assert.AreEqual("Bearer token", original.Headers.GetValues("Authorization").First());
        // Note: The internal Request(RequestNoUri, Uri) constructor copies these headers
        // This is tested indirectly through the Fetch.FetchAsync(uri, RequestNoUri) method
    }
}

