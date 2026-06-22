using StranglerFigSample.Services;
using Xunit;

namespace StranglerFigSample.Tests;

public class StranglerRouterTests
{
    [Fact]
    public async Task UsesLegacyHandlerForLegacyOnlyFeature()
    {
        var router = CreateRouter(RouteTarget.LegacyOnly);

        var result = await router.ExecuteAsync(
            featureName: "CustomerPage",
            modern: _ => Task.FromResult("modern"),
            legacy: _ => Task.FromResult("legacy"),
            CancellationToken.None);

        Assert.False(result.FallbackUsed);
        Assert.Equal("legacy", result.Result);
    }

    [Fact]
    public async Task UsesModernHandlerForModernOnlyFeature()
    {
        var router = CreateRouter(RouteTarget.ModernOnly);

        var result = await router.ExecuteAsync(
            featureName: "CustomerPage",
            modern: _ => Task.FromResult("modern"),
            legacy: _ => Task.FromResult("legacy"),
            CancellationToken.None);

        Assert.False(result.FallbackUsed);
        Assert.Equal("modern", result.Result);
    }

    [Fact]
    public async Task FallsBackToLegacyWhenModernFails()
    {
        var router = CreateRouter(RouteTarget.ModernWithFallback);

        var result = await router.ExecuteAsync(
            featureName: "CustomerPage",
            modern: _ => throw new InvalidOperationException("boom"),
            legacy: _ => Task.FromResult("legacy"),
            CancellationToken.None);

        Assert.True(result.FallbackUsed);
        Assert.Equal("legacy", result.Result);
        Assert.Equal("boom", result.ModernFailure);
    }

    private static StranglerRouter CreateRouter(RouteTarget target)
    {
        var registry = new FeatureRoutingRegistry(new Dictionary<string, RouteTarget>
        {
            ["CustomerPage"] = target
        });

        return new StranglerRouter(
            registry,
            new LegacyCustomerPageHandler(),
            new ModernCustomerPageHandler(),
            new LegacyExportHandler(),
            new ModernOrdersHandler());
    }
}
