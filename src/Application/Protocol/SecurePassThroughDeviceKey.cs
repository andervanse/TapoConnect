namespace Tapo.Application.Protocol;

public class SecurePassThroughDeviceKey : TapoDeviceKey
{
    public SecurePassThroughDeviceKey(
        string deviceIp,
        string sessionCookie,
        TimeSpan? timeout,
        DateTime issueTime,
        byte[] key,
        byte[] iv,
        string token) : base(TapoDeviceProtocol.SecurePassThrough, deviceIp, sessionCookie, timeout, issueTime)
    {
        Key = key;
        Iv = iv;
        Token = token;
    }

    public byte[] Key { get; }
    public byte[] Iv { get; }
    public string Token { get; }
}
