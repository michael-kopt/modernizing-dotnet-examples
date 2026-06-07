namespace RateLimitingMiddlewareSample.Services;

public sealed record RateLimiterSettings(int PermitLimit, int QueueLimit)
{
    public static RateLimiterSettings FromEnvironment()
    {
        return new RateLimiterSettings(
            PermitLimit: ParsePositiveInt("RateLimiterPermitLimit", 4),
            QueueLimit: ParseNonNegativeInt("RateLimiterQueueLimit", 4));
    }

    public static int ParsePositiveInt(string variableName, int defaultValue)
    {
        if (!int.TryParse(Environment.GetEnvironmentVariable(variableName), out var value) || value <= 0)
        {
            return defaultValue;
        }

        return value;
    }

    public static int ParseNonNegativeInt(string variableName, int defaultValue)
    {
        if (!int.TryParse(Environment.GetEnvironmentVariable(variableName), out var value) || value < 0)
        {
            return defaultValue;
        }

        return value;
    }
}
