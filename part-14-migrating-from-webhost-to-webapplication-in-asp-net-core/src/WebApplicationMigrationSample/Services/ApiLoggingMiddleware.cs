namespace WebApplicationMigrationSample.Services;

public sealed class ApiLoggingMiddleware(RequestDelegate next, ILogger<ApiLoggingMiddleware> logger)
{
    public const string FlagKey = "api-logging-ran";

    public async Task InvokeAsync(HttpContext context)
    {
        context.Items[FlagKey] = true;
        logger.LogInformation("API request for {Path}", context.Request.Path);

        await next(context);
    }
}
