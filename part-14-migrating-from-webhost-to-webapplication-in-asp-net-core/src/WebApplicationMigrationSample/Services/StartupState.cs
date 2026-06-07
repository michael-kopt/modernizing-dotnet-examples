namespace WebApplicationMigrationSample.Services;

public sealed class StartupState
{
    public bool Initialized { get; private set; }

    public DateTime? InitializedAtUtc { get; private set; }

    public void MarkInitialized()
    {
        Initialized = true;
        InitializedAtUtc = DateTime.UtcNow;
    }
}
