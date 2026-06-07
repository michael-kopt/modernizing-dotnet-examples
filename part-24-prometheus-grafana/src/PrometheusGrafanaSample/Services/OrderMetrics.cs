using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace PrometheusGrafanaSample.Services;

public sealed class OrderMetrics
{
    public const string MeterName = "ModernizingDotNet.Metrics";

    private readonly Counter<long> _ordersProcessed;
    private readonly Histogram<double> _orderDurationMs;
    private readonly UpDownCounter<long> _activeOrders;

    public OrderMetrics()
    {
        var meter = new Meter(MeterName);
        _ordersProcessed = meter.CreateCounter<long>("modernizing_orders_processed", unit: "{orders}");
        _orderDurationMs = meter.CreateHistogram<double>("modernizing_order_duration_ms", unit: "ms");
        _activeOrders = meter.CreateUpDownCounter<long>("modernizing_active_orders", unit: "{orders}");
    }

    public IDisposable TrackActiveOrder()
    {
        _activeOrders.Add(1);
        return new ActiveOrderScope(_activeOrders);
    }

    public void RecordProcessedOrder(int itemCount, string outcome, double durationMs)
    {
        var tags = new TagList
        {
            { "outcome", outcome }
        };

        _ordersProcessed.Add(1, tags);
        _orderDurationMs.Record(durationMs, tags);
    }

    private sealed class ActiveOrderScope : IDisposable
    {
        private readonly UpDownCounter<long> _counter;

        public ActiveOrderScope(UpDownCounter<long> counter)
        {
            _counter = counter;
        }

        public void Dispose()
        {
            _counter.Add(-1);
        }
    }
}
