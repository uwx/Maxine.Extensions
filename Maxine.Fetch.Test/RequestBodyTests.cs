using System.Text;
using System.Text.Json;

namespace Maxine.Fetch.Test;

/// <summary>
/// Tests for RequestBody class
/// </summary>
[TestClass]
public class RequestBodyTests
{
    [TestMethod]
    public void RequestBody_Json_CreatesJsonContent()
    {
        // Arrange
        var data = new { Name = "Test", Value = 42 };

        // Act
        var body = RequestBody.Json(data);

        // Assert
        Assert.IsNotNull(body);
        HttpContent content = body; // implicit conversion
        Assert.IsNotNull(content);
    }

    [TestMethod]
    public async Task RequestBody_Json_SerializesCorrectly()
    {
        // Arrange
        var data = new TestData { Name = "Test", Value = 42 };

        // Act
        var body = RequestBody.Json(data);
        HttpContent content = body;
        var json = await content.ReadAsStringAsync();

        // Assert - verify JSON was created
        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        // Default System.Text.Json uses PascalCase property names
        Assert.IsTrue(json.Contains("Name") || json.Contains("\"name\""), "JSON should contain Name property");
        Assert.IsTrue(json.Contains("Value") || json.Contains("\"value\""), "JSON should contain Value property");
        Assert.IsTrue(json.Contains("42"), "JSON should contain value 42");
    }

    [TestMethod]
    public void RequestBody_ImplicitString_CreatesStringContent()
    {
        // Arrange & Act
        RequestBody body = "test content";

        // Assert
        Assert.IsNotNull(body);
        HttpContent content = body;
        Assert.IsInstanceOfType(content, typeof(StringContent));
    }

    [TestMethod]
    public async Task RequestBody_ImplicitString_ContainsCorrectContent()
    {
        // Arrange
        const string testString = "Hello, World!";

        // Act
        RequestBody body = testString;
        HttpContent content = body;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.AreEqual(testString, result);
    }

    [TestMethod]
    public void RequestBody_ImplicitByteArray_CreatesByteArrayContent()
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes("test bytes");

        // Act
        RequestBody body = bytes;

        // Assert
        Assert.IsNotNull(body);
        HttpContent content = body;
        Assert.IsInstanceOfType(content, typeof(ByteArrayContent));
    }

    [TestMethod]
    public async Task RequestBody_ImplicitByteArray_ContainsCorrectContent()
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes("test bytes");

        // Act
        RequestBody body = bytes;
        HttpContent content = body;
        var result = await content.ReadAsByteArrayAsync();

        // Assert
        CollectionAssert.AreEqual(bytes, result);
    }

    [TestMethod]
    public void RequestBody_ImplicitReadOnlyMemory_CreatesContent()
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes("memory test");
        ReadOnlyMemory<byte> memory = bytes;

        // Act
        RequestBody body = memory;

        // Assert
        Assert.IsNotNull(body);
        HttpContent content = body;
        Assert.IsNotNull(content);
    }

    [TestMethod]
    public async Task RequestBody_ImplicitReadOnlyMemory_ContainsCorrectContent()
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes("memory test");
        ReadOnlyMemory<byte> memory = bytes;

        // Act
        RequestBody body = memory;
        HttpContent content = body;
        var result = await content.ReadAsByteArrayAsync();

        // Assert
        CollectionAssert.AreEqual(bytes, result);
    }

    [TestMethod]
    public void RequestBody_ImplicitStream_CreatesStreamContent()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("stream test"));

        // Act
        RequestBody body = stream;

        // Assert
        Assert.IsNotNull(body);
        HttpContent content = body;
        Assert.IsInstanceOfType(content, typeof(StreamContent));
    }

    [TestMethod]
    public async Task RequestBody_ImplicitStream_ContainsCorrectContent()
    {
        // Arrange
        var expectedBytes = Encoding.UTF8.GetBytes("stream test");
        var stream = new MemoryStream(expectedBytes);

        // Act
        RequestBody body = stream;
        HttpContent content = body;
        var result = await content.ReadAsByteArrayAsync();

        // Assert
        CollectionAssert.AreEqual(expectedBytes, result);
    }

    [TestMethod]
    public void RequestBody_CanBeUsedInRequest()
    {
        // Arrange
        var data = new { Message = "test" };
        var body = RequestBody.Json(data);

        // Act
        var request = new Request
        {
            RequestUri = new Uri("https://example.com"),
            Method = HttpMethod.Post,
            Body = body
        };

        // Assert
        Assert.IsNotNull(request.Content);
    }

    private class TestData
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
