using SharedContextSample.Contracts;

namespace SharedContextSample.Services;

public sealed class ApiClientFactory(IHttpClientFactory httpClientFactory) : IApiClientFactory
{
    public IApiClient Create()
    {
        return new ApiClient(httpClientFactory);
    }
}
