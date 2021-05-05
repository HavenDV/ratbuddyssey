using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Audyssey.Initialization
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddViews(this IHostBuilder hostBuilder)
        {
            hostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));

            hostBuilder.ConfigureServices(static services =>
            {
                //services
                //    .AddTransient<IViewFor<MainViewModel>, MainView>()
                //    .AddTransient<IViewFor<LoginViewModel>, LoginView>()
                //    .AddTransient<IViewFor<NavigationViewModel>, NavigationView>()
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

            hostBuilder.ConfigureLogging(static builder =>
            {
                builder
                    .AddConsole();
            });

            return hostBuilder;
        }
    }
}
