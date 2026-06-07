using LinuxFileMemorySpikesSample.Services;

var workspaceRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..");
workspaceRoot = Path.GetFullPath(workspaceRoot);

var sampleRoot = Path.Combine(workspaceRoot, "SampleData");
var sourceRoot = Path.Combine(sampleRoot, "source");
var naiveBackupRoot = Path.Combine(sampleRoot, "backup-naive");
var optimizedBackupRoot = Path.Combine(sampleRoot, "backup-optimized");

var generator = new SampleDataGenerator();
var naiveService = new NaiveBackupService();
var optimizedService = new OptimizedBackupService();

if (args.Length == 0)
{
    PrintUsage();
    return;
}

switch (args[0].ToLowerInvariant())
{
    case "generate":
        ResetDirectory(sampleRoot);
        await generator.GenerateAsync(sourceRoot, CancellationToken.None);
        Console.WriteLine($"Generated sample data under {sourceRoot}");
        break;

    case "backup-naive":
        EnsureSampleDataExists(sourceRoot);
        ResetDirectory(naiveBackupRoot);
        Console.WriteLine("Running naive backup...");
        Console.WriteLine(await naiveService.RunAsync(sourceRoot, naiveBackupRoot, CancellationToken.None));
        break;

    case "backup-optimized":
        EnsureSampleDataExists(sourceRoot);
        ResetDirectory(optimizedBackupRoot);
        Console.WriteLine("Running optimized backup...");
        Console.WriteLine(await optimizedService.RunAsync(sourceRoot, optimizedBackupRoot, CancellationToken.None));
        break;

    case "compare":
        ResetDirectory(sampleRoot);
        await generator.GenerateAsync(sourceRoot, CancellationToken.None);

        ResetDirectory(naiveBackupRoot);
        ResetDirectory(optimizedBackupRoot);

        var naiveResult = await naiveService.RunAsync(sourceRoot, naiveBackupRoot, CancellationToken.None);
        var optimizedResult = await optimizedService.RunAsync(sourceRoot, optimizedBackupRoot, CancellationToken.None);

        Console.WriteLine("Naive:");
        Console.WriteLine(naiveResult);
        Console.WriteLine();
        Console.WriteLine("Optimized:");
        Console.WriteLine(optimizedResult);
        break;

    default:
        PrintUsage();
        break;
}

static void ResetDirectory(string path)
{
    if (Directory.Exists(path))
    {
        Directory.Delete(path, recursive: true);
    }

    Directory.CreateDirectory(path);
}

static void EnsureSampleDataExists(string sourceRoot)
{
    if (!Directory.Exists(sourceRoot))
    {
        throw new InvalidOperationException("Sample data not found. Run 'generate' first.");
    }
}

static void PrintUsage()
{
    Console.WriteLine("Commands:");
    Console.WriteLine("  generate");
    Console.WriteLine("  backup-naive");
    Console.WriteLine("  backup-optimized");
    Console.WriteLine("  compare");
}
