using Microsoft.Extensions.DependencyInjection;
using SharedContextSample.Contracts;

namespace SharedContextSample.Services;

public static class SharedContext
{
    private static readonly ThreadLocal<IHttpClientFactory?> HttpClientFactoryHolder = new(() => null);
    private static readonly ThreadLocal<IApiClientFactory?> ApiClientFactoryHolder = new(() => null);

    public static IHttpClientFactory GetHttpClientFactory()
    {
        var requestServices = RequestContextAccessor.Current.Value;

        if (requestServices is not null)
        {
            return requestServices.GetRequiredService<IHttpClientFactory>();
        }

        return HttpClientFactoryHolder.Value
            ?? throw new InvalidOperationException("No IHttpClientFactory is available for the current thread.");
    }

    public static void SetHttpClientFactory(IHttpClientFactory factory)
    {
        HttpClientFactoryHolder.Value = factory;
    }

    public static IApiClientFactory GetApiClientFactory()
    {
        var requestServices = RequestContextAccessor.Current.Value;

        if (requestServices is not null)
        {
            return requestServices.GetRequiredService<IApiClientFactory>();
        }

        return ApiClientFactoryHolder.Value
            ?? throw new InvalidOperationException("No IApiClientFactory is available for the current thread.");
    }

    public static void SetApiClientFactory(IApiClientFactory factory)
    {
        ApiClientFactoryHolder.Value = factory;
    }

    public static IApiClient GetApiClient(string? baseUrl = null)
    {
        var client = GetApiClientFactory().Create();

        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            client.BaseUrl = new Uri(baseUrl, UriKind.Absolute);
        }

        return client;
    }
}
