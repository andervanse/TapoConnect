
namespace Tapo.Application.Protocol;

public class KlapDeviceKey : TapoDeviceKey
{
    public KlapChiper KlapChiper { get; }

    public KlapDeviceKey(
        string deviceIp,
        string sessionCookie,
        TimeSpan? timeout,
        DateTime issueTime,
        KlapChiper klapChiper)
        : base(TapoDeviceProtocol.Klap, deviceIp, sessionCookie, timeout, issueTime)
    {
        KlapChiper = klapChiper;
    }
}
