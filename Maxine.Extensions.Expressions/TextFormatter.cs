//
// TextFormatter.cs
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

namespace Maxine.Extensions.Expressions;

public class TextFormatter : IFormatter
{
    private readonly TextWriter _writer;
    private int _indent;

    private bool _writeIndent;

    public TextFormatter(TextWriter writer)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public void Write(string str)
    {
        WriteIndent();
        _writer.Write(str);
        _writeIndent = false;
    }

    public void WriteLine()
    {
        _writer.WriteLine();
        _writeIndent = true;
    }

    public void WriteSpace()
    {
        Write(" ");
    }

    public void WriteToken(string token)
    {
        Write(token);
    }

    public void WriteKeyword(string keyword)
    {
        Write(keyword);
    }

    public void WriteLiteral(string literal)
    {
        Write(literal);
    }

    public void WriteReference(string value, object reference)
    {
        Write(value);
    }

    public void WriteIdentifier(string value, object identifier)
    {
        Write(value);
    }

    public void Indent()
    {
        _indent++;
    }

    public void Dedent()
    {
        _indent--;
    }

    private void WriteIndent()
    {
        if (!_writeIndent)
            return;

        for (var i = 0; i < _indent; i++)
            _writer.Write("\t");
    }
}