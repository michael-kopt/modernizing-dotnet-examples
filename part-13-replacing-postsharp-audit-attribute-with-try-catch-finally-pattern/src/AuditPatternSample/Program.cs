using AuditPatternSample.Models;
using AuditPatternSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AuditQueue>();
builder.Services.AddSingleton<BusinessOperationStore>();
builder.Services.AddSingleton<LegacyBusinessService>();

var app = builder.Build();

app.MapPost("/audit/reset", (AuditQueue queue, BusinessOperationStore store) =>
{
    queue.Clear();
    store.Clear();
    return Results.NoContent();
});

app.MapGet("/audit/entries", (AuditQueue queue) => Results.Ok(queue.GetAll()));
app.MapGet("/operations/results", (BusinessOperationStore store) => Results.Ok(store.GetAll()));

app.MapPost("/operations/process", (HttpRequest request, ProcessRequest body, LegacyBusinessService service) =>
{
    var result = service.ProcessData(request, body.Input);
    return Results.Ok(new { result });
});

app.MapPost("/operations/create-item", (HttpRequest request, CreateItemRequest body, bool authorized, LegacyBusinessService service) =>
{
    var created = service.CreateItem(request, body.Name, authorized);
    return Results.Ok(new { created });
});

app.MapPost("/operations/background", (HttpRequest request, ProcessRequest body, LegacyBusinessService service) =>
{
    service.RunInBackground(request, body.Input);
    return Results.Accepted("/audit/entries");
});

app.Run("http://localhost:5881");
