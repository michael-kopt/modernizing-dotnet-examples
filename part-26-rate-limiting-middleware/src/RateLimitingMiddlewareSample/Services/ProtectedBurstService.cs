using System.Net;
using System.Net.Http.Json;

namespace RateLimitingMiddlewareSample.Services;

public sealed class ProtectedBurstService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProtectedBurstService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ProtectedBurstResult> RunAsync(int requests, int workMs, int memoryKb, CancellationToken cancellationToken)
    {
        if (requests <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requests), "requests must be greater than zero.");
        }

        var client = _httpClientFactory.CreateClient("self");

        var tasks = Enumerable.Range(0, requests)
            .Select(_ => ExecuteOneAsync(client, workMs, memoryKb, cancellationToken))
            .ToArray();

        var responses = await Task.WhenAll(tasks);

        var successful = responses.Where(response => response.StatusCode == (int)HttpStatusCode.OK).ToArray();
        var rejected = responses.Where(response => response.StatusCode == (int)HttpStatusCode.ServiceUnavailable).ToArray();
        var elapsedValues = successful.Select(response => response.Result?.ElapsedMs ?? 0).OrderBy(value => value).ToArray();

        var p95 = elapsedValues.Length == 0
            ? 0
            : elapsedValues[Math.Min(elapsedValues.Length - 1, (int)Math.Ceiling(elapsedValues.Length * 0.95) - 1)];

        return new ProtectedBurstResult(
            Requests: requests,
            SucceededRequests: successful.Length,
            RejectedRequests: rejected.Length,
            AverageElapsedMs: elapsedValues.Length == 0 ? 0 : Math.Round(elapsedValues.Average(), 2),
            P95ElapsedMs: Math.Round(p95, 2),
            MaxActiveRequestsObserved: successful.Length == 0 ? 0 : successful.Max(response => response.Result?.ActiveRequestsObserved ?? 0),
            Responses: responses);
    }

    private static async Task<ProtectedBurstItemResult> ExecuteOneAsync(HttpClient client, int workMs, int memoryKb, CancellationToken cancellationToken)
    {
        var response = await client.GetAsync($"/protected/process?workMs={workMs}&memoryKb={memoryKb}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var payload = await response.Content.ReadFromJsonAsync<ProtectedRequestResult>(cancellationToken: cancellationToken);
            return new ProtectedBurstItemResult((int)response.StatusCode, payload, null);
        }

        var error = await response.Content.ReadAsStringAsync(cancellationToken);
        return new ProtectedBurstItemResult((int)response.StatusCode, null, error);
    }
}

public sealed record ProtectedBurstResult(
    int Requests,
    int SucceededRequests,
    int RejectedRequests,
    double AverageElapsedMs,
    double P95ElapsedMs,
    int MaxActiveRequestsObserved,
    IReadOnlyList<ProtectedBurstItemResult> Responses);

public sealed record ProtectedBurstItemResult(
    int StatusCode,
    ProtectedRequestResult? Result,
    string? Error);
