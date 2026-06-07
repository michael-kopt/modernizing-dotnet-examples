using SharedContextSample.Models;

namespace SharedContextSample.Contracts;

public interface IApiClient : IDisposable
{
    Uri? BaseUrl { get; set; }

    Task<string> ExecuteAsync(ApiRequest request);
}
