using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ratbuddyssey.ViewModels;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Ratbuddyssey.Initialization
{
    public static class HostBuilder
    {
        public static IHostBuilder Create()
        {
            RxApp.DefaultExceptionHandler = new DefaultExceptionHandler();

            return Host
              .CreateDefaultBuilder()
              .ConfigureServices(ConfigureServices)
              .ConfigureLogging(ConfigureLogging)
              .AddViewModels();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.UseMicrosoftDependencyResolver();
            
            var resolver = Locator.CurrentMutable;
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
        }

        private static void ConfigureLogging(ILoggingBuilder builder)
        {
            builder
                .AddSplat()
#if DEBUG
                .SetMinimumLevel(LogLevel.Debug)
#else
                .SetMinimumLevel(LogLevel.Information)
#endif
                ;
        }

        public static IHostBuilder AddViewModels(this IHostBuilder builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices(static services =>
            {
                services
                    .AddSingleton<MainViewModel>()
                    .AddSingleton<IScreen, MainViewModel>(provider => provider.GetRequiredService<MainViewModel>())

                    .AddSingleton<FileViewModel>()
                    .AddSingleton<EthernetViewModel>()
                    ;
            });

            return builder;
        }
    }
}
