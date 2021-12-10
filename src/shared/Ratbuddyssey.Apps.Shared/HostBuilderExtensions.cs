using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ratbuddyssey.Converters;
using Ratbuddyssey.ViewModels;
using Ratbuddyssey.Views;

#nullable enable

namespace Ratbuddyssey.Initialization;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddViews(this IHostBuilder hostBuilder)
    {
        hostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));

        _ = hostBuilder.ConfigureServices(static services =>
        {
            _ = services
                .AddSingleton<IViewFor<MainViewModel>, MainView>()
                .AddSingleton<IViewFor<FileViewModel>, FileView>()
#if HAS_WPF
                .AddSingleton<IViewFor<EthernetViewModel>, EthernetView>()
#else
#endif
                ;
        });

        return hostBuilder;
    }

    public static IHostBuilder AddConverters(this IHostBuilder hostBuilder)
    {
        hostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));

        _ = hostBuilder.ConfigureServices(static services =>
        {
            _ = services
#if HAS_WPF
#else
                .AddSingleton<IBindingTypeConverter, IntegerTextTypeConverter>()
#endif
                ;
        });

        return hostBuilder;
    }

    public static IHostBuilder AddPlatformSpecificLoggers(this IHostBuilder hostBuilder)
    {
        hostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));

        _ = hostBuilder.ConfigureLogging(static builder =>
        {
#if WINDOWS_UWP
            // Remove loggers incompatible with UWP.
            foreach (var logger in builder
                .Services
                .Where(service => service.ImplementationType == typeof(Microsoft.Extensions.Logging.EventLog.EventLogLoggerProvider))
                .ToList())
            {
                _ = builder.Services.Remove(logger);
            }
#endif

            builder
#if __WASM__
                .ClearProviders()
#else
                .AddConsole()
#endif
                ;
        });

        return hostBuilder;
    }
}
