namespace Tapo.Application.Exceptions;

public class TapoUnknownDeviceKeyProtocolException : TapoException
{
    public TapoUnknownDeviceKeyProtocolException(string? message) : base(message)
    {
    }
}
