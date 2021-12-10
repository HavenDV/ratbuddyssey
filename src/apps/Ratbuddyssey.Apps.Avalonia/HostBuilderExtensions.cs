using Microsoft.Extensions.Hosting;

#nullable enable

namespace Ratbuddyssey.Initialization;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddConverters(this IHostBuilder hostBuilder)
    {
        hostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));

        _ = hostBuilder.ConfigureServices(static services =>
        {
            _ = services;
        });

        return hostBuilder;
    }
}
