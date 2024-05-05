using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace Tapo.Application.Test;


[TestClass]
public class CloudClientTest : BaseTest
{

    [TestMethod]
    public async Task CloudLoginAsync()
    {
        using var httpClient = new HttpClient {
            BaseAddress = new Uri(AuthenticationConfig.CloudUrl)
        };

        var client = new TapoCloudClient(httpClient, base.CloudLogger);
        var login = await client.LoginAsync(AuthenticationConfig.Email, AuthenticationConfig.Password);
        Assert.IsNotNull(login.Token);
    }


    [TestMethod]
    public async Task CloudRefreshLoginAsync()
    {
        using var httpClient = new HttpClient {
            BaseAddress = new Uri(AuthenticationConfig.CloudUrl)
        };

        var client = new TapoCloudClient(httpClient, base.CloudLogger);
        var login = await client.LoginAsync(AuthenticationConfig.Email, AuthenticationConfig.Password, true);

        Assert.IsNotNull(login.Token);
        Assert.IsNotNull(login.RefreshToken);

        var refreshLogin = await client.RefreshLoginAsync(login.RefreshToken);

        Assert.IsNotNull(refreshLogin.Token);
    }


    [TestMethod]
    public async Task ListDevicesAsync()
    {
        using var httpClient = new HttpClient {
            BaseAddress = new Uri(AuthenticationConfig.CloudUrl)
        };

        var client = new TapoCloudClient(httpClient, base.CloudLogger);
        var login = await client.LoginAsync(AuthenticationConfig.Email, AuthenticationConfig.Password);
        var response = await client.ListDevicesAsync(login.Token);

        Assert.IsNotNull(response.DeviceList);
    }
}