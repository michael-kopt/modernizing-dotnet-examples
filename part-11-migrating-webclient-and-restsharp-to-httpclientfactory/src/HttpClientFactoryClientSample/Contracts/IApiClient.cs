using System.Net;
using Part11.HttpClientFactoryClientSample.Models;

namespace Part11.HttpClientFactoryClientSample.Contracts;

public interface IApiClient : IDisposable
{
    Uri? BaseUrl { get; set; }
    CookieContainer CookieContainer { get; set; }
    bool FollowRedirects { get; set; }
    Task<T?> ExecuteAsync<T>(ApiRequest request) where T : class;
    Task<string> ExecuteAsync(ApiRequest request, CookieContainer? cc = null);
    Task<List<T>?> ExecuteForArrayAsync<T>(ApiRequest request) where T : class;
}
