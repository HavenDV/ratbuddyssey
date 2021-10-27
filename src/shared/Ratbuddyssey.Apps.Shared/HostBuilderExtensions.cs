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
#if WPF_APP
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
                .AddSingleton<IBindingTypeConverter, TypeToIconConverter>()
#if WPF_APP
#else
                .AddSingleton<IBindingTypeConverter, TypeToSymbolIconConverter>()
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

    /// <summary>
    /// Uno WebAssembly does not support FileWatchers.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder RemoveFileWatchers(this IHostBuilder hostBuilder)
    {
        hostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));

        hostBuilder.ConfigureAppConfiguration((context, builder) =>
        {
            foreach (var source in builder.Sources
                .Where(source => source is JsonConfigurationSource)
                .ToArray())
            {
                builder.Sources.Remove(source);
            }

            builder
                .AddJsonFile("appsettings.json", true, false)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, false);
        });

        return hostBuilder;
    }
}
