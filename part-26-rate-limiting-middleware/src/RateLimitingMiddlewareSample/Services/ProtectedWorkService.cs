using System.Diagnostics;
using System.Security.Cryptography;

namespace RateLimitingMiddlewareSample.Services;

public sealed class ProtectedWorkService
{
    private int _activeRequests;

    public async Task<ProtectedRequestResult> ProcessAsync(int workMs, int memoryKb, CancellationToken cancellationToken)
    {
        if (workMs <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(workMs), "workMs must be greater than zero.");
        }

        if (memoryKb <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(memoryKb), "memoryKb must be greater than zero.");
        }

        var activeRequestsObserved = Interlocked.Increment(ref _activeRequests);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var buffer = GC.AllocateUninitializedArray<byte>(memoryKb * 1024);
            RandomNumberGenerator.Fill(buffer);

            BusyCpu(workMs / 2);
            await Task.Delay(workMs / 2, cancellationToken);

            stopwatch.Stop();

            return new ProtectedRequestResult(
                ElapsedMs: Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2),
                ActiveRequestsObserved: activeRequestsObserved,
                AllocatedKb: memoryKb);
        }
        finally
        {
            Interlocked.Decrement(ref _activeRequests);
        }
    }

    private static void BusyCpu(int durationMs)
    {
        var stopwatch = Stopwatch.StartNew();
        double accumulator = 0;

        while (stopwatch.ElapsedMilliseconds < durationMs)
        {
            for (var index = 1; index < 5000; index++)
            {
                accumulator += Math.Sqrt(index) * Math.Cos(index);
            }
        }

        GC.KeepAlive(accumulator);
    }
}

public sealed record ProtectedRequestResult(
    double ElapsedMs,
    int ActiveRequestsObserved,
    int AllocatedKb);
