using StranglerFigSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new FeatureRoutingRegistry(new Dictionary<string, RouteTarget>(StringComparer.OrdinalIgnoreCase)
{
    ["CustomerPage"] = RouteTarget.ModernWithFallback,
    ["LegacyExport"] = RouteTarget.LegacyOnly,
    ["OrdersApi"] = RouteTarget.ModernOnly
}));
builder.Services.AddSingleton<StranglerRouter>();
builder.Services.AddSingleton<LegacyCustomerPageHandler>();
builder.Services.AddSingleton<ModernCustomerPageHandler>();
builder.Services.AddSingleton<LegacyExportHandler>();
builder.Services.AddSingleton<ModernOrdersHandler>();

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/feature-map"));

app.MapGet("/feature-map", (FeatureRoutingRegistry registry) =>
{
    return Results.Json(registry.GetAll());
});

app.MapGet("/Customer.aspx", async (int customerId, bool failModern, StranglerRouter router, CancellationToken cancellationToken) =>
{
    var result = await router.ExecuteAsync(
        featureName: "CustomerPage",
        modern: ct => router.ModernCustomerPageHandler.HandleAsync(customerId, failModern, ct),
        legacy: ct => router.LegacyCustomerPageHandler.HandleAsync(customerId, ct),
        cancellationToken);

    return Results.Json(result);
});

app.MapPost("/Customer.aspx", async (CustomerPagePostRequest request, StranglerRouter router, CancellationToken cancellationToken) =>
{
    var result = await router.ExecuteAsync(
        featureName: "CustomerPage",
        modern: ct => router.ModernCustomerPageHandler.HandlePostAsync(request, ct),
        legacy: ct => router.LegacyCustomerPageHandler.HandlePostAsync(request, ct),
        cancellationToken);

    return Results.Json(result);
});

app.MapGet("/Export.ashx", async (string reportId, StranglerRouter router, CancellationToken cancellationToken) =>
{
    var result = await router.ExecuteAsync(
        featureName: "LegacyExport",
        modern: _ => throw new InvalidOperationException("Export is not migrated yet."),
        legacy: ct => router.LegacyExportHandler.HandleAsync(reportId, ct),
        cancellationToken);

    return Results.Json(result);
});

app.MapGet("/api/orders/{orderId:int}", async (int orderId, StranglerRouter router, CancellationToken cancellationToken) =>
{
    var result = await router.ExecuteAsync(
        featureName: "OrdersApi",
        modern: ct => router.ModernOrdersHandler.HandleAsync(orderId, ct),
        legacy: _ => throw new InvalidOperationException("Orders API has no legacy handler."),
        cancellationToken);

    return Results.Json(result);
});

app.Run("http://localhost:5894");
