using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Tapo.Application;
using Tapo.WebAPI;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.ConfigureDependencies(configuration);
var app = builder.Build();
app.ConfigureApp(configuration);



app.MapGet("/devices", async (
    [FromServices] ITapoCloudClient cloudClient,
    IOptions<AuthenticationConfig> options) => {
    var config = options.Value;
    var loginResult = await cloudClient.LoginAsync(config.Email, config.Password);
    var listDevicesResponse = await cloudClient.ListDevicesAsync(loginResult.Token);
    return Results.Ok(listDevicesResponse);
})
.WithName("GetDevices")
.WithOpenApi();

app.MapGet("/devices/{ip}", async (
    [FromServices] ITapoDeviceClient deviceClient,
    IOptions<AuthenticationConfig> options,
    [FromRoute] string ip) => {
    var config = options.Value;
    var deviceKey = await deviceClient.LoginByIpAsync(ip, config.Email, config.Password);
    var deviceInfoResult = await deviceClient.GetDeviceInfoAsync(deviceKey);
    return Results.Ok(deviceInfoResult);
})
.WithName("GetDeviceInfo")
.WithOpenApi();

app.MapPatch("/devices/{ip}/{state:bool}", async (
    [FromServices] ITapoDeviceClient deviceClient,
    IOptions<AuthenticationConfig> options,
    [FromRoute] string ip, [FromRoute] bool state) => {
    var config = options.Value;
    var deviceKey = await deviceClient.LoginByIpAsync(ip, config.Email, config.Password);
    await deviceClient.SetStateAsync(deviceKey, new TapoSetPlugState(state));
    return Results.NoContent();
})
.WithName("SetDeviceState")
.WithOpenApi();

app.Run();
