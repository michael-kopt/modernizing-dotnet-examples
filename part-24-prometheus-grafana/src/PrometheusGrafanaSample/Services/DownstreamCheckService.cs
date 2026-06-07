namespace PrometheusGrafanaSample.Services;

public sealed class DownstreamCheckService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DownstreamCheckService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<DownstreamCheckResult> RunAsync(int calls, CancellationToken cancellationToken)
    {
        if (calls <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(calls), "calls must be greater than zero.");
        }

        var client = _httpClientFactory.CreateClient("downstream");
        var statuses = new List<int>(capacity: calls);

        for (var index = 0; index < calls; index++)
        {
            using var response = await client.GetAsync("/health", cancellationToken);
            statuses.Add((int)response.StatusCode);
        }

        return new DownstreamCheckResult(calls, statuses);
    }
}

public sealed record DownstreamCheckResult(int Calls, IReadOnlyList<int> StatusCodes);
