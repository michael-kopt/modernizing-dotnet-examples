using RateLimitingMiddlewareSample.Services;
using Xunit;

namespace RateLimitingMiddlewareSample.Tests;

public class RateLimiterSettingsTests
{
    [Theory]
    [InlineData(null, 4)]
    [InlineData("", 4)]
    [InlineData("-1", 4)]
    [InlineData("8", 8)]
    public void ParsePositiveInt_ReturnsExpectedValue(string? rawValue, int expected)
    {
        Environment.SetEnvironmentVariable("RateLimiterPermitLimit", rawValue);

        var actual = RateLimiterSettings.ParsePositiveInt("RateLimiterPermitLimit", 4);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null, 4)]
    [InlineData("", 4)]
    [InlineData("-1", 4)]
    [InlineData("0", 0)]
    [InlineData("6", 6)]
    public void ParseNonNegativeInt_ReturnsExpectedValue(string? rawValue, int expected)
    {
        Environment.SetEnvironmentVariable("RateLimiterQueueLimit", rawValue);

        var actual = RateLimiterSettings.ParseNonNegativeInt("RateLimiterQueueLimit", 4);

        Assert.Equal(expected, actual);
    }
}
