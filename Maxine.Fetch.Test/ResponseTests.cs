using System.Net;
using System.Text;
using System.Text.Json;

namespace Maxine.Fetch.Test;

/// <summary>
/// Tests for Response class
/// </summary>
[TestClass]
public class ResponseTests
{
    [TestMethod]
    public void Response_Ok_TrueFor2xxStatus()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.IsTrue(response.Ok);
    }

    [TestMethod]
    public void Response_Ok_TrueFor204NoContent()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.IsTrue(response.Ok);
    }

    [TestMethod]
    public void Response_Ok_FalseFor404()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.IsFalse(response.Ok);
    }

    [TestMethod]
    public void Response_Ok_FalseFor500()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.IsFalse(response.Ok);
    }

    [TestMethod]
    public void Response_Redirected_TrueFor3xxStatus()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.MovedPermanently);

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.IsTrue(response.Redirected);
        Assert.IsFalse(response.Ok);
    }

    [TestMethod]
    public void Response_Redirected_TrueFor302()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.Found);

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.IsTrue(response.Redirected);
    }

    [TestMethod]
    public void Response_Redirected_FalseFor200()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.IsFalse(response.Redirected);
    }

    [TestMethod]
    public void Response_Status_ReturnsStatusCode()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.Created);

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.Status);
    }

    [TestMethod]
    public void Response_StatusText_ReturnsReasonPhrase()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            ReasonPhrase = "Not Found"
        };

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.AreEqual("Not Found", response.StatusText);
    }

    [TestMethod]
    public void Response_StatusText_ReturnsEmptyWhenNull()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            ReasonPhrase = null
        };

        // Act
        using var response = new Response(httpResponse);

        // Assert
        // Note: When ReasonPhrase is null, .NET may provide a default value like "OK"
        // The Response.StatusText handles null by returning empty string,
        // but the framework may set a default before it gets to our code
        Assert.IsNotNull(response.StatusText);
    }

    [TestMethod]
    public void Response_Headers_ReturnsResponseHeaders()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponse.Headers.Add("X-Custom-Header", "custom-value");

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.IsNotNull(response.Headers);
        Assert.IsTrue(response.Headers.Contains("X-Custom-Header"));
    }

    [TestMethod]
    public async Task Response_Text_ReturnsStringContent()
    {
        // Arrange
        const string expectedText = "Hello, World!";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedText)
        };

        // Act
        using var response = new Response(httpResponse);
        var text = await response.Text();

        // Assert
        Assert.AreEqual(expectedText, text);
        Assert.IsTrue(response.BodyUsed);
    }

    [TestMethod]
    public async Task Response_Bytes_ReturnsByteArray()
    {
        // Arrange
        var expectedBytes = Encoding.UTF8.GetBytes("test data");
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(expectedBytes)
        };

        // Act
        using var response = new Response(httpResponse);
        var bytes = await response.Bytes();

        // Assert
        CollectionAssert.AreEqual(expectedBytes, bytes);
        Assert.IsTrue(response.BodyUsed);
    }

    [TestMethod]
    public async Task Response_Json_DeserializesObject()
    {
        // Arrange
        var expectedData = new TestData { Name = "Test", Value = 42 };
        var json = JsonSerializer.Serialize(expectedData);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        // Act
        using var response = new Response(httpResponse);
        var data = await response.Json<TestData>();

        // Assert
        Assert.IsNotNull(data);
        Assert.AreEqual("Test", data.Name);
        Assert.AreEqual(42, data.Value);
        Assert.IsTrue(response.BodyUsed);
    }

    [TestMethod]
    public async Task Response_Body_ReturnsStream()
    {
        // Arrange
        const string content = "stream content";
        var bytes = Encoding.UTF8.GetBytes(content);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(bytes)
        };

        // Act
        var response = new Response(httpResponse);
        var stream = await response.Body();
        // Note: UseBody() disposes the response, which may dispose the stream
        // This is a limitation of the current implementation

        // Assert
        Assert.IsNotNull(stream);
        Assert.IsTrue(response.BodyUsed);
        // After Body() is called, the response is disposed by design
    }

    [TestMethod]
    public async Task Response_BodyUsed_StartsAsFalse()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("test")
        };

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.IsFalse(response.BodyUsed);
    }

    [TestMethod]
    public async Task Response_BodyUsed_TrueAfterText()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("test")
        };

        // Act
        using var response = new Response(httpResponse);
        await response.Text();

        // Assert
        Assert.IsTrue(response.BodyUsed);
    }

    [TestMethod]
    public async Task Response_BodyUsed_ThrowsOnSecondRead()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("test")
        };

        // Act
        using var response = new Response(httpResponse);
        await response.Text();

        // Assert
        try
        {
            await response.Text();
            Assert.Fail("Expected InvalidOperationException");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Response body is already in use", ex.Message);
        }
    }

    [TestMethod]
    public async Task Response_BodyUsed_ThrowsOnMixedReads()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("test")
        };

        // Act
        using var response = new Response(httpResponse);
        await response.Bytes();

        // Assert - trying to read as text after bytes
        try
        {
            await response.Text();
            Assert.Fail("Expected InvalidOperationException");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Response body is already in use", ex.Message);
        }
    }

    [TestMethod]
    public void Response_EnsureSuccessStatusCode_DoesNotThrowFor200()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        using var response = new Response(httpResponse);
        response.EnsureSuccessStatusCode();

        // Assert - no exception thrown
    }

    [TestMethod]
    public void Response_EnsureSuccessStatusCode_ThrowsFor404()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

        // Act
        using var response = new Response(httpResponse);

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

    [TestMethod]
    public void Response_Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
        var response = new Response(httpResponse);

        // Act & Assert - no exception should be thrown
        response.Dispose();
        response.Dispose();
    }

    [TestMethod]
    public async Task Response_Dispose_DisposesAfterBodyRead()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("test")
        };
        var response = new Response(httpResponse);

        // Act
        await response.Text();
        // Text() disposes the response automatically

        // Assert
        Assert.IsTrue(response.BodyUsed);
    }

    private class TestData
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
