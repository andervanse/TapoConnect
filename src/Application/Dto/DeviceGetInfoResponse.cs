using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Tapo.Application.Dto;

public class DeviceGetInfoResponse : TapoResponse<DeviceGetInfoResult>
{
}

public class DeviceGetInfoResult
{
    public string DeviceId { get; set; } = null!;

    public string HwVersion { get; set; } = null!;

    public string FwVersion { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string Mac { get; set; } = null!;

    public string HwId { get; set; } = null!;

    public string FwId { get; set; } = null!;

    public string OemId { get; set; } = null!;

    public List<int> ColorTemperatureRange { get; set; } = null!;

    public bool Overheated { get; set; }

    public string IpAddress { get; set; } = null!;

    public int TimeDiff { get; set; }

    public string Ssid { get; set; } = null!;

    public int Rssi { get; set; }

    public int SignalLevel { get; set; }

    public string? AutoOffStatus { get; set; }

    public int AutoOffRemainTime { get; set; }

    public int Latitude { get; set; }

    public int Longitude { get; set; }

    public string Lang { get; set; } = null!;

    public string Avatar { get; set; } = null!;

    public string Region { get; set; } = null!;

    public string Specs { get; set; } = null!;

    public string Nickname { get; set; } = null!;

    public bool HasSetLocationInfo { get; set; }

    public float OnTime { get; set; }

    public bool DeviceOn { get; set; }

    public int Brightness { get; set; }

    public int Hue { get; set; }

    public int Saturation { get; set; }

    public int ColorTemperature { get; set; }

    public bool DynamicLightEffectEnable { get; set; }

    public DeviceGetInfoDefaultStateDto DefaultState { get; set; } = null!;

    public string? PowerProtectionStatus { get; set; }

    public string? OvercurrentStatus { get; set; }

    public int ErrorCode { get; set; }
}

public class DeviceGetEnergyUsageResponse : TapoResponse<DeviceGetEnergyUsageResult>
{
}
public class DeviceGetEnergyUsageResult
{
    public float CurrentPower { get; set; }

    public float TodayRuntime { get; set; }

    public float MonthRuntime { get; set; }

    public float TodayEnergy { get; set; }

    public float MonthEnergy { get; set; }

    public DateTime LocalTime { get; set; }

    public float[]? ElectricityCharge { get; set; }

    public int ErrorCode { get; set; }
}

public class DeviceGetInfoDefaultStateDto
{
    public string Type { get; set; } = null!;

    public JsonObject State { get; set; } = null!;
}
