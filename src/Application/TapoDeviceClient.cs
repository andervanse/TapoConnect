using Microsoft.Extensions.Logging;
using Tapo.Application.Dto;
using Tapo.Application.Protocol;

namespace Tapo.Application;

public class TapoDeviceClient : ITapoDeviceClient
{
    private readonly ILogger<TapoDeviceClient> _logger;
    private readonly ITapoDeviceClient _deviceClient;

    public TapoDeviceProtocol Protocol => TapoDeviceProtocol.Multi;

    public TapoDeviceClient(
        ILogger<TapoDeviceClient> logger,
        ITapoDeviceClient deviceClient)
    {
        _logger = logger;
        _deviceClient = deviceClient;
    }

    public async Task<TapoDeviceKey> LoginByIpAsync(string ipAddress, string username, string password)
    {
        ArgumentNullException.ThrowIfNull(nameof(ipAddress));
        ArgumentNullException.ThrowIfNull(nameof(username));
        ArgumentNullException.ThrowIfNull(nameof(password));
        return await _deviceClient.LoginByIpAsync(ipAddress, username, password);
    }

    public Task<DeviceGetInfoResult> GetDeviceInfoAsync(TapoDeviceKey deviceKey)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));
        return _deviceClient.GetDeviceInfoAsync(deviceKey);
    }

    public Task<DeviceGetEnergyUsageResult> GetEnergyUsageAsync(TapoDeviceKey deviceKey)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));
        return _deviceClient.GetEnergyUsageAsync(deviceKey);
    }

    public Task SetPowerAsync(TapoDeviceKey deviceKey, bool on)
    {
        return _deviceClient.SetPowerAsync(deviceKey, on);
    }

    public Task SetBrightnessAsync(TapoDeviceKey deviceKey, int brightness)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));
        return _deviceClient.SetBrightnessAsync(deviceKey, brightness);
    }

    public Task SetColorAsync(TapoDeviceKey deviceKey, TapoColor color)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey)); 
        ArgumentNullException.ThrowIfNull(nameof(color));
        return _deviceClient.SetColorAsync(deviceKey, color);
    }

    public Task SetStateAsync<TState>(TapoDeviceKey deviceKey, TState state)
       where TState : TapoSetDeviceState
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));
        ArgumentNullException.ThrowIfNull(nameof(state));
        return _deviceClient.SetStateAsync(deviceKey, state);
    }
}
