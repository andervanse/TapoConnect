namespace Tapo.Application.Dto;

public class DeviceSecurePassthroughReponse : TapoResponse<DeviceSecurePassthroughResult> { }

public class DeviceSecurePassthroughResult
{
    public string Response { get; set; } = null!;
}
