//
// CSharp.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2010 Novell, Inc. (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#nullable disable

using System.Linq.Expressions;
using FastExpressionCompiler;
using Microsoft.Extensions.Logging;
using RayTech.RayLog.MEL;

namespace Poki.Shared.HSNXT.Linq.Expressions;

public static class CSharp
{
    public static string ToCSharpCode(this Expression self)
    {
        if (self == null)
            throw new ArgumentNullException(nameof(self));

        return ToCode(writer => writer.Write(self));
    }

    public static string ToCSharpCode(this LambdaExpression self)
    {
        if (self == null)
            throw new ArgumentNullException(nameof(self));

        return ToCode(writer => writer.Write(self));
    }

    private static string ToCode(Action<CSharpWriter> writer)
    {
        var str = new StringWriter();
        var csharp = new CSharpWriter(new TextFormatter(str));

        writer(csharp);

        return str.ToString();
    }

    public static T PrintCodeIfDebug<T>(this T expression) where T : LambdaExpression
    {
#if DEBUG
        RayLogConsole.Log(LogLevel.Debug, expression.ToCSharpString());
#endif
        return expression;
    }
}