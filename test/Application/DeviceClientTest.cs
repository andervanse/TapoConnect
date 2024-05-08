using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tapo.Application.Protocol;


namespace Tapo.Application.Test;


[TestClass]
public class DeviceClientTest : BaseTest
{
    private ITapoDeviceClient _client;
    private TapoDeviceKey _deviceKey = null!;
    private HttpClient _httpClient;

    [TestInitialize]
    public async Task TestInitialize()
    {
        var device = AuthenticationConfig.Devices.FirstOrDefault();

        _httpClient = new HttpClient {
            BaseAddress = new Uri($"http://{device.IpAddress}")
        };
        
        _client = new TapoDeviceClient (
            DeviceLogger,
            new KlapDeviceClient(DeviceLogger, _httpClient));

        _deviceKey = await _client.LoginByIpAsync(device.IpAddress, AuthenticationConfig.Email, AuthenticationConfig.Password);
        DeviceLogger.LogDebug("login token: {deviceKey}", _deviceKey);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _httpClient.Dispose();
    }

    

    [TestMethod]
    public async Task GetDeviceInfoAsync()
    {
        var deviceInfo = await _client.GetDeviceInfoAsync(_deviceKey);

        Console.WriteLine("info: {0}", deviceInfo.Nickname);
        Assert.IsNotNull(deviceInfo);
    }


    [TestMethod]
    public async Task SetPowerAsync()
    {
        await _client.SetPowerAsync(_deviceKey, true);
    }


    [TestMethod]
    public async Task SetBrightnessAsync()
    {
        await _client.SetBrightnessAsync(_deviceKey, 1);
    }


    [TestMethod]
    public async Task SetColorAsync()
    {
        await _client.SetColorAsync(_deviceKey, TapoColor.FromRgb(10, 0, 0));
    }


    [TestMethod]
    public async Task SetColorTempAsync()
    {
        await _client.SetColorAsync(_deviceKey, TapoColor.FromTemperature(4500, 10));
    }


    [TestMethod]
    public async Task SetStateAsync()
    {
        await _client.SetStateAsync(_deviceKey, new TapoSetBulbState(
            color: TapoColor.FromTemperature(4500),
            deviceOn: true));
    }
}