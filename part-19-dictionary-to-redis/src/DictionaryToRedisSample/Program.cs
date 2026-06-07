using Microsoft.Extensions.Caching.Distributed;
using DictionaryToRedisSample.Models;
using DictionaryToRedisSample.Services;

var builder = WebApplication.CreateBuilder(args);

var redisActive = bool.TryParse(Environment.GetEnvironmentVariable("REDIS_ACTIVE"), out var redisFlag) && redisFlag;
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost:6379";

if (redisActive)
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = "modernizing-dotnet:";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

builder.Services.AddSingleton(new CacheMode(redisActive, redisConnectionString));
builder.Services.AddSingleton<DistributedTokenManager>();
builder.Services.AddScoped<SessionHelper>();

var app = builder.Build();

app.MapPost("/auth/login", async (LoginRequest request, SessionHelper sessionHelper) =>
{
    if (!string.Equals(request.Username, "demo", StringComparison.OrdinalIgnoreCase) ||
        request.Password != "pass123")
    {
        return Results.Unauthorized();
    }

    var user = new User
    {
        Id = 1,
        Username = request.Username,
        DisplayName = "Demo User",
        Roles = ["admin", "reporting"]
    };

    var token = await sessionHelper.GenerateLoginTokenAsync(user);
    return Results.Ok(new { token });
});

app.MapGet("/auth/validate", async (string token, SessionHelper sessionHelper) =>
{
    var isValid = await sessionHelper.IsValidTokenAsync(token);
    if (!isValid)
    {
        return Results.Unauthorized();
    }

    var user = await sessionHelper.GetUserAsync(token);
    return Results.Ok(new { user });
});

app.MapPost("/auth/logout", async (string token, SessionHelper sessionHelper) =>
{
    await sessionHelper.LogoutAsync(token);
    return Results.NoContent();
});

app.MapGet("/auth/token-count", async (DistributedTokenManager tokenManager) =>
{
    var count = await tokenManager.GetActiveTokenCountAsync();
    return Results.Ok(new { count });
});

app.MapGet("/auth/cache-info", (CacheMode mode) => Results.Ok(new
{
    redisActive = mode.RedisActive,
    connection = mode.RedisConnectionString
}));

app.Run("http://localhost:5885");
