namespace Tapo.Application.Dto;

public class CloudRefreshLoginResponse : TapoResponse<CloudRefreshLoginResult>
{
}

public class CloudRefreshLoginResult
{
    public string Token { get; set; } = null!;
}
