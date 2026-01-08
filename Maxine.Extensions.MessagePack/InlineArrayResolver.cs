using System.Runtime.CompilerServices;
using MessagePack;
using MessagePack.Formatters;

namespace Maxine.Extensions.MessagePack;

public sealed class InlineArrayResolver : IFormatterResolver
{
    /// <summary>
    /// The singleton instance that can be used.
    /// </summary>
    public static readonly InlineArrayResolver Instance = new();

    private InlineArrayResolver()
    {
    }

    public IMessagePackFormatter<T>? GetFormatter<T>()
    {
        return FormatterCache<T>.Formatter;
    }

    private static class FormatterCache<T>
    {
        public static readonly IMessagePackFormatter<T>? Formatter;

        static FormatterCache()
        {
            Formatter = (IMessagePackFormatter<T>?)Helper.GetFormatter(typeof(T));
        }
    }

    private static class Helper
    {
        public static object? GetFormatter(Type type)
        {
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(InlineArray2<>))
                    return CreateInstance(typeof(InlineArray2Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray3<>))
                    return CreateInstance(typeof(InlineArray3Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray4<>))
                    return CreateInstance(typeof(InlineArray4Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray5<>))
                    return CreateInstance(typeof(InlineArray5Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray6<>))
                    return CreateInstance(typeof(InlineArray6Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray7<>))
                    return CreateInstance(typeof(InlineArray7Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray8<>))
                    return CreateInstance(typeof(InlineArray8Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray9<>))
                    return CreateInstance(typeof(InlineArray9Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray10<>))
                    return CreateInstance(typeof(InlineArray10Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray11<>))
                    return CreateInstance(typeof(InlineArray11Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray12<>))
                    return CreateInstance(typeof(InlineArray12Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray13<>))
                    return CreateInstance(typeof(InlineArray13Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray14<>))
                    return CreateInstance(typeof(InlineArray14Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray15<>))
                    return CreateInstance(typeof(InlineArray15Formatter<>), type.GenericTypeArguments);
                if (genericType == typeof(InlineArray16<>))
                    return CreateInstance(typeof(InlineArray16Formatter<>), type.GenericTypeArguments);
            }
            
            return null;
        }

        private static object? CreateInstance(Type genericType, Type[] genericTypeArguments, params object?[] arguments)
        {
            return Activator.CreateInstance(genericType.MakeGenericType(genericTypeArguments), arguments);
        }
    }
}
