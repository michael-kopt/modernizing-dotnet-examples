# Part 24: Modern Monitoring with Prometheus and Grafana

Article: [Modernizing .NET - Part 24 Modern Monitoring with Prometheus and Grafana](https://medium.com/@michael.kopt/modernizing-net-part-24-modern-monitoring-with-prometheus-and-grafana-3cbfa8a09da0)

This sample demonstrates the migration pattern from the article: expose modern ASP.NET Core and .NET runtime metrics through OpenTelemetry, scrape them with Prometheus, and add one custom business meter so Grafana can show application behavior and not just infrastructure health.

## Sample Focus

- OpenTelemetry metrics configured in `Program.cs`.
- Prometheus scraping endpoint at `/metrics`.
- Built-in instrumentation for ASP.NET Core, `HttpClient`, and .NET runtime metrics.
- A custom meter for order processing throughput and duration.
- Example Prometheus scrape config and Grafana queries.

## Implementation Notes

- `AddAspNetCoreInstrumentation()` emits inbound HTTP metrics.
- `AddHttpClientInstrumentation()` emits outbound HTTP metrics.
- `AddRuntimeInstrumentation()` emits GC, memory, and thread-pool metrics.
- `OrderMetrics` adds a custom counter and histogram for domain-level visibility.
- `/orders/process` generates custom metrics and `/downstream/check` triggers an instrumented `HttpClient` call.
- The custom metrics label only by `outcome` to avoid turning request-specific values into high-cardinality Prometheus labels.

## Structure

```text
src/PrometheusGrafanaSample
tests/PrometheusGrafanaSample.Tests
observability/prometheus.yml
```

## Run

```powershell
dotnet run --project .\src\PrometheusGrafanaSample
```

The sample listens on `http://localhost:5890`.

## Try It

Generate custom business metrics:

```powershell
Invoke-RestMethod "http://localhost:5890/orders/process?itemCount=3&outcome=success&delayMs=40"
```

Generate outbound HTTP client metrics:

```powershell
Invoke-RestMethod "http://localhost:5890/downstream/check?calls=2"
```

Scrape Prometheus metrics:

```powershell
Invoke-WebRequest http://localhost:5890/metrics
```

## Example Metrics To Look For

- `http_server_request_duration_seconds`
- `http_client_request_duration_seconds`
- `process_runtime_dotnet_gc_heap_size_bytes`
- `process_runtime_dotnet_thread_pool_queue_length`
- `modernizing_orders_processed_total`
- `modernizing_order_duration_ms_milliseconds`

## Grafana Queries

Request rate:

```promql
rate(http_server_request_duration_seconds_count[5m])
```

P95 request latency:

```promql
histogram_quantile(0.95, rate(http_server_request_duration_seconds_bucket[5m]))
```

Order throughput:

```promql
sum(rate(modernizing_orders_processed_total[5m])) by (outcome)
```

Order processing P95:

```promql
histogram_quantile(0.95, sum(rate(modernizing_order_duration_ms_milliseconds_bucket[5m])) by (le, outcome))
```

Thread-pool queue depth:

```promql
process_runtime_dotnet_thread_pool_queue_length
```

## Notes

- The Prometheus exporter package is still prerelease, which matches the state of the exporter itself as of June 7, 2026.
- The sample uses Prometheus scraping directly because that is the clearest migration step from the article. Larger deployments may prefer OTLP export through an OpenTelemetry Collector.
