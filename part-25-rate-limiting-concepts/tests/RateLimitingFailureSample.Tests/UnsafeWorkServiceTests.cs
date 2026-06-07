using RateLimitingFailureSample.Services;
using Xunit;

namespace RateLimitingFailureSample.Tests;

public class UnsafeWorkServiceTests
{
    [Fact]
    public async Task ProcessAsync_ReturnsElapsedTimeAndAllocation()
    {
        var service = new UnsafeWorkService();

        var result = await service.ProcessAsync(20, 64, CancellationToken.None);

        Assert.True(result.ElapsedMs > 0);
        Assert.Equal(64, result.AllocatedKb);
        Assert.True(result.ActiveRequestsObserved >= 1);
    }

    [Theory]
    [InlineData(0, 64)]
    [InlineData(20, 0)]
    public async Task ProcessAsync_RejectsInvalidArguments(int workMs, int memoryKb)
    {
        var service = new UnsafeWorkService();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.ProcessAsync(workMs, memoryKb, CancellationToken.None));
    }
}
