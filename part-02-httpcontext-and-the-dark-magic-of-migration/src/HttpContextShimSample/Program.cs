using Microsoft.AspNetCore.Http;
using Part02.Legacy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

System.Web.HttpContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

app.MapGet("/", () => Results.Redirect("/legacy"));

app.MapGet("/legacy", () =>
{
    var snapshot = LegacyRequestReader.Read();
    return Results.Json(snapshot);
});

app.MapGet("/legacy/async", async () =>
{
    await Task.Delay(25);
    var snapshot = await LegacyRequestReader.ReadAfterAwaitAsync();
    return Results.Json(snapshot);
});

app.Run("http://localhost:5090");
