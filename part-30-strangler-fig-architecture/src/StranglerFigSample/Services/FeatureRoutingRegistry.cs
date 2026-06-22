namespace StranglerFigSample.Services;

public sealed class FeatureRoutingRegistry
{
    private readonly IReadOnlyDictionary<string, RouteTarget> _featureRoutes;

    public FeatureRoutingRegistry(IReadOnlyDictionary<string, RouteTarget> featureRoutes)
    {
        _featureRoutes = featureRoutes;
    }

    public RouteTarget GetRouteTarget(string featureName)
    {
        if (!_featureRoutes.TryGetValue(featureName, out var target))
        {
            throw new KeyNotFoundException($"No route target configured for feature '{featureName}'.");
        }

        return target;
    }

    public IReadOnlyDictionary<string, string> GetAll()
    {
        return _featureRoutes.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.ToString(),
            StringComparer.OrdinalIgnoreCase);
    }
}
