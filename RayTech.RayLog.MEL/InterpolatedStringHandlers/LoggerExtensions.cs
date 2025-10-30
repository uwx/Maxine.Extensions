using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace RayTech.RayLog.MEL;

[PublicAPI]
public static partial class LoggerExtensions
{
    public static void Log(this ILogger logger, LogLevel logLevel, [InterpolatedStringHandlerArgument(nameof(logger), nameof(logLevel))] ref StructuredLoggingInterpolatedStringHandler handler)
    {
        if (logger.IsEnabled(logLevel))
        {
            handler.GetTemplateAndArguments(out var template, out var arguments);
            logger.Log(logLevel, template, arguments);
        }
    }
}