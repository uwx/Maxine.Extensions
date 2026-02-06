#nullable disable

internal class NativeTypeNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}