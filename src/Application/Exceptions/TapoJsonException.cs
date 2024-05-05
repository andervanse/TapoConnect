namespace Tapo.Application.Exceptions;
public class TapoJsonException : TapoException
{
    public TapoJsonException(string? message) : base(JsonFormatErrorCode, message)
    {
    }
}
