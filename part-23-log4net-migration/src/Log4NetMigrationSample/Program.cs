using log4net;
using log4net.Config;
using Log4NetMigrationSample.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryAppender>();
builder.Services.AddSingleton<LogDemoService>();

var app = builder.Build();

var repository = LogManager.GetRepository(typeof(Program).Assembly);
var memoryAppender = app.Services.GetRequiredService<InMemoryAppender>();
XmlConfigurator.Configure(repository, new FileInfo(Path.Combine(app.Environment.ContentRootPath, "log4net.config")));
memoryAppender.AttachToRootLogger();

app.MapGet("/", () => Results.Redirect("/demo"));

app.MapGet("/demo", async (HttpContext httpContext, LogDemoService demoService, InMemoryAppender appender, string? userId, string? tenantId, string? sessionId, string? operationId) =>
{
    appender.Clear();

    using var scope = LoggingScope.Begin(
        userId ?? "jane.doe",
        tenantId ?? "tenant-42",
        sessionId ?? "sess-123",
        operationId);

    var events = await demoService.RunAsyncFlowAsync(httpContext.RequestAborted);

    return Results.Json(new
    {
        Scenario = "async-flow",
        Events = events
    });
});

app.MapGet("/background", async (LogDemoService demoService, InMemoryAppender appender, string? userId, string? tenantId, string? sessionId, string? operationId) =>
{
    appender.Clear();

    using var scope = LoggingScope.Begin(
        userId ?? "jane.doe",
        tenantId ?? "tenant-42",
        sessionId ?? "sess-123",
        operationId);

    var events = await demoService.RunBackgroundFlowAsync();

    return Results.Json(new
    {
        Scenario = "captured-background-flow",
        Events = events
    });
});

app.Run("http://localhost:5889");
