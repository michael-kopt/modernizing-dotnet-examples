namespace Part11.HttpClientFactoryClientSample.Contracts;

public interface IWebClientService
{
    Task<byte[]> DownloadDataAsync(string url, string acceptHeader = "", Dictionary<string, string>? headers = null);
    Task<string> DownloadStringAsync(string url, string acceptHeader = "", Dictionary<string, string>? headers = null);
    Task<string> UploadStringAsync(string url, string method, string data, string contentType = "", string acceptHeader = "", Dictionary<string, string>? headers = null);

    byte[] DownloadData(string url, string acceptHeader = "", Dictionary<string, string>? headers = null);
    string DownloadString(string url, string acceptHeader = "", Dictionary<string, string>? headers = null);
    string UploadString(string url, string method, string data, string contentType = "", string acceptHeader = "", Dictionary<string, string>? headers = null);
}
