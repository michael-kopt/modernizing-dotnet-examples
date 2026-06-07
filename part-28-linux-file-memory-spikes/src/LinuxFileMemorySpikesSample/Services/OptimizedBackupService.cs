using System.Collections.Concurrent;
using System.Diagnostics;

namespace LinuxFileMemorySpikesSample.Services;

public sealed class OptimizedBackupService
{
    private static readonly HashSet<string> ExcludedFolders = new(StringComparer.OrdinalIgnoreCase)
    {
        "PromptMetadata",
        "TempCache",
        "RebuildableData"
    };

    public async Task<BackupRunResult> RunAsync(string sourceRoot, string destinationRoot, CancellationToken cancellationToken)
    {
        var filesCopied = 0;
        long bytesCopied = 0;
        long peakManagedMemory = GC.GetTotalMemory(forceFullCollection: true);

        var filePairs = new ConcurrentBag<(string Source, string Destination)>();
        CollectFiles(sourceRoot, destinationRoot, filePairs);

        var stopwatch = Stopwatch.StartNew();

        await Parallel.ForEachAsync(
            filePairs,
            new ParallelOptions
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2)
            },
            async (pair, ct) =>
            {
                Directory.CreateDirectory(Path.GetDirectoryName(pair.Destination)!);

                await using var sourceStream = File.OpenRead(pair.Source);
                await using var destinationStream = File.Create(pair.Destination);
                await sourceStream.CopyToAsync(destinationStream, 81920, ct);

                Interlocked.Increment(ref filesCopied);
                Interlocked.Add(ref bytesCopied, sourceStream.Length);
                UpdatePeakManagedMemory(ref peakManagedMemory);
            });

        stopwatch.Stop();

        var finalManagedMemory = GC.GetTotalMemory(forceFullCollection: true);

        return new BackupRunResult(
            Mode: "optimized",
            FilesCopied: filesCopied,
            BytesCopied: bytesCopied,
            PeakManagedMemoryBytes: peakManagedMemory,
            FinalManagedMemoryBytes: finalManagedMemory,
            ElapsedMs: Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2));
    }

    private static void CollectFiles(string source, string destination, ConcurrentBag<(string Source, string Destination)> filePairs)
    {
        Directory.CreateDirectory(destination);

        foreach (var file in Directory.GetFiles(source))
        {
            filePairs.Add((file, Path.Combine(destination, Path.GetFileName(file))));
        }

        foreach (var directory in Directory.GetDirectories(source))
        {
            var name = Path.GetFileName(directory);
            if (ExcludedFolders.Contains(name))
            {
                continue;
            }

            CollectFiles(directory, Path.Combine(destination, name), filePairs);
        }
    }

    private static void UpdatePeakManagedMemory(ref long peakManagedMemory)
    {
        long current;
        do
        {
            current = peakManagedMemory;
            var observed = GC.GetTotalMemory(forceFullCollection: false);
            if (observed <= current)
            {
                return;
            }
        }
        while (Interlocked.CompareExchange(ref peakManagedMemory, GC.GetTotalMemory(forceFullCollection: false), current) != current);
    }
}
