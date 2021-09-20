using System;
using Ratbuddyssey.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ratbuddyssey.Converters;
using Ratbuddyssey.Views;
using ReactiveUI;

#nullable enable

namespace Ratbuddyssey.Initialization
{
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
                    .AddSingleton<IViewFor<EthernetViewModel>, EthernetView>()
#if WPF_APP
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
                _ = builder
                    .AddConsole();
            });

            return hostBuilder;
        }
    }
}
