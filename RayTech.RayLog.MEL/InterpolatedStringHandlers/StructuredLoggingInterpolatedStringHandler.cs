using System.Buffers;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Poki.Tests")]

namespace RayTech.RayLog.MEL;

[InterpolatedStringHandler]
public ref struct StructuredLoggingInterpolatedStringHandler
{
    private static readonly char[] CharsToEscape = ['{', '}']; // escape these chars if they appear in a literal
    internal static readonly char[] CharsToRemove = ['{', ':', '}']; // remove these chars if they appear in the argument name
    
    private DefaultInterpolatedStringHandler _template;
    private ArgumentList _arguments;

    public StructuredLoggingInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, LogLevel logLevel, out bool isEnabled)
    {
        isEnabled = logger.IsEnabled(logLevel);
        if (isEnabled)
        {
            _template = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
            _arguments = new ArgumentList(formattedCount);
        }
        else
        {
            _template = default;
            _arguments = default;
        }
    }

    public void AppendLiteral(string s)
    {
        _template.AppendLiteral(s.IndexOfAny(CharsToEscape) != -1 ? EscapeProblematicChars(s) : s);
    }

    // input: input string
    // newIdx: first input.IndexOfAny(CharsToRemove)
    internal static string RemoveProblematicChars(string input, int nextIndex)
    {
        ReadOnlySpan<char> problematicChars = CharsToRemove;

        ReadOnlySpan<char> inputBuf = input;

        var inputLen = input.Length;

        char[]? arrayToReturnToPool = null;
        var outputBuf = inputLen < 1000
            ? stackalloc char[inputLen]
            : arrayToReturnToPool = ArrayPool<char>.Shared.Rent(inputLen);

        var written = 0; // length written to output
        while (!inputBuf.IsEmpty)
        {
            if (nextIndex == -1)
            {
                inputBuf.CopyTo(outputBuf[written..]); // copy the remaining data to the span
                written += inputBuf.Length;
                
                break;
            }

            inputBuf[..nextIndex].CopyTo(outputBuf[written..]); // copy all the data up until the problematic char
            written += nextIndex; // add how much data was written

            inputBuf = inputBuf[(nextIndex + 1)..]; // substr after the problematic char

            nextIndex = inputBuf.IndexOfAny(problematicChars);
        }

        if (outputBuf.IsEmpty)
        {
            return "noName";
        }

        var outputStr = new string(outputBuf[..written]);

        if (arrayToReturnToPool != null)
        {
            ArrayPool<char>.Shared.Return(arrayToReturnToPool, true);
        }

        return outputStr;
    }

    private static string EscapeProblematicChars(string s)
    {
        return s.Replace("{", "{{").Replace("}", "}}");
    }

    public void AppendFormatted<T>(T value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        _arguments.Add(value);
        _template.AppendLiteral("{");
        _template.AppendLiteral(name.IndexOfAny(CharsToRemove) is var firstIdx and not -1 ? RemoveProblematicChars(name, firstIdx) : name);
        _template.AppendLiteral("}");
    }

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        if (format is [ '@' or '$' ])
        {
            _arguments.Add(value);
            _template.AppendLiteral("{");
            _template.AppendLiteral(format);
            _template.AppendLiteral(name.IndexOfAny(CharsToRemove) is var firstIdx and not -1 ? RemoveProblematicChars(name, firstIdx) : name);
            _template.AppendLiteral("}");
        }
        else if (format is [ ':', .. ])
        {
            _arguments.Add(value);
            _template.AppendLiteral("{");
            _template.AppendLiteral(format[1..]);
            _template.AppendLiteral("}");
        }
        else
        {
            AppendFormatted(value, name);
        }
    }

    public void GetTemplateAndArguments(out string template, out object?[] arguments)
    {
        template = _template.ToStringAndClear();
        arguments  = _arguments.Arguments;
    }

    private struct ArgumentList
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        private int _index = 0;

        public readonly object?[] Arguments;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArgumentList(int formattedCount) => Arguments = new object?[formattedCount];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(object? value) => Arguments[_index++] = value;
    }
}