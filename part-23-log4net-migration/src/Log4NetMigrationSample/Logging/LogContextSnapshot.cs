using log4net;

namespace Log4NetMigrationSample.Logging;

public sealed class LogContextSnapshot
{
    private readonly IReadOnlyDictionary<string, string> _properties;
    private readonly string? _ndcValue;

    private LogContextSnapshot(IReadOnlyDictionary<string, string> properties, string? ndcValue)
    {
        _properties = properties;
        _ndcValue = ndcValue;
    }

    public static LogContextSnapshot Capture(params string[] propertyNames)
    {
        var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var propertyName in propertyNames)
        {
            var value = log4net.LogicalThreadContext.Properties[propertyName];
            if (value is not null)
            {
                properties[propertyName] = value.ToString() ?? string.Empty;
            }
        }

        var ndcValue = log4net.LogicalThreadContext.Stacks["NDC"].ToString();

        return new LogContextSnapshot(properties, string.IsNullOrWhiteSpace(ndcValue) ? null : ndcValue);
    }

    public IDisposable Restore()
    {
        var previousValues = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var previousNdcValue = log4net.LogicalThreadContext.Stacks["NDC"].ToString();

        foreach (var property in _properties)
        {
            previousValues[property.Key] = log4net.LogicalThreadContext.Properties[property.Key];
            log4net.LogicalThreadContext.Properties[property.Key] = property.Value;
        }

        IDisposable? ndcScope = null;
        log4net.LogicalThreadContext.Stacks["NDC"].Clear();
        if (!string.IsNullOrWhiteSpace(_ndcValue))
        {
            ndcScope = log4net.LogicalThreadContext.Stacks["NDC"].Push(_ndcValue);
        }

        return new RestoreScope(previousValues, previousNdcValue, ndcScope);
    }

    private sealed class RestoreScope : IDisposable
    {
        private readonly IReadOnlyDictionary<string, object?> _previousValues;
        private readonly string? _previousNdcValue;
        private readonly IDisposable? _ndcScope;

        public RestoreScope(IReadOnlyDictionary<string, object?> previousValues, string? previousNdcValue, IDisposable? ndcScope)
        {
            _previousValues = previousValues;
            _previousNdcValue = previousNdcValue;
            _ndcScope = ndcScope;
        }

        public void Dispose()
        {
            _ndcScope?.Dispose();
            log4net.LogicalThreadContext.Stacks["NDC"].Clear();
            if (!string.IsNullOrWhiteSpace(_previousNdcValue))
            {
                log4net.LogicalThreadContext.Stacks["NDC"].Push(_previousNdcValue);
            }

            foreach (var property in _previousValues)
            {
                log4net.LogicalThreadContext.Properties[property.Key] = property.Value;
            }
        }
    }
}
