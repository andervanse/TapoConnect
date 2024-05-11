using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tapo.Application;

namespace Tapo.ConsoleApp;

internal class Program
{
    private const string ENV_FILE = "appsettings.Development.json";

    static async Task Main()
    {
        if (!File.Exists(ENV_FILE))
        {
            throw new FileNotFoundException(ENV_FILE);
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(ENV_FILE)
            .Build();

        var authConfig = new AuthenticationConfig();
        configuration.Bind("Authentication", authConfig);

        var serviceCollection = new ServiceCollection();

        var serviceProvider = serviceCollection
            .ConfigureDependencies(configuration)
            .BuildServiceProvider();

        var cloudClient = serviceProvider.GetService<ITapoCloudClient>();

        Console.WriteLine("************* BEGIN *************");
        Console.WriteLine("Logging in to the cloud");
        var loginResult = await cloudClient.LoginAsync(authConfig.Email, authConfig.Password);
        Console.WriteLine("Account id.: {0}", loginResult.AccountId);
        Console.WriteLine("Nickname...: {0}", loginResult.Nickname);
        Console.WriteLine("************ DEVICES ************");
        var devicesResponse = await cloudClient.ListDevicesAsync(loginResult.Token);

        bool firstRow = true;
        foreach(var device in devicesResponse.DeviceList)
        {
            if (!firstRow)
                Console.WriteLine("----------------------------------");
            Console.WriteLine("Alias.: {0}", device.Alias);
            Console.WriteLine("Model.: {0}", device.DeviceModel);
            Console.WriteLine("Mac...: {0}", device.DeviceMac);
            firstRow = false;
        }

        Console.WriteLine("*************  END  *************");
    }
}