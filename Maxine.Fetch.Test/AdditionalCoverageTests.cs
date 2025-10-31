using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Maxine.Fetch.Test;

public class CoverageTestData
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(CoverageTestData))]
internal partial class CoverageTestDataContext : JsonSerializerContext
{
}

/// <summary>
/// Additional tests to improve code coverage
/// </summary>
[TestClass]
public class AdditionalCoverageTests
{
    [TestMethod]
    public void RequestNoUri_Body_CanBeSetToNull()
    {
        // Arrange
        var request = new RequestNoUri
        {
            Method = HttpMethod.Post,
            Body = "initial content"
        };

        // Act - set body to null
        request.Body = null;

        // Assert
        Assert.IsNull(request.Content);
    }

    [TestMethod]
    public async Task RequestBody_JsonWithTypeInfo_SerializesCorrectly()
    {
        // Arrange
        var data = new CoverageTestData { Name = "TypeInfo", Value = 99 };
        var typeInfo = CoverageTestDataContext.Default.CoverageTestData;

        // Act
        var body = RequestBody.Json(data, typeInfo);
        HttpContent content = body;
        var json = await content.ReadAsStringAsync();

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        var deserialized = JsonSerializer.Deserialize(json, typeInfo);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual("TypeInfo", deserialized.Name);
        Assert.AreEqual(99, deserialized.Value);
    }

    [TestMethod]
    public async Task RequestBody_JsonWithObjectTypeInfo_SerializesCorrectly()
    {
        // Arrange
        object data = new CoverageTestData { Name = "ObjectTypeInfo", Value = 123 };
        var typeInfo = CoverageTestDataContext.Default.CoverageTestData;

        // Act
        var body = RequestBody.Json(data, (JsonTypeInfo)typeInfo);
        HttpContent content = body;
        var json = await content.ReadAsStringAsync();

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        Assert.IsTrue(json.Contains("ObjectTypeInfo"));
        Assert.IsTrue(json.Contains("123"));
    }

    [TestMethod]
    public async Task RequestBody_JsonWithTypeParameter_SerializesCorrectly()
    {
        // Arrange
        object data = new CoverageTestData { Name = "TypeParam", Value = 456 };
        var type = typeof(CoverageTestData);

        // Act
        var body = RequestBody.Json(data, type);
        HttpContent content = body;
        var json = await content.ReadAsStringAsync();

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        Assert.IsTrue(json.Contains("TypeParam"));
        Assert.IsTrue(json.Contains("456"));
    }

    [TestMethod]
    public async Task Response_Url_ReturnsRequestUri()
    {
        // Arrange
        var uri = new Uri("https://example.com/test");
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            RequestMessage = httpRequest,
            Content = new StringContent("test")
        };

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.IsNotNull(response.Url);
        Assert.AreEqual(uri, response.Url);
    }

    [TestMethod]
    public void Response_Url_NullWhenNoRequestMessage()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            RequestMessage = null,
            Content = new StringContent("test")
        };

        // Act
        using var response = new Response(httpResponse);

        // Assert
        Assert.IsNull(response.Url);
    }

    [TestMethod]
    public async Task Response_JsonWithTypeInfo_DeserializesCorrectly()
    {
        // Arrange
        var data = new CoverageTestData { Name = "JsonTypeInfo", Value = 789 };
        var json = JsonSerializer.Serialize(data, CoverageTestDataContext.Default.CoverageTestData);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"))
        };

        // Act
        using var response = new Response(httpResponse);
        var result = await response.Json(CoverageTestDataContext.Default.CoverageTestData);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("JsonTypeInfo", result.Name);
        Assert.AreEqual(789, result.Value);
        Assert.IsTrue(response.BodyUsed);
    }

    [TestMethod]
    public async Task Response_JsonWithTypeInfo_AndCancellationToken_Works()
    {
        // Arrange
        var data = new CoverageTestData { Name = "WithToken", Value = 999 };
        var json = JsonSerializer.Serialize(data, CoverageTestDataContext.Default.CoverageTestData);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"))
        };
        using var cts = new CancellationTokenSource();

        // Act
        using var response = new Response(httpResponse);
        var result = await response.Json(CoverageTestDataContext.Default.CoverageTestData, cts.Token);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("WithToken", result.Name);
        Assert.AreEqual(999, result.Value);
    }

    [TestMethod]
    public async Task Response_JsonWithOptions_AndCancellationToken_Works()
    {
        // Arrange
        var data = new CoverageTestData { Name = "WithOptions", Value = 111 };
        var json = JsonSerializer.Serialize(data);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"))
        };
        var options = new JsonSerializerOptions();
        using var cts = new CancellationTokenSource();

        // Act
        using var response = new Response(httpResponse);
        var result = await response.Json<CoverageTestData>(options, cts.Token);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("WithOptions", result.Name);
        Assert.AreEqual(111, result.Value);
    }

    [TestMethod]
    public async Task FormData_AppendStringWithoutFileName_AddsCorrectly()
    {
        // Arrange
        var formData = new FormData();

        // Act
        formData.Append("field", "value", null);
        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("field"));
        Assert.IsTrue(result.Contains("value"));
    }

    [TestMethod]
    public async Task FormData_AppendByteArrayWithoutFileName_AddsCorrectly()
    {
        // Arrange
        var formData = new FormData();
        var bytes = Encoding.UTF8.GetBytes("data");

        // Act
        formData.Append("binary", bytes, null);
        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("binary"));
    }

    [TestMethod]
    public async Task FormData_AppendReadOnlyMemoryWithoutFileName_AddsCorrectly()
    {
        // Arrange
        var formData = new FormData();
        ReadOnlyMemory<byte> memory = Encoding.UTF8.GetBytes("mem");

        // Act
        formData.Append("memfield", memory, null);
        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("memfield"));
    }

    [TestMethod]
    public async Task FormData_AppendStreamWithoutFileName_AddsCorrectly()
    {
        // Arrange
        var formData = new FormData();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("stream"));

        // Act
        formData.Append("streamfield", stream, null);
        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("streamfield"));
    }
}
