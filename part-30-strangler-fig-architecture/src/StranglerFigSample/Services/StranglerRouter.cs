namespace StranglerFigSample.Services;

public sealed class StranglerRouter
{
    private readonly FeatureRoutingRegistry _registry;

    public StranglerRouter(
        FeatureRoutingRegistry registry,
        LegacyCustomerPageHandler legacyCustomerPageHandler,
        ModernCustomerPageHandler modernCustomerPageHandler,
        LegacyExportHandler legacyExportHandler,
        ModernOrdersHandler modernOrdersHandler)
    {
        _registry = registry;
        LegacyCustomerPageHandler = legacyCustomerPageHandler;
        ModernCustomerPageHandler = modernCustomerPageHandler;
        LegacyExportHandler = legacyExportHandler;
        ModernOrdersHandler = modernOrdersHandler;
    }

    public LegacyCustomerPageHandler LegacyCustomerPageHandler { get; }

    public ModernCustomerPageHandler ModernCustomerPageHandler { get; }

    public LegacyExportHandler LegacyExportHandler { get; }

    public ModernOrdersHandler ModernOrdersHandler { get; }

    public async Task<StranglerExecutionResult<T>> ExecuteAsync<T>(
        string featureName,
        Func<CancellationToken, Task<T>> modern,
        Func<CancellationToken, Task<T>> legacy,
        CancellationToken cancellationToken)
    {
        var routeTarget = _registry.GetRouteTarget(featureName);

        switch (routeTarget)
        {
            case RouteTarget.LegacyOnly:
                return new StranglerExecutionResult<T>(
                    Feature: featureName,
                    Target: routeTarget.ToString(),
                    FallbackUsed: false,
                    Result: await legacy(cancellationToken));

            case RouteTarget.ModernOnly:
                return new StranglerExecutionResult<T>(
                    Feature: featureName,
                    Target: routeTarget.ToString(),
                    FallbackUsed: false,
                    Result: await modern(cancellationToken));

            case RouteTarget.ModernWithFallback:
                try
                {
                    return new StranglerExecutionResult<T>(
                        Feature: featureName,
                        Target: routeTarget.ToString(),
                        FallbackUsed: false,
                        Result: await modern(cancellationToken));
                }
                catch (Exception ex)
                {
                    return new StranglerExecutionResult<T>(
                        Feature: featureName,
                        Target: routeTarget.ToString(),
                        FallbackUsed: true,
                        Result: await legacy(cancellationToken),
                        ModernFailure: ex.Message);
                }

            default:
                throw new ArgumentOutOfRangeException(nameof(routeTarget), routeTarget, "Unknown route target.");
        }
    }
}

public sealed record StranglerExecutionResult<T>(
    string Feature,
    string Target,
    bool FallbackUsed,
    T Result,
    string? ModernFailure = null);
