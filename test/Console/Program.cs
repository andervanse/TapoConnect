using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tapo.Application;

namespace Tapo.ConsoleApp;

internal class Program
{
    private const string ENV_FILE = "appsettings.Development.json";

    static async Task Main(string[] args)
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
        var result = await cloudClient.LoginAsync(authConfig.Email, authConfig.Password);
        Console.WriteLine("Account id.: {0}", result.AccountId);
        Console.WriteLine("Nickname...: {0}", result.Nickname);
        Console.WriteLine("*************  END  *************");
    }
}