namespace Tapo.Application.Exceptions;
public class TapoProtocolMismatchException : TapoException
{
    public TapoProtocolMismatchException(string? message) : base(message)
    {
    }
}
