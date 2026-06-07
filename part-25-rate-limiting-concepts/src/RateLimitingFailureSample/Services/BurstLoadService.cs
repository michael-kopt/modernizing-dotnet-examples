using System.Diagnostics;
using System.Net.Http.Json;

namespace RateLimitingFailureSample.Services;

public sealed class BurstLoadService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BurstLoadService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<BurstRunResult> RunAsync(int requests, int workMs, int memoryKb, CancellationToken cancellationToken)
    {
        if (requests <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requests), "requests must be greater than zero.");
        }

        var client = _httpClientFactory.CreateClient("self");

        var tasks = Enumerable.Range(0, requests)
            .Select(_ => ExecuteOneAsync(client, workMs, memoryKb, cancellationToken))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        var orderedElapsed = results.Select(result => result.ElapsedMs).OrderBy(value => value).ToArray();
        var p95Index = Math.Min(orderedElapsed.Length - 1, (int)Math.Ceiling(orderedElapsed.Length * 0.95) - 1);

        return new BurstRunResult(
            Requests: requests,
            SingleRequestWorkMs: workMs,
            AverageElapsedMs: Math.Round(orderedElapsed.Average(), 2),
            P95ElapsedMs: Math.Round(orderedElapsed[p95Index], 2),
            MaxElapsedMs: Math.Round(orderedElapsed[^1], 2),
            MaxActiveRequestsObserved: results.Max(result => result.ActiveRequestsObserved),
            Results: results);
    }

    private static async Task<UnsafeRequestResult> ExecuteOneAsync(HttpClient client, int workMs, int memoryKb, CancellationToken cancellationToken)
    {
        var uri = $"/unsafe/process?workMs={workMs}&memoryKb={memoryKb}";
        var stopwatch = Stopwatch.StartNew();
        var response = await client.GetFromJsonAsync<UnsafeRequestResult>(uri, cancellationToken)
            ?? throw new InvalidOperationException("Request returned no payload.");
        stopwatch.Stop();

        return response with
        {
            ElapsedMs = Math.Round(Math.Max(response.ElapsedMs, stopwatch.Elapsed.TotalMilliseconds), 2)
        };
    }
}

public sealed record BurstRunResult(
    int Requests,
    int SingleRequestWorkMs,
    double AverageElapsedMs,
    double P95ElapsedMs,
    double MaxElapsedMs,
    int MaxActiveRequestsObserved,
    IReadOnlyList<UnsafeRequestResult> Results);
