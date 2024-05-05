using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Tapo.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddKeyedScoped<ITapoCloudClient, TapoCloudClient>(nameof(TapoCloudClient));
builder.Services.AddKeyedScoped<ITapoDeviceClient, TapoDeviceClient>(nameof(TapoDeviceClient));

builder.Services.AddOptions<AuthenticationConfig>()
    .Bind(builder.Configuration.GetSection(AuthenticationConfig.Authentication));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/hello", async ([FromKeyedServices(nameof(TapoCloudClient))] ITapoCloudClient cloudClient) => {

    var response = await cloudClient.LoginAsync("abc", "abc");
    return Results.Ok(response);
})
.WithName("GetHello")
.WithOpenApi();

app.Run();
