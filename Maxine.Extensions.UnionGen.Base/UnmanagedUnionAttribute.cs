using System;

namespace Maxine.Extensions.UnionGen;

[AttributeUsage(AttributeTargets.Struct)]
public class UnmanagedUnionAttribute(params Type[] types) : Attribute
{
    public Type[] Types { get; } = types;
    public bool Nullable { get; set; }
}