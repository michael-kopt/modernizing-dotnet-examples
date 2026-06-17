using System.Collections.Specialized;
namespace System.Web;

public sealed class HttpRequest
{
    private readonly NameValueCollection _params;
    private readonly Stream _inputStream;

    public HttpRequest()
    {
        var request = HttpContext.Current.Request;
        _params = BuildParams(request);
        _inputStream = BuildInputStream(request);
    }

    public NameValueCollection Params => _params;

    public string HttpMethod => HttpContext.Current.Request.Method;

    public string RawUrl => $"{HttpContext.Current.Request.Path}{HttpContext.Current.Request.QueryString}";

    public string Path => HttpContext.Current.Request.Path;

    public string? UserHostAddress => HttpContext.Current.Connection.RemoteIpAddress?.ToString();

    public string? UserHostName => UserHostAddress;

    public bool IsSecureConnection => HttpContext.Current.Request.IsHttps;

    public Stream InputStream => _inputStream;

    private static NameValueCollection BuildParams(Microsoft.AspNetCore.Http.HttpRequest request)
    {
        var parameters = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

        foreach (var queryParam in request.Query)
        {
            parameters[queryParam.Key] = queryParam.Value.ToString();
        }

        if (request.HasFormContentType)
        {
            foreach (var formItem in request.Form)
            {
                parameters[formItem.Key] = formItem.Value.ToString();
            }
        }

        foreach (var cookie in request.Cookies)
        {
            parameters[cookie.Key] = cookie.Value;
        }

        foreach (var header in request.Headers)
        {
            if (parameters[header.Key] is null)
            {
                parameters[header.Key] = header.Value.ToString();
            }
        }

        return parameters;
    }

    private static Stream BuildInputStream(Microsoft.AspNetCore.Http.HttpRequest request)
    {
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        request.Body.Position = 0;
        return request.Body;
    }
}
