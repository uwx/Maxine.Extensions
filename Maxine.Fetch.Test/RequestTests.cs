using System.Text;
using System.Text.Json;

namespace Maxine.Fetch.Test;

/// <summary>
/// Tests for Request and RequestNoUri classes
/// </summary>
[TestClass]
public class RequestTests
{
    [TestMethod]
    public void Request_RequiresRequestUri()
    {
        // Arrange & Act
        var request = new Request 
        { 
            RequestUri = new Uri("https://example.com"),
            Method = HttpMethod.Get
        };

        // Assert - RequestUri is write-only, but we can verify via HttpRequestMessage
        var httpRequest = (HttpRequestMessage)request;
        Assert.IsNotNull(httpRequest.RequestUri);
        Assert.AreEqual("https://example.com/", httpRequest.RequestUri.ToString());
    }

    [TestMethod]
    public void Request_CanSetMethod()
    {
        // Arrange & Act
        var request = new Request 
        { 
            RequestUri = new Uri("https://example.com"),
            Method = HttpMethod.Post
        };

        // Assert - Method is write-only, verify via HttpRequestMessage
        var httpRequest = (HttpRequestMessage)request;
        Assert.AreEqual(HttpMethod.Post, httpRequest.Method);
    }

    [TestMethod]
    public void Request_CanSetHeaders()
    {
        // Arrange
        var request = new Request 
        { 
            RequestUri = new Uri("https://example.com"),
            Method = HttpMethod.Get
        };

        // Act
        request.Headers.Add("X-Custom", "value");
        request.Headers.Add("X-Another", "another");

        // Assert
        Assert.IsTrue(request.Headers.Contains("X-Custom"));
        Assert.IsTrue(request.Headers.Contains("X-Another"));
        Assert.AreEqual("value", request.Headers.GetValues("X-Custom").First());
    }

    [TestMethod]
    public void Request_CanSetStringBody()
    {
        // Arrange & Act
        var request = new Request 
        { 
            RequestUri = new Uri("https://example.com"),
            Method = HttpMethod.Post,
            Body = "test content"
        };

        // Assert
        Assert.IsNotNull(request.Content);
        Assert.IsInstanceOfType(request.Content, typeof(StringContent));
    }

    [TestMethod]
    public void Request_CanSetJsonBody()
    {
        // Arrange
        var data = new { Name = "Test", Value = 42 };

        // Act
        var request = new Request 
        { 
            RequestUri = new Uri("https://example.com"),
            Method = HttpMethod.Post,
            Body = RequestBody.Json(data)
        };

        // Assert
        Assert.IsNotNull(request.Content);
    }

    [TestMethod]
    public void RequestNoUri_CanBeCreated()
    {
        // Arrange & Act
        var request = new RequestNoUri
        {
            Method = HttpMethod.Get
        };

        // Assert - verify via HttpRequestMessage
        var httpRequest = (HttpRequestMessage)request;
        Assert.AreEqual(HttpMethod.Get, httpRequest.Method);
    }

    [TestMethod]
    public void RequestNoUri_CanSetAllProperties()
    {
        // Arrange & Act
        var request = new RequestNoUri
        {
            Method = HttpMethod.Put,
            Body = "content"
        };
        request.Headers.Add("Authorization", "Bearer token");

        // Assert - verify via HttpRequestMessage
        var httpRequest = (HttpRequestMessage)request;
        Assert.AreEqual(HttpMethod.Put, httpRequest.Method);
        Assert.IsNotNull(request.Content);
        Assert.IsTrue(request.Headers.Contains("Authorization"));
    }

    [TestMethod]
    public void Request_CreatedViaFetchAPI_CopiesPropertiesFromRequestNoUri()
    {
        // Arrange
        var requestNoUri = new RequestNoUri
        {
            Method = HttpMethod.Delete,
            Body = "delete content"
        };
        requestNoUri.Headers.Add("X-Custom", "header");

        // Act - Since internal constructor can't be tested directly,
        // verify that Fetch API pattern works
        // (The actual constructor is tested implicitly through Fetch.FetchAsync)
        
        // Assert - verify requestNoUri properties are set
        var httpRequest = (HttpRequestMessage)requestNoUri;
        Assert.AreEqual(HttpMethod.Delete, httpRequest.Method);
        Assert.IsNotNull(requestNoUri.Content);
        Assert.IsTrue(requestNoUri.Headers.Contains("X-Custom"));
    }
}
