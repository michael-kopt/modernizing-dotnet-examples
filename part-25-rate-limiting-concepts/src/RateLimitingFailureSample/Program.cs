using RateLimitingFailureSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("self", client =>
{
    client.BaseAddress = new Uri("http://localhost:5891");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddSingleton<UnsafeWorkService>();
builder.Services.AddSingleton<BurstLoadService>();

var app = builder.Build();

app.MapGet("/", () => Results.Json(new
{
    Endpoints = new[]
    {
        "/unsafe/process?workMs=80&memoryKb=256",
        "/unsafe/burst?requests=24&workMs=80&memoryKb=256"
    }
}));

app.MapGet("/unsafe/process", async (UnsafeWorkService service, int workMs = 80, int memoryKb = 256, CancellationToken cancellationToken = default) =>
{
    var result = await service.ProcessAsync(workMs, memoryKb, cancellationToken);
    return Results.Ok(result);
});

app.MapGet("/unsafe/burst", async (BurstLoadService service, int requests = 24, int workMs = 80, int memoryKb = 256, CancellationToken cancellationToken = default) =>
{
    var result = await service.RunAsync(requests, workMs, memoryKb, cancellationToken);
    return Results.Ok(result);
});

app.Run("http://localhost:5891");
