
namespace Tapo.Application.Dto;

public class CloudLoginResponse : TapoResponse<CloudLoginResult>
{
}

public class CloudLoginResult
{
    public string AccountId { get; set; } = null!;

    public DateTime RegTime { get; set; }

    public string CountryCode { get; set; } = null!;

    public int RiskDetected { get; set; }

    public string Nickname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    public string? RefreshToken { get; set; }
}
