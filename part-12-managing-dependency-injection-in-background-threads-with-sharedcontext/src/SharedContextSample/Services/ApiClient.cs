using SharedContextSample.Contracts;
using SharedContextSample.Models;

namespace SharedContextSample.Services;

public sealed class ApiClient(IHttpClientFactory httpClientFactory) : IApiClient
{
    public Uri? BaseUrl { get; set; }

    public async Task<string> ExecuteAsync(ApiRequest request)
    {
        using var httpClient = httpClientFactory.CreateClient("Default");

        if (BaseUrl is not null)
        {
            httpClient.BaseAddress = BaseUrl;
        }

        using var message = new HttpRequestMessage(request.Method, request.Resource);

        foreach (var header in request.Headers)
        {
            message.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        using var response = await httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
