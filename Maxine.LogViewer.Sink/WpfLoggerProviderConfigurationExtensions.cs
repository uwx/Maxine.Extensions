using System;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Maxine.LogViewer.Sink;

public static class WpfLoggerProviderConfigurationExtensions
{
    public static ILoggingBuilder AddWpfControl(
        this ILoggingBuilder builder,
        IWpfLogBroker logBroker,
        DispatcherPriority dispatcherQueuePriority = DispatcherPriority.Background,
        TimeSpan? loggingInterval = null
    )
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, WpfLoggerProvider>(
            _ => new WpfLoggerProvider(
                logBroker,
                dispatcherQueuePriority,
                loggingInterval ?? TimeSpan.FromMilliseconds(500)
            )
        ));

        return builder;
    }
}
