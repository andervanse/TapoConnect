using Tapo.Application;
using Serilog;
using Tapo.Application.Protocol;

namespace Tapo.WebAPI;

public static class ConfigureDI
{

    public static IServiceCollection ConfigureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
       services.AddOptions<AuthenticationConfig>()
            .Bind(configuration.GetSection(AuthenticationConfig.Authentication));

        var authConfig = new AuthenticationConfig();
        configuration?.Bind(AuthenticationConfig.Authentication, authConfig);

        services.AddLogging(builder =>
        {
            var logger = new LoggerConfiguration()
                                .WriteTo
                                .Console()
                                .MinimumLevel
                                .Debug()
                                .CreateLogger();

            builder.AddSerilog(logger);
            builder.AddConsole();
        });

        services            
            .AddHttpClient<ITapoCloudClient, TapoCloudClient>(nameof(TapoCloudClient), client =>
            {
                client.BaseAddress = new Uri(authConfig.CloudUrl);
            });

        services
            .AddHttpClient<IDeviceProtocol, KlapDeviceClient>(nameof(KlapDeviceClient), client =>
            {
                bool? anyDevices = authConfig?.Devices?.Any();

                if (anyDevices.HasValue && anyDevices.Value)
                {
                    var device = authConfig?.Devices?.FirstOrDefault();
                    client.BaseAddress = new Uri($"http://{device?.IpAddress}");
                }
                else
                {
                    throw new Exception("Missing configuration [Device]");
                }
            });

        services.AddScoped<ITapoDeviceClient, TapoDeviceClient>();

        return services;
    }
}