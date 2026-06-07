using SharedContextSample.Contracts;
using SharedContextSample.Models;
using SharedContextSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("Default");
builder.Services.AddSingleton<IApiClientFactory, ApiClientFactory>();
builder.Services.AddSingleton<WorkResultStore>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    RequestContextAccessor.Current.Value = context.RequestServices;

    try
    {
        await next();
    }
    finally
    {
        RequestContextAccessor.Current.Value = null;
    }
});

app.MapGet("/ping", () => Results.Text("pong"));

app.MapPost("/work/reset", (WorkResultStore store) =>
{
    store.Clear();
    return Results.NoContent();
});

app.MapGet("/work/results", (WorkResultStore store) => Results.Ok(store.GetAll()));

app.MapPost("/work/request-thread", (WorkResultStore store) =>
{
    var factory = SharedContext.GetApiClientFactory();

    var thread = new Thread(() =>
    {
        SharedContext.SetApiClientFactory(factory);

        using var client = SharedContext.GetApiClient("http://localhost:5880");
        var response = client.ExecuteAsync(new ApiRequest
        {
            Resource = "ping",
            Method = HttpMethod.Get
        }).GetAwaiter().GetResult();

        store.Add($"thread:{response}");
    });

    thread.Start();
    thread.Join();

    return Results.Accepted("/work/results");
});

app.MapPost("/work/timer", (WorkResultStore store) =>
{
    var factory = SharedContext.GetApiClientFactory();
    using var completion = new ManualResetEventSlim(false);
    using var timer = new Timer(_ =>
    {
        SharedContext.SetApiClientFactory(factory);

        using var client = SharedContext.GetApiClient("http://localhost:5880");
        var response = client.ExecuteAsync(new ApiRequest
        {
            Resource = "ping",
            Method = HttpMethod.Get
        }).GetAwaiter().GetResult();

        store.Add($"timer:{response}");
        completion.Set();
    }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

    timer.Change(TimeSpan.Zero, Timeout.InfiniteTimeSpan);

    if (!completion.Wait(TimeSpan.FromSeconds(5)))
    {
        return Results.Problem("Timer callback did not complete within 5 seconds.");
    }

    return Results.Accepted("/work/results");
});

app.Run("http://localhost:5880");
