using System.ComponentModel;
using System.Reflection;

namespace Maxine.Extensions;

public static class EnumExtensions
{
    
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