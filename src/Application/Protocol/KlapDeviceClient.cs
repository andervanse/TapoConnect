using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tapo.Application.Dto;
using Tapo.Application.Exceptions;
using Tapo.Application.Util;

namespace Tapo.Application.Protocol;

public class KlapDeviceClient : IDeviceProtocol
{
    private const string TpSessionKey = "TP_SESSIONID";
    public TapoDeviceProtocol Protocol => TapoDeviceProtocol.Klap;
    public sealed class KlapHandshakeKey
    {
        public KlapHandshakeKey(
            string sessionCookie,
            TimeSpan? timeout,
            DateTime issueTime,
            byte[] key,
            byte[] iv)
        {
            SessionCookie = sessionCookie;
            Timeout = timeout;
            IssueTime = issueTime;
            RemoteSeed = key;
            AuthHash = iv;
        }

        public string SessionCookie { get; } = null!;
        public TimeSpan? Timeout { get; }
        public DateTime IssueTime { get; }
        public byte[] RemoteSeed { get; } = null!;
        public byte[] AuthHash { get; } = null!;
    }
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<TapoDeviceClient> _logger;
    private readonly HttpClient _httpClient;

    public KlapDeviceClient(
        ILogger<TapoDeviceClient> logger,
        HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        _jsonSerializerOptions.Converters.Add(new TapoJsonDateTimeConverter());
    }

    public async Task<TapoDeviceKey> LoginByIpAsync(string deviceIp, string username, string password)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceIp));
        ArgumentNullException.ThrowIfNull(nameof(username));
        ArgumentNullException.ThrowIfNull(nameof(password));

        var localSeed = TapoCrypto.GenerateRandomBytes(16);
        var handshake1 = await KlapHandshake1Async(deviceIp, username, password, localSeed);
        var klapChiper = await KlapHandshake2Async(deviceIp, handshake1.SessionCookie, localSeed, handshake1.RemoteSeed, handshake1.AuthHash);
        return new KlapDeviceKey(deviceIp, handshake1.SessionCookie, handshake1.Timeout, handshake1.IssueTime, klapChiper);
    }

    public async Task<DeviceGetEnergyUsageResult> GetEnergyUsageAsync(TapoDeviceKey deviceKey)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));

        var protocol = deviceKey.ToProtocol<KlapDeviceKey>();

        var request = new TapoRequest
        {
            Method = "get_energy_usage",
        };

        var jsonRequest = JsonSerializer.Serialize(request, _jsonSerializerOptions);

        var response = await KlapRequestAsync<DeviceGetEnergyUsageResponse>(jsonRequest, deviceKey.DeviceIp, deviceKey.SessionCookie, protocol.KlapChiper);

        return response.Result;
    }

    public async Task SetPowerAsync(TapoDeviceKey deviceKey, bool on)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));

        var protocol = deviceKey.ToProtocol<KlapDeviceKey>();

        var request = new TapoRequest<TapoSetBulbState>
        {
            Method = "set_device_info",
            Params = new TapoSetBulbState(
               on)
        };

        var jsonRequest = JsonSerializer.Serialize(request, _jsonSerializerOptions);

        await KlapRequestAsync<TapoResponse>(jsonRequest, deviceKey.DeviceIp, deviceKey.SessionCookie, protocol.KlapChiper);
    }

    public async Task SetBrightnessAsync(TapoDeviceKey deviceKey, int brightness)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));

        var protocol = deviceKey.ToProtocol<KlapDeviceKey>();

        var request = new TapoRequest<TapoSetBulbState>
        {
            Method = "set_device_info",
            Params = new TapoSetBulbState(
                brightness: brightness)
        };

        var jsonRequest = JsonSerializer.Serialize(request, _jsonSerializerOptions);

        await KlapRequestAsync<TapoResponse>(jsonRequest, deviceKey.DeviceIp, deviceKey.SessionCookie, protocol.KlapChiper);
    }

    public async Task SetColorAsync(TapoDeviceKey deviceKey, TapoColor color)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));
        ArgumentNullException.ThrowIfNull(nameof(color));

        var protocol = deviceKey.ToProtocol<KlapDeviceKey>();

        var request = new TapoRequest<TapoSetBulbState>
        {
            Method = "set_device_info",
            Params = new TapoSetBulbState(color),
        };

        var jsonRequest = JsonSerializer.Serialize(request, _jsonSerializerOptions);

        await KlapRequestAsync<TapoResponse>(jsonRequest, deviceKey.DeviceIp, deviceKey.SessionCookie, protocol.KlapChiper);
    }

    public async Task SetStateAsync<TState>(TapoDeviceKey deviceKey, TState state)
       where TState : TapoSetDeviceState
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));
        ArgumentNullException.ThrowIfNull(nameof(state));

        var protocol = deviceKey.ToProtocol<KlapDeviceKey>();

        var request = new TapoRequest<TState>
        {
            Method = "set_device_info",
            Params = state,
        };

        var jsonRequest = JsonSerializer.Serialize(request, _jsonSerializerOptions);

        await KlapRequestAsync<TapoResponse>(jsonRequest, deviceKey.DeviceIp, deviceKey.SessionCookie, protocol.KlapChiper);
    }

    public async Task<KlapHandshakeKey> KlapHandshake1Async(string deviceIp, string username, string password, byte[] localSeed)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceIp));
        ArgumentNullException.ThrowIfNull(nameof(username));
        ArgumentNullException.ThrowIfNull(nameof(password));
        ArgumentNullException.ThrowIfNull(nameof(localSeed));

        var requestContent = new ByteArrayContent(localSeed);
        var requestTime = DateTime.Now;

        using var response = await _httpClient.PostAsync("/app/handshake1", requestContent);

        _logger.LogDebug("response: {response}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response);
        }

        var responseContentBytes = await response.Content.ReadAsByteArrayAsync();

        if (responseContentBytes is null)
        {
            throw new TapoKlapException($"Payload missing bytes.");
        }
        else
        {
            var remoteSeed = responseContentBytes.Take(16).ToArray();
            var serverHash = responseContentBytes.Skip(16).ToArray();
            var usernameHash = TapoCrypto.Sha1Hash(Encoding.UTF8.GetBytes(username));
            var passwordHash = TapoCrypto.Sha1Hash(Encoding.UTF8.GetBytes(password));
            var authHash = TapoCrypto.Sha256Hash(usernameHash.Concat(passwordHash).ToArray());
            var localSeedAuthHash = TapoCrypto.Sha256Hash(localSeed.Concat(remoteSeed).Concat(authHash).ToArray());

            _logger.LogDebug("localSeed........: {localSeed}", localSeed);
            _logger.LogDebug("remoteSeed.......: {remoteSeed}", remoteSeed);
            _logger.LogDebug("serverHash.......: {serverHash}", serverHash);
            _logger.LogDebug("usernameHash.....: {usernameHash}", usernameHash);
            _logger.LogDebug("localSeedAuthHash: {localSeedAuthHash}", localSeedAuthHash);

            if (localSeedAuthHash.SequenceEqual(serverHash))
            {
                string sessionCookie;
                TimeSpan? timeout = null;

                if (response.Headers.TryGetValues("set-cookie", out var values))
                {
                    var s = values.First();

                    var keyValue = s
                        .Split(';')
                        .Select(x => x.Split('='))
                        .ToDictionary(x => x[0], x => x[1]);

                    if (keyValue.ContainsKey(TpSessionKey))
                    {
                        sessionCookie = $"{TpSessionKey}={keyValue[TpSessionKey]}";
                    }
                    else
                    {
                        throw new Exception("Tapo login did not recieve a session id.");
                    }

                    if (keyValue.ContainsKey("TIMEOUT"))
                    {
                        timeout = TimeSpan.FromSeconds(int.Parse(keyValue["TIMEOUT"]));
                    }

                    return new KlapHandshakeKey(sessionCookie, timeout, requestTime, remoteSeed, authHash);
                }
                else
                {
                    throw new TapoNoSetCookieHeaderException("Tapo login did not recieve a set-cookie header.");
                }
            }
            else
            {
                throw new TapoInvalidRequestException(TapoException.InvalidRequestOrCredentialsErrorCode, "Authentication hash does not match server hash.");
            }
        }
    }

    protected virtual async Task<KlapChiper> KlapHandshake2Async(string deviceIp, string sessionCookie, byte[] localSeed, byte[] remoteSeed, byte[] authHash)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceIp));
        ArgumentNullException.ThrowIfNull(nameof(sessionCookie));
        ArgumentNullException.ThrowIfNull(nameof(localSeed));
        ArgumentNullException.ThrowIfNull(nameof(remoteSeed));
        ArgumentNullException.ThrowIfNull(nameof(authHash));

        _logger.LogDebug("deviceIp.....: {deviceIp}", deviceIp);
        _logger.LogDebug("sessionCookie: {sessionCookie}", sessionCookie);
        _logger.LogDebug("localSeed....: {localSeed}", localSeed);
        _logger.LogDebug("remoteSeed...: {remoteSeed}", remoteSeed);
        _logger.LogDebug("authHash.....: {authHash}", authHash);

        var payload = TapoCrypto.Sha256Hash(remoteSeed.Concat(localSeed).Concat(authHash).ToArray());
        var requestContent = new ByteArrayContent(payload);

        _logger.LogDebug("base address.: {baseAddress}", _httpClient.BaseAddress);
        _logger.LogDebug("payload......: {payload}", payload);
        
        using var response = await _httpClient.PostAsync("/app/handshake2", requestContent);

        _logger.LogDebug("status code..: {status}", response.StatusCode);
        _logger.LogDebug("reason.......: {reason}", response.ReasonPhrase);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response);
        }

        return new KlapChiper(localSeed, remoteSeed, authHash);
    }

    public virtual async Task<DeviceGetInfoResult> GetDeviceInfoAsync(TapoDeviceKey deviceKey)
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceKey));

        var protocol = deviceKey.ToProtocol<KlapDeviceKey>();

        var request = new TapoRequest
        {
            Method = "get_device_info",
        };

        var jsonRequest = JsonSerializer.Serialize(request, _jsonSerializerOptions);

        var response = await KlapRequestAsync<DeviceGetInfoResponse>(jsonRequest, deviceKey.DeviceIp, deviceKey.SessionCookie, protocol.KlapChiper);

        response.Result.Ssid = TapoCrypto.Base64Decode(response.Result.Ssid);
        response.Result.Nickname = TapoCrypto.Base64Decode(response.Result.Nickname);

        return response.Result;
    }

    protected virtual async Task<TResponse> KlapRequestAsync<TResponse>(
        string deviceRequest,
        string deviceIp,
        string sessionCookie,
        KlapChiper klapChiper)
        where TResponse : TapoResponse
    {
        ArgumentNullException.ThrowIfNull(nameof(deviceIp));
        
        _logger.LogDebug("device request.......: {deviceRequest}", deviceRequest);
        _logger.LogDebug("device ip............: {deviceIp}", deviceIp);
        _logger.LogDebug("session cookie.......: {sessionCookie}", sessionCookie);
        _logger.LogDebug("klap chiper seq .....: {klapChiper}", klapChiper.Seq);
        _logger.LogDebug("device request.......: {deviceRequest}", deviceRequest);

        var payload = klapChiper.Encrypt(deviceRequest);
        var requestContent = new ByteArrayContent(payload);
        var baseUrl = $"http://{deviceIp}";
        var url = $"{baseUrl}/app/request?seq={klapChiper.Seq}";

        using var httpClient = new HttpClient();
        using var message = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Method = HttpMethod.Post,
            Content = requestContent,
        };

        if (sessionCookie != null)
            message.Headers.Add("Cookie", sessionCookie);

        var requestTime = DateTime.Now;
        var response = await httpClient.SendAsync(message);

        _logger.LogDebug("status code.........: {statusCode}", response.StatusCode);
        _logger.LogDebug("reason phrase.......: {reason}", response.ReasonPhrase);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response);
        }

        var responseBytes = await response.Content.ReadAsByteArrayAsync();
        var decryptedBytes = klapChiper.Decrypt(responseBytes);
        var decryptedString = Encoding.UTF8.GetString(decryptedBytes);
        var responseJson = JsonSerializer.Deserialize<DeviceSecurePassthroughReponse>(decryptedString, _jsonSerializerOptions);

        if (responseJson is null)
        {
            throw new TapoJsonException($"Failed to deserialize {responseJson}.");
        }
        else
        {
            TapoException.ThrowFromErrorCode(responseJson.ErrorCode);

            var decryptedResponseJson = JsonSerializer.Deserialize<TResponse>(decryptedString, _jsonSerializerOptions);

            if (decryptedResponseJson is null)
            {
                throw new TapoJsonException($"Failed to deserialize {decryptedString}.");
            }
            else
            {
                TapoException.ThrowFromErrorCode(decryptedResponseJson.ErrorCode);
                return decryptedResponseJson;
            }
        }
    }
}
