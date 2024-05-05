namespace Tapo.Application.Dto;

public class DeviceHandshakeReponse : TapoResponse<DeviceHandshakeResult>
{
}

public class DeviceHandshakeResult
{
    public string Key { get; set; } = null!;
}
