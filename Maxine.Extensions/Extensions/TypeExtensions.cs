using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Maxine.Extensions;

public static class TypeExtensions
{
    /*
     * https://github.com/koculu/ZoneTree/blob/main/LICENSE
     * 
     * MIT License
     * 
     * Copyright (c) 2022 Ahmed Yasin Koculu
     * 
     * Permission is hereby granted, free of charge, to any person obtaining a copy
     * of this software and associated documentation files (the "Software"), to deal
     * in the Software without restriction, including without limitation the rights
     * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
     * copies of the Software, and to permit persons to whom the Software is
     * furnished to do so, subject to the following conditions:
     * 
     * The above copyright notice and this permission notice shall be included in all
     * copies or substantial portions of the Software.
     * 
     * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
     * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
     * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
     * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
     * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
     * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
     * SOFTWARE.
     */
    [RequiresUnreferencedCode("Uses reflection to access type metadata which may be trimmed")]
    public static string HumanizeTypeName(this Type type)
    {
        if (type is { IsGenericType: false, FullName: {} fullName })
            return fullName;

        var builder = new ValueStringBuilder(stackalloc char[type.FullName?.Length ?? 32]);

        try
        {
            HumanizeInner(type, ref builder);

            return builder.ToString();

            static void HumanizeInner(Type type, ref ValueStringBuilder builder)
            {
                if (type.Namespace != null)
                    builder.Append($"{type.Namespace}.");

                bool generic;
                var typeName = type.Name;
                if (typeName.IndexOf('`') is var idx and > 0)
                {
                    builder.Append(typeName.AsSpan(..idx));
                    generic = true;
                }
                else
                {
                    builder.Append(typeName);
                    generic = false;
                }

                if (generic)
                {
                    builder.Append('<');
                    var genericArguments = type.GetGenericArguments();
                    var len = genericArguments.Length;
                    for (var i = 0; i < len; i++)
                    {
                        if (i > 0)
                        {
                            builder.Append(", ");
                        }

                        if (genericArguments[i] is { IsGenericType: false, FullName: { } fullName })
                        {
                            builder.Append(fullName);
                        }
                        else
                        {
                            HumanizeInner(genericArguments[i], ref builder);
                        }
                    }

                    builder.Append('>');
                }
            }
        }
        finally
        {
            builder.Dispose();
        }
    }
}