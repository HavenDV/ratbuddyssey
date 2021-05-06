using System;
using Audyssey.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Audyssey.Initialization
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            services
                .AddSingleton<MainViewModel>()
                .AddSingleton<IScreen, MainViewModel>(provider => provider.GetRequiredService<MainViewModel>())

                .AddSingleton<FileViewModel>()
                .AddSingleton<EthernetViewModel>()
                ;

            return services;
        }
    }
}
