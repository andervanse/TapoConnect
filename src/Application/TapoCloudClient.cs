using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tapo.Application.Dto;
using Tapo.Application.Exceptions;
using Tapo.Application.Protocol;
using Tapo.Application.Util;

namespace Tapo.Application;

public class TapoCloudClient : ITapoCloudClient
{
    private const string DefaultAppType = "Tapo_Android";
    private readonly HttpClient _httpClient;
    private readonly ILogger<TapoCloudClient> _logger;
    protected virtual string AppType { get; }
    protected virtual JsonSerializerOptions JsonSerializerOptions { get; }

    public TapoCloudClient(        
        HttpClient httpClient,
        ILogger<TapoCloudClient> logger,
        string appType = DefaultAppType, 
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        _httpClient = httpClient;
        _logger = logger;
        AppType = appType;

        if (jsonSerializerOptions != null)
        {
            JsonSerializerOptions = jsonSerializerOptions;
        }
        else
        {
            JsonSerializerOptions = new JsonSerializerOptions { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            JsonSerializerOptions.Converters.Add(new TapoJsonDateTimeConverter());
        }
    }


    public virtual async Task<CloudLoginResult> LoginAsync(
        string email,
        string password,
        bool refreshTokenNeeded = false)
    {
        _logger.LogDebug("email: {email}", email);
        ArgumentNullException.ThrowIfNullOrEmpty(nameof(email));
        ArgumentNullException.ThrowIfNullOrEmpty(nameof(password));

        var request = new TapoRequest<object>
        {
            Method = "login",
            Params = new
            {
                appType = AppType,
                cloudUserName = email,
                cloudPassword = password,
                refreshTokenNeeded,
                terminalUUID = TapoCrypto.UuidV4(),
            }
        };

        var requestJson = JsonSerializer.Serialize(request, JsonSerializerOptions);

        _logger.LogDebug("request: {request}", requestJson);

        var requestContent = new StringContent(
            requestJson,
            Encoding.UTF8,
            "application/json");

        using var message = new HttpRequestMessage(HttpMethod.Post, _httpClient.BaseAddress)
        {
            Method = HttpMethod.Post,
            Content = requestContent
        };

        _logger.LogDebug("BaseAddress: {baseAddress}", _httpClient.BaseAddress);
        var response = await _httpClient.SendAsync(message);
        _logger.LogDebug("status code: {statusCode}, reason phrase: {reasonPhrase}", response.StatusCode, response.ReasonPhrase);
        response.EnsureSuccessStatusCode();
        var responseContentString = await response.Content.ReadAsStringAsync();
        _logger.LogDebug("responseContentString: {responseContentString}", responseContentString);

        var responseJson = JsonSerializer.Deserialize<CloudLoginResponse>(responseContentString, JsonSerializerOptions);

        if (responseJson is null)
        {
            throw new TapoJsonException($"Failed to deserialize {responseJson}.");
        }
        else
        {
            TapoException.ThrowFromErrorCode(responseJson.ErrorCode);
            return responseJson.Result;
        }
    }


    public virtual async Task<CloudRefreshLoginResult> RefreshLoginAsync(
        string refreshToken)
    {
        ArgumentNullException.ThrowIfNull(nameof(refreshToken));

        var request = new TapoRequest<object>
        {
            Method = "refreshToken",
            Params = new
            {
                appType = AppType,
                refreshToken,
                terminalUUID = TapoCrypto.UuidV4(),
            }
        };

        var requestJson = JsonSerializer.Serialize(request, JsonSerializerOptions);

        var requestContent = new StringContent(
            requestJson,
            Encoding.UTF8,
            "application/json");

        using var message = new HttpRequestMessage(HttpMethod.Post, _httpClient.BaseAddress)
        {
            Method = HttpMethod.Post,
            Content = requestContent,
        };

        var response = await _httpClient.SendAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response);
        }

        var responseContentString = await response.Content.ReadAsStringAsync();

        var responseJson = JsonSerializer.Deserialize<CloudRefreshLoginResponse>(responseContentString, JsonSerializerOptions);
        if (responseJson == null)
        {
            throw new TapoJsonException($"Failed to deserialize {responseJson}.");
        }
        else
        {
            TapoException.ThrowFromErrorCode(responseJson.ErrorCode);

            return responseJson.Result;
        }
    }

    public virtual async Task<CloudListDeviceResult> ListDevicesAsync(string cloudToken)
    {
        ArgumentNullException.ThrowIfNull(nameof(cloudToken));

        var request = new TapoRequest
        {
            Method = "getDeviceList",
        };

        var requestJson = JsonSerializer.Serialize(request, JsonSerializerOptions);

        var requestContent = new StringContent(
            requestJson,
            Encoding.UTF8,
            "application/json");

        var url = $"{_httpClient.BaseAddress}?token={cloudToken}";

        using var message = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Method = HttpMethod.Post,
            Content = requestContent,
        };

        var response = await _httpClient.SendAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response);
        }

        var responseContentString = await response.Content.ReadAsStringAsync();

        var responseJson = JsonSerializer.Deserialize<CloudListDeviceReponse>(responseContentString, JsonSerializerOptions);

        if (responseJson == null)
        {
            throw new TapoJsonException($"Failed to deserialize {responseJson}.");
        }
        else
        {
            TapoException.ThrowFromErrorCode(responseJson.ErrorCode);

            foreach (var d in responseJson.Result.DeviceList)
            {
                if (TapoUtils.IsTapoDevice(d.DeviceType))
                {
                    d.Alias = TapoCrypto.Base64Decode(d.Alias);
                }
            }

            TapoException.ThrowFromErrorCode(responseJson.ErrorCode);

            return responseJson.Result;
        }
    }
}
