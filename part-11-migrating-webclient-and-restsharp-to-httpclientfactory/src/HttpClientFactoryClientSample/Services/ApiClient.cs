using System.Net;
using System.Text.Json;
using Part11.HttpClientFactoryClientSample.Contracts;
using Part11.HttpClientFactoryClientSample.Models;

namespace Part11.HttpClientFactoryClientSample.Services;

public sealed class ApiClient(IHttpClientFactory clientFactory) : IApiClient
{
    private bool _disposed;

    public Uri? BaseUrl { get; set; }
    public CookieContainer CookieContainer { get; set; } = new();
    public bool FollowRedirects { get; set; } = true;

    public async Task<T?> ExecuteAsync<T>(ApiRequest request) where T : class
    {
        var httpClient = CreateHttpClient(request);
        using var httpRequest = request.ToHttpRequestMessage();
        using var response = await httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(content) ? default : JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    public async Task<string> ExecuteAsync(ApiRequest request, CookieContainer? cc = null)
    {
        var httpClient = CreateHttpClient(request, cc);
        using var httpRequest = request.ToHttpRequestMessage();
        using var response = await httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<List<T>?> ExecuteForArrayAsync<T>(ApiRequest request) where T : class
    {
        var httpClient = CreateHttpClient(request);
        using var httpRequest = request.ToHttpRequestMessage();
        using var response = await httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(content) ? default : JsonSerializer.Deserialize<List<T>>(content, JsonOptions);
    }

    private HttpClient CreateHttpClient(ApiRequest request, CookieContainer? cc = null)
    {
        var useCookies = cc is not null || CookieContainer.Count > 0;
        var clientName = GetClientName(useCookies, FollowRedirects);
        var client = clientFactory.CreateClient(clientName);

        if (BaseUrl is not null)
        {
            client.BaseAddress = new Uri(BaseUrl.ToString().TrimEnd('/') + "/");
        }

        if (request.Timeout > 0)
        {
            client.Timeout = TimeSpan.FromSeconds(request.Timeout);
        }

        var cookieContainer = cc ?? CookieContainer;
        if (useCookies)
        {
            client.DefaultRequestHeaders.Remove("Cookie");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", BuildCookieHeader(cookieContainer));
        }

        return client;
    }

    private static string GetClientName(bool useCookies, bool followRedirects)
    {
        if (!useCookies && followRedirects) return "Default";
        if (!useCookies && !followRedirects) return "NoRedirectClient";
        if (useCookies && followRedirects) return "CookiesClient";
        return "NoCookiesNoRedirectClient";
    }

    private static string BuildCookieHeader(CookieContainer cookieContainer)
    {
        var uri = new Uri("http://localhost:5780/");
        var cookies = cookieContainer.GetCookies(uri).Cast<Cookie>();
        return string.Join("; ", cookies.Select(cookie => $"{cookie.Name}={cookie.Value}"));
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
