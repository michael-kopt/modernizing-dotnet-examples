using Microsoft.AspNetCore.Http;

namespace System.Web;

public sealed class HttpResponse
{
    private Microsoft.AspNetCore.Http.HttpResponse Inner => HttpContext.Current.Response;

    public int StatusCode
    {
        get => Inner.StatusCode;
        set => Inner.StatusCode = value;
    }

    public string? ContentType
    {
        get => Inner.ContentType;
        set => Inner.ContentType = value;
    }

    public IHeaderDictionary Headers => Inner.Headers;

    public Stream Body
    {
        get => Inner.Body;
        set => Inner.Body = value;
    }

    public bool IsClientConnected => !HttpContext.Current.RequestAborted.IsCancellationRequested;

    public void AppendHeader(string name, string value)
    {
        Inner.Headers.Append(name, value);
    }

    public void AddHeader(string name, string value)
    {
        Inner.Headers[name] = value;
    }

    public Task WriteAsync(string value)
    {
        return Inner.WriteAsync(value);
    }

    public Task FlushAsync()
    {
        return Inner.Body.FlushAsync();
    }

    public void Redirect(string location, bool permanent = false)
    {
        Inner.Redirect(location, permanent);
    }
}
