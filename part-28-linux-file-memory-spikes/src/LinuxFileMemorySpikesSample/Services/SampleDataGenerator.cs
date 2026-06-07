using System.Security.Cryptography;

namespace LinuxFileMemorySpikesSample.Services;

public sealed class SampleDataGenerator
{
    public async Task GenerateAsync(string sourceRoot, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(sourceRoot);

        await CreateFolderAsync(Path.Combine(sourceRoot, "BusinessData"), 400, 8 * 1024, cancellationToken);
        await CreateFolderAsync(Path.Combine(sourceRoot, "PromptMetadata"), 300, 6 * 1024, cancellationToken);
        await CreateFolderAsync(Path.Combine(sourceRoot, "TempCache"), 300, 6 * 1024, cancellationToken);
        await CreateFolderAsync(Path.Combine(sourceRoot, "RebuildableData"), 300, 6 * 1024, cancellationToken);
        await CreateFolderAsync(Path.Combine(sourceRoot, "BusinessData", "Nested"), 250, 4 * 1024, cancellationToken);

        await CreateLargeFileAsync(Path.Combine(sourceRoot, "BusinessData", "catalog.bin"), 2 * 1024 * 1024, cancellationToken);
        await CreateLargeFileAsync(Path.Combine(sourceRoot, "PromptMetadata", "snapshot.bin"), 2 * 1024 * 1024, cancellationToken);
    }

    private static async Task CreateFolderAsync(string folder, int fileCount, int bytesPerFile, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(folder);

        for (var index = 0; index < fileCount; index++)
        {
            var path = Path.Combine(folder, $"file-{index:D4}.json");
            var buffer = GC.AllocateUninitializedArray<byte>(bytesPerFile);
            RandomNumberGenerator.Fill(buffer);
            await File.WriteAllBytesAsync(path, buffer, cancellationToken);
        }
    }

    private static async Task CreateLargeFileAsync(string path, int bytes, CancellationToken cancellationToken)
    {
        var buffer = GC.AllocateUninitializedArray<byte>(bytes);
        RandomNumberGenerator.Fill(buffer);
        await File.WriteAllBytesAsync(path, buffer, cancellationToken);
    }
}
