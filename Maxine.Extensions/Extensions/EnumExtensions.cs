using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Maxine.Extensions;

public static class EnumExtensions
{
    [RequiresUnreferencedCode("Uses reflection to get enum field and custom attributes which may be trimmed")]
    public static string GetDescription<T>(this T anEnum) where T : Enum
    {
        var fi = typeof(T).GetField(anEnum.ToString());

        if (fi == null)
        {
            throw new InvalidOperationException("enum FieldInfo is null?");
        }

        return fi.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute attr
            ? attr.Description
            : anEnum.ToString();
    }
}