namespace SharedContextSample.Models;

public sealed class ApiRequest
{
    public string Resource { get; init; } = "/";

    public HttpMethod Method { get; init; } = HttpMethod.Get;

    public Dictionary<string, string> Headers { get; init; } = [];
}
