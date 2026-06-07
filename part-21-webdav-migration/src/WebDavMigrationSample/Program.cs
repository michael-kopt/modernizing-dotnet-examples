using Microsoft.AspNetCore.Http.Features;
using WebDavMigrationSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});

builder.Services.AddSingleton<DavModule>();

var app = builder.Build();

app.MapMethods("/webdav/{**path}", ["GET", "PUT", "OPTIONS", "PROPFIND"], async (HttpContext context, string? path, DavModule davModule) =>
{
    return await davModule.ProcessRequestAsync(context, path ?? string.Empty);
}).DisableAntiforgery();

app.Run("http://localhost:5887");
