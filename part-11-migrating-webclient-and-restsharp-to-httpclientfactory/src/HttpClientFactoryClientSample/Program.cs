using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Part11.HttpClientFactoryClientSample.Contracts;
using Part11.HttpClientFactoryClientSample.Models;
using Part11.HttpClientFactoryClientSample.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("Default");
builder.Services.AddHttpClient("NoCookiesClient")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseCookies = false
    });
builder.Services.AddHttpClient("NoRedirectClient")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AllowAutoRedirect = false
    });
builder.Services.AddHttpClient("CookiesClient")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseCookies = false
    });
builder.Services.AddHttpClient("NoCookiesNoRedirectClient")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseCookies = false,
        AllowAutoRedirect = false
    });

builder.Services.AddTransient<IWebClientService, WebClientService>();
builder.Services.AddTransient<IApiClient, ApiClient>();

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var webClientService = scope.ServiceProvider.GetRequiredService<IWebClientService>();
var apiClient = scope.ServiceProvider.GetRequiredService<IApiClient>();

var headers = new Dictionary<string, string>
{
    ["X-Client-Name"] = "webclient-service"
};

var text = await webClientService.DownloadStringAsync("http://localhost:5780/text", "text/plain", headers);
var bytes = await webClientService.DownloadDataAsync("http://localhost:5780/bytes", "application/octet-stream");
var echo = await webClientService.UploadStringAsync(
    "http://localhost:5780/echo",
    "POST",
    "{\"message\":\"hello\"}",
    "application/json",
    "application/json",
    headers);

apiClient.BaseUrl = new Uri("http://localhost:5780/");
apiClient.CookieContainer.Add(new Uri("http://localhost:5780/"), new Cookie("session-id", "factory-cookie"));

var cookieResponse = await apiClient.ExecuteAsync<CookieCheckResponse>(new ApiRequest
{
    Resource = "cookie-check",
    Method = HttpMethod.Get,
    Headers = new Dictionary<string, string>
    {
        ["X-Client-Name"] = "api-client"
    }
});

apiClient.FollowRedirects = false;
string redirectStatus;
try
{
    await apiClient.ExecuteAsync(new ApiRequest
    {
        Resource = "redirect",
        Method = HttpMethod.Get
    });
    redirectStatus = "unexpected-success";
}
catch (HttpRequestException ex)
{
    redirectStatus = ex.Message;
}

Console.WriteLine($"WebClientService.DownloadStringAsync => {text}");
Console.WriteLine($"WebClientService.DownloadDataAsync => {string.Join(',', bytes)}");
Console.WriteLine($"WebClientService.UploadStringAsync => {echo}");
Console.WriteLine($"ApiClient cookie response => sessionId={cookieResponse?.SessionId}");
Console.WriteLine($"ApiClient no-redirect behavior => {redirectStatus}");
