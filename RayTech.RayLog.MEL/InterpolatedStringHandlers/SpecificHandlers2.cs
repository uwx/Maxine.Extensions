#nullable enable

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace RayTech.RayLog.MEL;


[InterpolatedStringHandler]
public ref struct StructuredLoggingTraceInterpolatedStringHandler
{
    private StructuredLoggingInterpolatedStringHandler _handler;

    public StructuredLoggingTraceInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool isEnabled)
    {
        _handler = new StructuredLoggingInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Trace, out isEnabled);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string s) => _handler.AppendLiteral(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, format, name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetTemplateAndArguments(out string template, out object?[] arguments) => _handler.GetTemplateAndArguments(out template, out arguments);
}

[InterpolatedStringHandler]
public ref struct StructuredLoggingDebugInterpolatedStringHandler
{
    private StructuredLoggingInterpolatedStringHandler _handler;

    public StructuredLoggingDebugInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool isEnabled)
    {
        _handler = new StructuredLoggingInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Debug, out isEnabled);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string s) => _handler.AppendLiteral(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, format, name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetTemplateAndArguments(out string template, out object?[] arguments) => _handler.GetTemplateAndArguments(out template, out arguments);
}

[InterpolatedStringHandler]
public ref struct StructuredLoggingInformationInterpolatedStringHandler
{
    private StructuredLoggingInterpolatedStringHandler _handler;

    public StructuredLoggingInformationInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool isEnabled)
    {
        _handler = new StructuredLoggingInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Information, out isEnabled);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string s) => _handler.AppendLiteral(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, format, name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetTemplateAndArguments(out string template, out object?[] arguments) => _handler.GetTemplateAndArguments(out template, out arguments);
}

[InterpolatedStringHandler]
public ref struct StructuredLoggingWarningInterpolatedStringHandler
{
    private StructuredLoggingInterpolatedStringHandler _handler;

    public StructuredLoggingWarningInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool isEnabled)
    {
        _handler = new StructuredLoggingInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Warning, out isEnabled);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string s) => _handler.AppendLiteral(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, format, name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetTemplateAndArguments(out string template, out object?[] arguments) => _handler.GetTemplateAndArguments(out template, out arguments);
}

[InterpolatedStringHandler]
public ref struct StructuredLoggingErrorInterpolatedStringHandler
{
    private StructuredLoggingInterpolatedStringHandler _handler;

    public StructuredLoggingErrorInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool isEnabled)
    {
        _handler = new StructuredLoggingInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Error, out isEnabled);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string s) => _handler.AppendLiteral(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, format, name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetTemplateAndArguments(out string template, out object?[] arguments) => _handler.GetTemplateAndArguments(out template, out arguments);
}

[InterpolatedStringHandler]
public ref struct StructuredLoggingCriticalInterpolatedStringHandler
{
    private StructuredLoggingInterpolatedStringHandler _handler;

    public StructuredLoggingCriticalInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool isEnabled)
    {
        _handler = new StructuredLoggingInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Critical, out isEnabled);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string s) => _handler.AppendLiteral(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression(nameof(value))] string name = "") => _handler.AppendFormatted(value, format, name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetTemplateAndArguments(out string template, out object?[] arguments) => _handler.GetTemplateAndArguments(out template, out arguments);
}
