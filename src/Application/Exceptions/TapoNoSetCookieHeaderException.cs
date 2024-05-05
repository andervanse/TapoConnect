namespace Tapo.Application.Exceptions;
public class TapoNoSetCookieHeaderException : TapoException
{
    public TapoNoSetCookieHeaderException(string? message) : base(message)
    {
    }
}
