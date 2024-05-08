using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Tapo.Application;
using Tapo.Application.Protocol;
using Serilog;

namespace Tapo.WebAPI;

public static class Configuration
{

    public static void ConfigureApp(this WebApplication webApp, IConfiguration configuration)
    {
        var routePrefix = configuration["RoutePrefix"];
        webApp.UsePathBase($"/{routePrefix}");

        webApp.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        if (webApp.Environment.IsDevelopment())
        {
            webApp.UseSwagger();
            webApp.UseSwaggerUI();
            webApp.UseDeveloperExceptionPage();
        }

        webApp.UseHttpsRedirection();
    }

    public static IServiceCollection ConfigureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "TAPO API", Version = "v1" });          
        });

        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
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