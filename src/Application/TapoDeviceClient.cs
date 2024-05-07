using Microsoft.Extensions.Logging;
using Tapo.Application.Dto;

namespace Tapo.Application;

public class TapoDeviceClient : ITapoDeviceClient
{
    private readonly ILogger<TapoDeviceClient> _logger;
    private readonly IDeviceProtocol _deviceProtocol;

    public TapoDeviceClient(
        ILogger<TapoDeviceClient> logger,
        IDeviceProtocol deviceProtocol)
    {
        _logger = logger;
        _deviceProtocol = deviceProtocol;
    }

    public async Task<TapoDeviceKey> LoginByIpAsync(string ipAddress, string username, string password)
    {
        ArgumentNullException.ThrowIfNull(nameof(ipAddress));
        ArgumentNullException.ThrowIfNull(nameof(username));
        ArgumentNullException.ThrowIfNull(nameof(password));
        return await _deviceProtocol.LoginByIpAsync(ipAddress, username, password);
    }

    public async Task<DeviceGetInfoResult> GetDeviceInfoAsync(TapoDeviceKey deviceKey)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));
        return await _deviceProtocol.GetDeviceInfoAsync(deviceKey);
    }

    public async Task<DeviceGetEnergyUsageResult> GetEnergyUsageAsync(TapoDeviceKey deviceKey)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));
        return await _deviceProtocol.GetEnergyUsageAsync(deviceKey);
    }

    public async Task SetPowerAsync(TapoDeviceKey deviceKey, bool on)
    {
        await _deviceProtocol.SetPowerAsync(deviceKey, on);
    }

    public async Task SetBrightnessAsync(TapoDeviceKey deviceKey, int brightness)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));
        await _deviceProtocol.SetBrightnessAsync(deviceKey, brightness);
    }

    public async Task SetColorAsync(TapoDeviceKey deviceKey, TapoColor color)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey)); 
        ArgumentNullException.ThrowIfNull(nameof(color));
        await _deviceProtocol.SetColorAsync(deviceKey, color);
    }

    public async Task SetStateAsync<TState>(TapoDeviceKey deviceKey, TState state)
       where TState : TapoSetDeviceState
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));
        ArgumentNullException.ThrowIfNull(nameof(state));
        await _deviceProtocol.SetStateAsync(deviceKey, state);
    }
}
