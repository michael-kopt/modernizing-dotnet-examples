using Microsoft.AspNetCore.Http;
using SystemWebWrappersSample.Legacy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

System.Web.HttpContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

app.MapGet("/", () => Results.Redirect("/legacy/request?id=42&format=json"));

app.MapMethods("/legacy/request", new[] { "GET", "POST" }, async context =>
{
    var snapshot = await LegacyRequestReader.ReadAsync();
    await context.Response.WriteAsJsonAsync(snapshot);
});

app.MapGet("/legacy/export", async context =>
{
    var result = await LegacyResponseWriter.WriteExportAsync();
    if (!context.Response.HasStarted)
    {
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.Run("http://localhost:5893");
