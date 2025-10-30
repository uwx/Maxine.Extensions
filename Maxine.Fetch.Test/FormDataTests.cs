using System.Text;

namespace Maxine.Fetch.Test;

/// <summary>
/// Tests for FormData class
/// </summary>
[TestClass]
public class FormDataTests
{
    [TestMethod]
    public void FormData_CanBeCreated()
    {
        // Act
        var formData = new FormData();

        // Assert
        Assert.IsNotNull(formData);
        HttpContent content = formData;
        Assert.IsInstanceOfType(content, typeof(MultipartFormDataContent));
    }

    [TestMethod]
    public void FormData_AppendString_ReturnsFluentInterface()
    {
        // Arrange
        var formData = new FormData();

        // Act
        var result = formData.Append("field1", "value1");

        // Assert
        Assert.AreSame(formData, result);
    }

    [TestMethod]
    public async Task FormData_AppendString_AddsField()
    {
        // Arrange
        var formData = new FormData();

        // Act
        formData.Append("field1", "value1");
        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("field1"));
        Assert.IsTrue(result.Contains("value1"));
    }

    [TestMethod]
    public async Task FormData_AppendStringWithFileName_AddsFileField()
    {
        // Arrange
        var formData = new FormData();

        // Act
        formData.Append("file", "file content", "test.txt");
        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("file"));
        Assert.IsTrue(result.Contains("test.txt"));
        Assert.IsTrue(result.Contains("file content"));
    }

    [TestMethod]
    public void FormData_AppendByteArray_ReturnsFluentInterface()
    {
        // Arrange
        var formData = new FormData();
        var bytes = Encoding.UTF8.GetBytes("test");

        // Act
        var result = formData.Append("data", bytes);

        // Assert
        Assert.AreSame(formData, result);
    }

    [TestMethod]
    public async Task FormData_AppendByteArray_AddsField()
    {
        // Arrange
        var formData = new FormData();
        var bytes = Encoding.UTF8.GetBytes("binary data");

        // Act
        formData.Append("binaryField", bytes);
        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("binaryField"));
        Assert.IsTrue(result.Contains("binary data"));
    }

    [TestMethod]
    public async Task FormData_AppendByteArrayWithFileName_AddsFileField()
    {
        // Arrange
        var formData = new FormData();
        var bytes = Encoding.UTF8.GetBytes("file data");

        // Act
        formData.Append("upload", bytes, "data.bin");
        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("upload"));
        Assert.IsTrue(result.Contains("data.bin"));
    }

    [TestMethod]
    public void FormData_AppendReadOnlyMemory_ReturnsFluentInterface()
    {
        // Arrange
        var formData = new FormData();
        ReadOnlyMemory<byte> memory = Encoding.UTF8.GetBytes("memory");

        // Act
        var result = formData.Append("memField", memory);

        // Assert
        Assert.AreSame(formData, result);
    }

    [TestMethod]
    public async Task FormData_AppendReadOnlyMemory_AddsField()
    {
        // Arrange
        var formData = new FormData();
        ReadOnlyMemory<byte> memory = Encoding.UTF8.GetBytes("memory content");

        // Act
        formData.Append("memoryField", memory);
        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("memoryField"));
        Assert.IsTrue(result.Contains("memory content"));
    }

    [TestMethod]
    public void FormData_AppendStream_ReturnsFluentInterface()
    {
        // Arrange
        var formData = new FormData();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("stream"));

        // Act
        var result = formData.Append("streamField", stream);

        // Assert
        Assert.AreSame(formData, result);
    }

    [TestMethod]
    public async Task FormData_AppendStream_AddsField()
    {
        // Arrange
        var formData = new FormData();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("stream data"));

        // Act
        formData.Append("streamField", stream);
        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("streamField"));
        Assert.IsTrue(result.Contains("stream data"));
    }

    [TestMethod]
    public async Task FormData_AppendStreamWithFileName_AddsFileField()
    {
        // Arrange
        var formData = new FormData();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("stream file"));

        // Act
        formData.Append("file", stream, "stream.dat");
        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("file"));
        Assert.IsTrue(result.Contains("stream.dat"));
    }

    [TestMethod]
    public async Task FormData_ChainMultipleAppends_AllFieldsAdded()
    {
        // Arrange
        var formData = new FormData();
        var bytes = Encoding.UTF8.GetBytes("bytes");
        ReadOnlyMemory<byte> memory = Encoding.UTF8.GetBytes("memory");
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("stream"));

        // Act
        formData
            .Append("field1", "value1")
            .Append("field2", bytes)
            .Append("field3", memory)
            .Append("field4", stream)
            .Append("file", "content", "test.txt");

        HttpContent content = formData;
        var result = await content.ReadAsStringAsync();

        // Assert
        Assert.IsTrue(result.Contains("field1"));
        Assert.IsTrue(result.Contains("value1"));
        Assert.IsTrue(result.Contains("field2"));
        Assert.IsTrue(result.Contains("field3"));
        Assert.IsTrue(result.Contains("field4"));
        Assert.IsTrue(result.Contains("file"));
        Assert.IsTrue(result.Contains("test.txt"));
    }

    [TestMethod]
    public void FormData_CanBeUsedInRequest()
    {
        // Arrange
        var formData = new FormData()
            .Append("name", "John")
            .Append("age", "30");

        // Act
        var request = new Request
        {
            RequestUri = new Uri("https://example.com/upload"),
            Method = HttpMethod.Post,
            Body = formData
        };

        // Assert
        Assert.IsNotNull(request.Content);
        Assert.IsInstanceOfType(request.Content, typeof(MultipartFormDataContent));
    }
}
