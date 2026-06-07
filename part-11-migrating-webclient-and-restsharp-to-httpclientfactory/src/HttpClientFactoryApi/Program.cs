var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/text", (HttpContext context) =>
{
    var clientName = context.Request.Headers["X-Client-Name"].ToString();
    return Results.Text($"hello from api; client={clientName}");
});

app.MapGet("/bytes", () => Results.Bytes(new byte[] { 1, 2, 3, 4, 5 }, "application/octet-stream"));

app.MapPost("/echo", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    return Results.Json(new
    {
        body,
        contentType = context.Request.ContentType,
        clientName = context.Request.Headers["X-Client-Name"].ToString()
    });
});

app.MapGet("/cookie-check", (HttpContext context) =>
{
    return Results.Json(new
    {
        sessionId = context.Request.Cookies["session-id"]
    });
});

app.MapGet("/redirect", () => Results.Redirect("/text"));

app.Run("http://localhost:5780");
