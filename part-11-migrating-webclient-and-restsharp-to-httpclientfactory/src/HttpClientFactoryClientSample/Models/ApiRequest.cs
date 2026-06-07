using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Part11.HttpClientFactoryClientSample.Models;

public sealed class ApiRequest
{
    public string Resource { get; set; } = "/";
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public Dictionary<string, string> Headers { get; set; } = [];
    public object? Body { get; set; }
    public string ContentType { get; set; } = "application/json";
    public int Timeout { get; set; } = 100;

    public HttpRequestMessage ToHttpRequestMessage()
    {
        var request = new HttpRequestMessage(Method, Resource);

        foreach (var header in Headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (Body is not null)
        {
            var json = JsonSerializer.Serialize(Body);
            request.Content = new StringContent(json, Encoding.UTF8, ContentType);
        }

        return request;
    }
}
