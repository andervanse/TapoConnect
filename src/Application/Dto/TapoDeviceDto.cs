
namespace Tapo.Application.Dto;
public class TapoDeviceDto
{
    public string DeviceType { get; set; } = null!;

    public int Role { get; set; }

    public string FwVer { get; set; } = null!;

    public string AppServerUrl { get; set; } = null!;

    public string DeviceRegion { get; set; } = null!;

    public string DeviceId { get; set; } = null!;

    public string DeviceName { get; set; } = null!;

    public string DeviceHwVer { get; set; } = null!;

    public string Alias { get; set; } = null!;

    public string DeviceMac { get; set; } = null!;

    public string OemId { get; set; } = null!;

    public string DeviceModel { get; set; } = null!;

    public string HwId { get; set; } = null!;

    public string FwId { get; set; } = null!;

    public bool IsSameRegion { get; set; }

    public int Status { get; set; }
}