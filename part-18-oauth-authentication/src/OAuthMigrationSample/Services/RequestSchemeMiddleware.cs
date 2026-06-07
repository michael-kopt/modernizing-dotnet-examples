namespace OAuthMigrationSample.Services;

public sealed class RequestSchemeMiddleware(RequestDelegate next, ILogger<RequestSchemeMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        var requestScheme = GetActualScheme(context);

        if (Uri.UriSchemeHttps.Equals(requestScheme, StringComparison.OrdinalIgnoreCase) &&
            !Uri.UriSchemeHttps.Equals(context.Request.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Promoting request scheme to HTTPS based on forwarded headers.");
            context.Request.Scheme = Uri.UriSchemeHttps;
        }

        await next(context);
    }

    private static string GetActualScheme(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-Proto", out var proto))
        {
            return proto.ToString();
        }

        if (context.Request.Headers.TryGetValue("X-Forwarded-Ssl", out var ssl))
        {
            return ssl.ToString().Equals("on", StringComparison.OrdinalIgnoreCase)
                ? Uri.UriSchemeHttps
                : Uri.UriSchemeHttp;
        }

        return context.Request.Scheme;
    }
}
