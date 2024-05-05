namespace Tapo.Application.Dto;

public class CloudListDeviceReponse : TapoResponse<CloudListDeviceResult>
{
}

public class CloudListDeviceResult
{
    public List<TapoDeviceDto> DeviceList { get; set; } = null!;
}
