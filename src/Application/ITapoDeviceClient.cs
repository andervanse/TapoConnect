﻿using Tapo.Application.Dto;
using Tapo.Application.Protocol;

namespace Tapo.Application;

public interface ITapoDeviceClient
{
    TapoDeviceProtocol Protocol { get; }
    Task<TapoDeviceKey> LoginByIpAsync(string ipAddress, string email, string password);
    Task<DeviceGetEnergyUsageResult> GetEnergyUsageAsync(TapoDeviceKey deviceKey);
    Task<DeviceGetInfoResult> GetDeviceInfoAsync(TapoDeviceKey deviceKey);
    Task SetPowerAsync(TapoDeviceKey deviceKey, bool on);
    Task SetBrightnessAsync(TapoDeviceKey deviceKey, int brightness);
    Task SetColorAsync(TapoDeviceKey deviceKey, TapoColor color);
    Task SetStateAsync<TState>(TapoDeviceKey deviceKey, TState state)
       where TState : TapoSetDeviceState;
}
