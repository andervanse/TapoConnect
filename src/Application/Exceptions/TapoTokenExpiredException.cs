namespace Tapo.Application.Exceptions;
public class TapoTokenExpiredException : TapoException
{
    public TapoTokenExpiredException(string? message) : base(CloudTokenExpiredOrInvalidErrorCode, message)
    {
    }
}
