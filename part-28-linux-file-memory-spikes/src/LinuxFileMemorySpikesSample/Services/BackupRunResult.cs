namespace LinuxFileMemorySpikesSample.Services;

public sealed record BackupRunResult(
    string Mode,
    int FilesCopied,
    long BytesCopied,
    long PeakManagedMemoryBytes,
    long FinalManagedMemoryBytes,
    double ElapsedMs)
{
    public override string ToString()
    {
        return $"{Mode} | FilesCopied={FilesCopied}, BytesCopied={BytesCopied}, PeakManagedMemoryMB={ToMb(PeakManagedMemoryBytes):N2}, FinalManagedMemoryMB={ToMb(FinalManagedMemoryBytes):N2}, ElapsedMs={ElapsedMs:N2}";
    }

    private static double ToMb(long bytes) => bytes / 1024d / 1024d;
}
