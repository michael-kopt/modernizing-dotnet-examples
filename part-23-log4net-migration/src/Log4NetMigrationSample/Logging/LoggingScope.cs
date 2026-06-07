using log4net;
using log4net.Util;

namespace Log4NetMigrationSample.Logging;

public sealed class LoggingScope : IDisposable
{
    private readonly IDisposable _ndcScope;

    private LoggingScope(IDisposable ndcScope, IReadOnlyDictionary<string, object?> previousValues)
    {
        _ndcScope = ndcScope;
        PreviousValues = previousValues;
    }

    private IReadOnlyDictionary<string, object?> PreviousValues { get; }

    public static LoggingScope Begin(string userId, string tenantId, string sessionId, string? operationId = null)
    {
        log4net.LogicalThreadContext.Stacks["NDC"].Clear();

        var previousValues = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["UserId"] = log4net.LogicalThreadContext.Properties["UserId"],
            ["TenantId"] = log4net.LogicalThreadContext.Properties["TenantId"],
            ["SessionId"] = log4net.LogicalThreadContext.Properties["SessionId"],
            ["OperationId"] = log4net.LogicalThreadContext.Properties["OperationId"],
            ["EventTime"] = log4net.LogicalThreadContext.Properties["EventTime"]
        };

        var ndcValue = string.IsNullOrWhiteSpace(operationId)
            ? $"{userId},{tenantId}"
            : $"{userId},{tenantId},{operationId}";

        IDisposable ndcScope = log4net.LogicalThreadContext.Stacks["NDC"].Push(ndcValue);

        log4net.LogicalThreadContext.Properties["UserId"] = userId;
        log4net.LogicalThreadContext.Properties["TenantId"] = tenantId;
        log4net.LogicalThreadContext.Properties["SessionId"] = sessionId;
        log4net.LogicalThreadContext.Properties["EventTime"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

        if (!string.IsNullOrWhiteSpace(operationId))
        {
            log4net.LogicalThreadContext.Properties["OperationId"] = operationId;
        }

        return new LoggingScope(ndcScope, previousValues);
    }

    public void Dispose()
    {
        _ndcScope.Dispose();
        log4net.LogicalThreadContext.Stacks["NDC"].Clear();

        foreach (var property in PreviousValues)
        {
            log4net.LogicalThreadContext.Properties[property.Key] = property.Value;
        }
    }
}
