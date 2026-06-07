using System.Threading.RateLimiting;
using RateLimitingMiddlewareSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("self", client =>
{
    client.BaseAddress = new Uri("http://localhost:5892");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddSingleton<ProtectedWorkService>();
builder.Services.AddSingleton<ProtectedBurstService>();

var limiterSettings = RateLimiterSettings.FromEnvironment();
builder.Services.AddSingleton(limiterSettings);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status503ServiceUnavailable;
    options.OnRejected = static (context, cancellationToken) =>
    {
        context.HttpContext.Response.Headers.RetryAfter = "2";
        return new ValueTask(context.HttpContext.Response.WriteAsJsonAsync(new
        {
            Error = "rate_limited",
            Message = "The concurrency limit and queue are full. Retry later."
        }, cancellationToken));
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
        RateLimitPartition.GetConcurrencyLimiter("global", _ => new ConcurrencyLimiterOptions
        {
            PermitLimit = limiterSettings.PermitLimit,
            QueueLimit = limiterSettings.QueueLimit,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        }));
});

var app = builder.Build();

app.UseRateLimiter();

app.MapGet("/", (RateLimiterSettings settings) => Results.Json(new
{
    settings.PermitLimit,
    settings.QueueLimit,
    Endpoints = new[]
    {
        "/protected/process?workMs=80&memoryKb=256",
        "/protected/burst?requests=20&workMs=80&memoryKb=256"
    }
}));

app.MapGet("/protected/process", async (ProtectedWorkService service, int workMs = 80, int memoryKb = 256, CancellationToken cancellationToken = default) =>
{
    var result = await service.ProcessAsync(workMs, memoryKb, cancellationToken);
    return Results.Ok(result);
});

app.MapGet("/protected/burst", async (ProtectedBurstService service, int requests = 20, int workMs = 80, int memoryKb = 256, CancellationToken cancellationToken = default) =>
{
    var result = await service.RunAsync(requests, workMs, memoryKb, cancellationToken);
    return Results.Ok(result);
});

app.Run("http://localhost:5892");
