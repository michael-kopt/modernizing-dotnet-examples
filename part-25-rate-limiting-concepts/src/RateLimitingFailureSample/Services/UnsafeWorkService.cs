using System.Diagnostics;
using System.Security.Cryptography;

namespace RateLimitingFailureSample.Services;

public sealed class UnsafeWorkService
{
    private int _activeRequests;

    public async Task<UnsafeRequestResult> ProcessAsync(int workMs, int memoryKb, CancellationToken cancellationToken)
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
            // Simulate per-request memory pressure.
            var buffer = GC.AllocateUninitializedArray<byte>(memoryKb * 1024);
            RandomNumberGenerator.Fill(buffer);

            // Simulate CPU-heavy work that becomes painful at high concurrency.
            BusyCpu(workMs / 2);

            // Simulate additional wait on downstream resources.
            await Task.Delay(workMs / 2, cancellationToken);

            stopwatch.Stop();

            return new UnsafeRequestResult(
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

public sealed record UnsafeRequestResult(
    double ElapsedMs,
    int ActiveRequestsObserved,
    int AllocatedKb);
