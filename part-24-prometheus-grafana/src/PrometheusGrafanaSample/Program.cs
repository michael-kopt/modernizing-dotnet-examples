using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using PrometheusGrafanaSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("downstream", client =>
{
    client.BaseAddress = new Uri("http://localhost:5890");
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddSingleton<OrderMetrics>();
builder.Services.AddSingleton<OrderProcessingService>();
builder.Services.AddSingleton<DownstreamCheckService>();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("PrometheusGrafanaSample"))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddMeter(OrderMetrics.MeterName)
        .AddMeter("Microsoft.AspNetCore.Hosting")
        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
        .AddPrometheusExporter());

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGet("/", () => Results.Json(new
{
    Endpoints = new[]
    {
        "/orders/process?itemCount=3&outcome=success&delayMs=40",
        "/downstream/check?calls=2",
        "/metrics"
    }
}));

app.MapGet("/health", () => Results.Ok(new
{
    Status = "ok",
    UtcNow = DateTimeOffset.UtcNow
}));

app.MapGet("/orders/process", async (OrderProcessingService service, int itemCount = 1, string outcome = "success", int delayMs = 25, CancellationToken cancellationToken = default) =>
{
    var result = await service.ProcessAsync(itemCount, outcome, delayMs, cancellationToken);
    return Results.Ok(result);
});

app.MapGet("/downstream/check", async (DownstreamCheckService service, int calls = 1, CancellationToken cancellationToken = default) =>
{
    var result = await service.RunAsync(calls, cancellationToken);
    return Results.Ok(result);
});

app.Run("http://localhost:5890");
