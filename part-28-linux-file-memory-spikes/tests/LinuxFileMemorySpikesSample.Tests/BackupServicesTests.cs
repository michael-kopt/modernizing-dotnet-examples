using LinuxFileMemorySpikesSample.Services;
using Xunit;

namespace LinuxFileMemorySpikesSample.Tests;

public class BackupServicesTests
{
    [Fact]
    public async Task OptimizedBackup_ExcludesRebuildableFolders()
    {
        var root = Path.Combine(Path.GetTempPath(), "linux-file-spikes-tests", Guid.NewGuid().ToString("n"));
        var source = Path.Combine(root, "source");
        var destination = Path.Combine(root, "destination");

        try
        {
            Directory.CreateDirectory(Path.Combine(source, "BusinessData"));
            Directory.CreateDirectory(Path.Combine(source, "PromptMetadata"));
            await File.WriteAllTextAsync(Path.Combine(source, "BusinessData", "keep.txt"), "keep");
            await File.WriteAllTextAsync(Path.Combine(source, "PromptMetadata", "skip.txt"), "skip");

            var service = new OptimizedBackupService();
            await service.RunAsync(source, destination, CancellationToken.None);

            Assert.True(File.Exists(Path.Combine(destination, "BusinessData", "keep.txt")));
            Assert.False(File.Exists(Path.Combine(destination, "PromptMetadata", "skip.txt")));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [Fact]
    public async Task NaiveBackup_CopiesAllFolders()
    {
        var root = Path.Combine(Path.GetTempPath(), "linux-file-spikes-tests", Guid.NewGuid().ToString("n"));
        var source = Path.Combine(root, "source");
        var destination = Path.Combine(root, "destination");

        try
        {
            Directory.CreateDirectory(Path.Combine(source, "BusinessData"));
            Directory.CreateDirectory(Path.Combine(source, "PromptMetadata"));
            await File.WriteAllTextAsync(Path.Combine(source, "BusinessData", "keep.txt"), "keep");
            await File.WriteAllTextAsync(Path.Combine(source, "PromptMetadata", "copy.txt"), "copy");

            var service = new NaiveBackupService();
            await service.RunAsync(source, destination, CancellationToken.None);

            Assert.True(File.Exists(Path.Combine(destination, "BusinessData", "keep.txt")));
            Assert.True(File.Exists(Path.Combine(destination, "PromptMetadata", "copy.txt")));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }
}
