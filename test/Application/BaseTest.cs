using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;


namespace Tapo.Application.Test;

public class BaseTest : IDisposable
{
    public ILogger<TapoCloudClient> CloudLogger { get; private set; }

    public ILogger<TapoDeviceClient> DeviceLogger { get; private set; }

    public AuthenticationConfig AuthenticationConfig { get; private set; }

    private const string ENV_SETTINGS_FILE = "appsettings.Development.json";

    public BaseTest()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        
        if (!File.Exists(ENV_SETTINGS_FILE))
        {
            throw new FileNotFoundException(ENV_SETTINGS_FILE);
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(ENV_SETTINGS_FILE)
            .Build();

        AuthenticationConfig = new AuthenticationConfig();
        configuration.Bind(AuthenticationConfig.Authentication, AuthenticationConfig);

        
        using var loggerFactory = LoggerFactory.Create(builder =>
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

        CloudLogger = loggerFactory.CreateLogger<TapoCloudClient>();
        DeviceLogger = loggerFactory.CreateLogger<TapoDeviceClient>();
    }


    public void Dispose()
    {        
    }
}