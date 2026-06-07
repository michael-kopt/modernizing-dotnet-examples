using Microsoft.AspNetCore.Http;

namespace Part02.Legacy;

public sealed record RequestSnapshot(
    string Mode,
    string Method,
    string Path,
    string TraceIdentifier,
    string? UserAgent)
{
    public static RequestSnapshot From(HttpContext context, string mode)
    {
        return new RequestSnapshot(
            mode,
            context.Request.Method,
            context.Request.Path.Value ?? string.Empty,
            context.TraceIdentifier,
            context.Request.Headers.UserAgent.ToString());
    }
}
