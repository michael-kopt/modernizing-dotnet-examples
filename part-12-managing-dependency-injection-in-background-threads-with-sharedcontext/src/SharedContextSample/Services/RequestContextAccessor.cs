namespace SharedContextSample.Services;

public static class RequestContextAccessor
{
    public static ThreadLocal<IServiceProvider?> Current { get; } = new(() => null);
}
