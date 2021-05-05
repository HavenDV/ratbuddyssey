using System;
using Microsoft.Extensions.DependencyInjection;

namespace Audyssey.Initialization
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            //services
            //    .AddSingleton(_ =>
            //    {
            //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //        return new HttpClient();
            //    })
            //    ;

            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            //services
            //    .AddTransient<MainViewModel>()

            //    .AddSingleton<NavigationViewModel>()
            //    .AddSingleton<IScreen, NavigationViewModel>(provider => provider.GetRequiredService<NavigationViewModel>())
            //    ;

            return services;
        }
    }
}
