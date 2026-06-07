namespace WebDavMigrationSample.Services;

public static class ResultExtensions
{
    public static IResult WithHeaders(this IResult inner, IReadOnlyDictionary<string, string> headers)
    {
        return new HeaderResult(inner, headers);
    }

    private sealed class HeaderResult(IResult inner, IReadOnlyDictionary<string, string> headers) : IResult
    {
        public async Task ExecuteAsync(HttpContext httpContext)
        {
            foreach (var header in headers)
            {
                httpContext.Response.Headers[header.Key] = header.Value;
            }

            await inner.ExecuteAsync(httpContext);
        }
    }
}
