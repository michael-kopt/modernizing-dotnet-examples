using System.Net.Http.Headers;
using System.Text;
using Part11.HttpClientFactoryClientSample.Contracts;

namespace Part11.HttpClientFactoryClientSample.Services;

public sealed class WebClientService(IHttpClientFactory clientFactory) : IWebClientService, IDisposable
{
    private bool _disposed;

    public async Task<string> DownloadStringAsync(string url, string acceptHeader = "", Dictionary<string, string>? headers = null)
    {
        var client = clientFactory.CreateClient("Default");
        AddAcceptHeader(client, acceptHeader);
        AddCustomHeaders(client, headers);
        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<byte[]> DownloadDataAsync(string url, string acceptHeader = "", Dictionary<string, string>? headers = null)
    {
        var client = clientFactory.CreateClient("Default");
        AddAcceptHeader(client, acceptHeader);
        AddCustomHeaders(client, headers);
        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<string> UploadStringAsync(string url, string method, string data, string contentType = "", string acceptHeader = "", Dictionary<string, string>? headers = null)
    {
        var client = clientFactory.CreateClient("Default");
        AddAcceptHeader(client, acceptHeader);
        AddCustomHeaders(client, headers);
        using var content = CreateContent(data, contentType);

        using HttpResponseMessage response = method.ToUpperInvariant() switch
        {
            "POST" => await client.PostAsync(url, content),
            "PUT" => await client.PutAsync(url, content),
            "DELETE" => await client.DeleteAsync(url),
            _ => throw new InvalidOperationException($"Unsupported method: {method}")
        };

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public string DownloadString(string url, string acceptHeader = "", Dictionary<string, string>? headers = null)
        => DownloadStringAsync(url, acceptHeader, headers).GetAwaiter().GetResult();

    public byte[] DownloadData(string url, string acceptHeader = "", Dictionary<string, string>? headers = null)
        => DownloadDataAsync(url, acceptHeader, headers).GetAwaiter().GetResult();

    public string UploadString(string url, string method, string data, string contentType = "", string acceptHeader = "", Dictionary<string, string>? headers = null)
        => UploadStringAsync(url, method, data, contentType, acceptHeader, headers).GetAwaiter().GetResult();

    private static void AddAcceptHeader(HttpClient client, string acceptHeader)
    {
        if (!string.IsNullOrWhiteSpace(acceptHeader))
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
        }
    }

    private static void AddCustomHeaders(HttpClient client, Dictionary<string, string>? headers)
    {
        if (headers is null)
        {
            return;
        }

        foreach (var header in headers)
        {
            if (client.DefaultRequestHeaders.Contains(header.Key))
            {
                client.DefaultRequestHeaders.Remove(header.Key);
            }

            client.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    private static StringContent CreateContent(string data, string contentType)
    {
        return string.IsNullOrWhiteSpace(contentType)
            ? new StringContent(data ?? string.Empty, Encoding.UTF8)
            : new StringContent(data ?? string.Empty, Encoding.UTF8, contentType);
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
}
