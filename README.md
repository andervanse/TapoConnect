
# TapoConnect
Unofficial TP-Link Tapo smart device library for C#.

### Updates
* Ported to dotnet 8.0
* Linux friendly

Build clients to interact with Tapo Cloud or smart device.
```cs
TapoCloudClient cloudClient = new TapoCloudClient();
TapoDeviceClient deviceClient = new TapoDeviceClient();

//Specify order of login protocols to try.
TapoDeviceClient deviceClient = new TapoDeviceClient(new List<ITapoDeviceClient>
{
	new KlapDeviceClient(),
	new SecurePassthroughDeviceClient(),
});
```

Get devices from cloud.
```cs
CloudLoginResult cloudKey = await cloudClient.LoginAsync("<Username>", "<Password>");
CloudListDeviceResult deviceResult = await cloudClient.ListDevicesAsync(cloudKey.Token);
```

Select devices by type.
```cs
IEnumerable<TapoDeviceDto> devices = deviceResult.DeviceList
    .Where(x => x.DeviceType == TapoUtils.TapoBulbDeviceType);
```

Select device by alias.
```cs
TapoDeviceDto device = deviceResult.DeviceList.First(x => x.Alias == "<Device Name>")
```

Login to device by known IP address. 
```cs
TapoDeviceKey deviceKey = await deviceClient.LoginByIpAsync("<Username>", "<Password>", "<IpAddress>");
```

Login to device by known MAC address (finds the MAC address on local network through a Windows ARP request).
```cs
string ip = TapoUtils.GetIpAddressByMacAddress(device.DeviceMac);
TapoDeviceKey deviceKey = await deviceClient.LoginByIpAsync("<Username>", "<Password>", ip);
```

Get Device info.
```cs
DeviceGetInfoResult deviceInfo = await deviceClient.GetDeviceInfoAsync(deviceKey);
```

Set Device's power, brightness, color, and state.
```cs
await deviceClient.SetPowerAsync(deviceKey, true);

await deviceClient.SetBrightnessAsync(deviceKey, 80);

await deviceClient.SetColorAsync(deviceKey, TapoColor.FromHex("#e8d974"));
await deviceClient.SetColorAsync(deviceKey, TapoColor.FromHsl(200, 100, 100));
await deviceClient.SetColorAsync(deviceKey, TapoColor.FromRgb(100, 25, 32));
await deviceClient.SetColorAsync(deviceKey, TapoColor.FromTemperature(3500));

await deviceClient.SetStateAsync(deviceKey, new TapoSetBulbState(
    color: TapoColor.FromTemperature(3800, brightness: 1),
    deviceOn: true));
```

### Disclaimer
This is an unofficial SDK that has no affiliation with TP-Link.
TP-Link and all respective product names are copyright TP-Link Technologies Co, Ltd. and/or its subsidiaries and affiliates.

### Credits
Credit to this API go to:
* Forked from [cwakefie27/TapoConnect](https://github.com/cwakefie27/TapoConnect)

Many thanks to [cwakefie27](https://github.com/cwakefie27) for this fork.