#nullable enable

using System.Runtime.CompilerServices;
using RayTech.RayLog.MEL;

namespace Microsoft.Extensions.Logging;

public static partial class LoggerExtensions
{

    public static void LogTrace(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingTraceInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogTrace(template, arguments);
        }
    }

    public static void LogTrace(this ILogger logger, EventId eventId, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingTraceInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogTrace(eventId, template, arguments);
        }
    }

    public static void LogTrace(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingTraceInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogTrace(exception, template, arguments);
        }
    }

    public static void LogTrace(this ILogger logger, EventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingTraceInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogTrace(eventId, exception, template, arguments);
        }
    }

    public static void LogDebug(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingDebugInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogDebug(template, arguments);
        }
    }

    public static void LogDebug(this ILogger logger, EventId eventId, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingDebugInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogDebug(eventId, template, arguments);
        }
    }

    public static void LogDebug(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingDebugInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogDebug(exception, template, arguments);
        }
    }

    public static void LogDebug(this ILogger logger, EventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingDebugInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogDebug(eventId, exception, template, arguments);
        }
    }

    public static void LogInformation(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingInformationInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogInformation(template, arguments);
        }
    }

    public static void LogInformation(this ILogger logger, EventId eventId, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingInformationInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogInformation(eventId, template, arguments);
        }
    }

    public static void LogInformation(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingInformationInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogInformation(exception, template, arguments);
        }
    }

    public static void LogInformation(this ILogger logger, EventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingInformationInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogInformation(eventId, exception, template, arguments);
        }
    }

    public static void LogWarning(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingWarningInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Warning))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogWarning(template, arguments);
        }
    }

    public static void LogWarning(this ILogger logger, EventId eventId, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingWarningInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Warning))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogWarning(eventId, template, arguments);
        }
    }

    public static void LogWarning(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingWarningInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Warning))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogWarning(exception, template, arguments);
        }
    }

    public static void LogWarning(this ILogger logger, EventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingWarningInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Warning))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogWarning(eventId, exception, template, arguments);
        }
    }

    public static void LogError(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingErrorInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Error))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogError(template, arguments);
        }
    }

    public static void LogError(this ILogger logger, EventId eventId, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingErrorInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Error))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogError(eventId, template, arguments);
        }
    }

    public static void LogError(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingErrorInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Error))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogError(exception, template, arguments);
        }
    }

    public static void LogError(this ILogger logger, EventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingErrorInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Error))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogError(eventId, exception, template, arguments);
        }
    }

    public static void LogCritical(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingCriticalInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Critical))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogCritical(template, arguments);
        }
    }

    public static void LogCritical(this ILogger logger, EventId eventId, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingCriticalInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Critical))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogCritical(eventId, template, arguments);
        }
    }

    public static void LogCritical(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingCriticalInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Critical))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogCritical(exception, template, arguments);
        }
    }

    public static void LogCritical(this ILogger logger, EventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref StructuredLoggingCriticalInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(LogLevel.Critical))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.LogCritical(eventId, exception, template, arguments);
        }
    }
}