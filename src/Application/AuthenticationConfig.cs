namespace Tapo.Application;

public class AuthenticationConfig
{
    public const string Authentication = "Authentication";
    public string CloudUrl {get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public IEnumerable<DeviceConfig>? Devices { get; set; }
}
