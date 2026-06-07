using PrometheusGrafanaSample.Services;
using Xunit;

namespace PrometheusGrafanaSample.Tests;

public class OrderProcessingServiceTests
{
    [Theory]
    [InlineData("success", "success")]
    [InlineData("SUCCESS", "success")]
    [InlineData("dependency_failure", "dependency_failure")]
    [InlineData("unknown", "validation_error")]
    [InlineData("", "success")]
    public void NormalizeOutcome_ReturnsExpectedValue(string outcome, string expected)
    {
        var actual = OrderProcessingService.NormalizeOutcome(outcome);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task ProcessAsync_ReturnsNormalizedResult()
    {
        var service = new OrderProcessingService(new OrderMetrics());

        var result = await service.ProcessAsync(3, "SUCCESS", 1, CancellationToken.None);

        Assert.Equal(3, result.ItemCount);
        Assert.Equal("success", result.Outcome);
        Assert.NotEmpty(result.OrderId);
        Assert.True(result.DurationMs >= 0);
    }
}
