using System.Diagnostics;

namespace LinuxFileMemorySpikesSample.Services;

public sealed class NaiveBackupService
{
    public async Task<BackupRunResult> RunAsync(string sourceRoot, string destinationRoot, CancellationToken cancellationToken)
    {
        var filesCopied = 0;
        long bytesCopied = 0;
        long peakManagedMemory = GC.GetTotalMemory(forceFullCollection: true);

        var stopwatch = Stopwatch.StartNew();
        await CopyDirectoryAsync(sourceRoot, destinationRoot, cancellationToken);
        stopwatch.Stop();

        var finalManagedMemory = GC.GetTotalMemory(forceFullCollection: true);

        return new BackupRunResult(
            Mode: "naive",
            FilesCopied: filesCopied,
            BytesCopied: bytesCopied,
            PeakManagedMemoryBytes: peakManagedMemory,
            FinalManagedMemoryBytes: finalManagedMemory,
            ElapsedMs: Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2));

        async Task CopyDirectoryAsync(string source, string destination, CancellationToken ct)
        {
            Directory.CreateDirectory(destination);

            foreach (var file in Directory.GetFiles(source))
            {
                var content = await File.ReadAllBytesAsync(file, ct);
                var destinationFile = Path.Combine(destination, Path.GetFileName(file));
                await File.WriteAllBytesAsync(destinationFile, content, ct);

                filesCopied++;
                bytesCopied += content.Length;
                peakManagedMemory = Math.Max(peakManagedMemory, GC.GetTotalMemory(forceFullCollection: false));
            }

            foreach (var directory in Directory.GetDirectories(source))
            {
                var destinationDirectory = Path.Combine(destination, Path.GetFileName(directory));
                await CopyDirectoryAsync(directory, destinationDirectory, ct);
            }
        }
    }
}
