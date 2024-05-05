namespace Tapo.Application.Dto;

public class DeviceLoginResponse : TapoResponse<DeviceLoginResult>{ }

public class DeviceLoginResult
{
    public string Token { get; set; } = null!;
}
