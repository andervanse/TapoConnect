namespace Tapo.Application.Exceptions;
public class TapoProtocolDeprecatedException : TapoException
{
    public TapoProtocolDeprecatedException(string? message) : base(message)
    {
    }
}
