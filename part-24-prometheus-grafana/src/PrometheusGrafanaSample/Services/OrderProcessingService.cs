using System.Diagnostics;

namespace PrometheusGrafanaSample.Services;

public sealed class OrderProcessingService
{
    private static readonly HashSet<string> AllowedOutcomes = new(StringComparer.OrdinalIgnoreCase)
    {
        "success",
        "validation_error",
        "dependency_failure"
    };

    private readonly OrderMetrics _metrics;

    public OrderProcessingService(OrderMetrics metrics)
    {
        _metrics = metrics;
    }

    public async Task<OrderProcessingResult> ProcessAsync(int itemCount, string outcome, int delayMs, CancellationToken cancellationToken)
    {
        if (itemCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(itemCount), "itemCount must be greater than zero.");
        }

        if (delayMs < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(delayMs), "delayMs cannot be negative.");
        }

        var normalizedOutcome = NormalizeOutcome(outcome);

        using var activeOrder = _metrics.TrackActiveOrder();

        var stopwatch = Stopwatch.StartNew();
        await Task.Delay(delayMs, cancellationToken);
        stopwatch.Stop();

        _metrics.RecordProcessedOrder(itemCount, normalizedOutcome, stopwatch.Elapsed.TotalMilliseconds);

        return new OrderProcessingResult(
            OrderId: Guid.NewGuid().ToString("n"),
            ItemCount: itemCount,
            Outcome: normalizedOutcome,
            DurationMs: Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2));
    }

    public static string NormalizeOutcome(string? outcome)
    {
        var value = string.IsNullOrWhiteSpace(outcome) ? "success" : outcome.Trim().ToLowerInvariant();
        return AllowedOutcomes.Contains(value) ? value : "validation_error";
    }
}

public sealed record OrderProcessingResult(
    string OrderId,
    int ItemCount,
    string Outcome,
    double DurationMs);
