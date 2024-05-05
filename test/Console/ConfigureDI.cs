using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tapo.Application;

namespace Tapo.ConsoleApp;

public static class ConfigureDI
{

    public static IServiceCollection ConfigureDependencies(this IServiceCollection services, IConfigurationRoot? configuration)
    {
        var authConfig = new AuthenticationConfig();
        configuration?.Bind("Authentication", authConfig);
        
        services
        .AddSingleton(authConfig)
        .AddScoped<ITapoCloudClient, TapoCloudClient>()
        .AddScoped<ITapoDeviceClient, TapoDeviceClient>();

        services
            .AddHttpClient<ITapoCloudClient, TapoCloudClient>(client =>
            {
                client.BaseAddress = new Uri(authConfig.CloudUrl);
            });

        services
            .AddHttpClient<ITapoDeviceClient, TapoDeviceClient>(client =>
            {
                var device = authConfig?.Devices?.FirstOrDefault();
                client.BaseAddress = new Uri($"http://{device.IpAddress}");
            });

        return services;
    }
}