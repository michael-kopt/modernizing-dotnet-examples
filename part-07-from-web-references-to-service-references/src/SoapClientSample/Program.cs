using System.Net;
using Part07.SoapClientSample.Client;

var baseUrl = "http://localhost:5580/";
using var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

await httpClient.PostAsync("debug/reset", content: null);

var wrapper = new SoapServiceClientWrapper(
    $"{baseUrl}SoapService.asmx",
    new Dictionary<string, string>
    {
        ["X-Client-Name"] = "svcutil-sample"
    });

wrapper.CookieContainer.Add(
    new Uri(baseUrl),
    new Cookie("session-id", "sample-session"));

var result = wrapper.Login("demo", "pass123");
var debugSnapshot = await httpClient.GetStringAsync("debug/last-request");

Console.WriteLine($"Login result: {result}");
Console.WriteLine($"Host debug snapshot: {debugSnapshot}");
