using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace WebDavMigrationSample.Services;

public sealed class DavModule
{
    private readonly string rootPath;

    public DavModule(IWebHostEnvironment environment)
    {
        rootPath = Path.Combine(environment.ContentRootPath, "Storage");
        Directory.CreateDirectory(rootPath);
    }

    public async Task<IResult> ProcessRequestAsync(HttpContext context, string path)
    {
        if (!string.IsNullOrEmpty(context.Request.Query["_MWDRes"]))
        {
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }

        return context.Request.Method.ToUpperInvariant() switch
        {
            "PROPFIND" => await HandlePropFindAsync(path),
            "PUT" => await HandlePutAsync(context, path),
            "GET" => await HandleGetAsync(path),
            "OPTIONS" => HandleOptions(),
            _ => Results.StatusCode(StatusCodes.Status501NotImplemented)
        };
    }

    private async Task<IResult> HandlePropFindAsync(string path)
    {
        var filePath = GetFullPath(path);

        if (!File.Exists(filePath))
        {
            return Results.NotFound();
        }

        var fileInfo = new FileInfo(filePath);
        var href = $"/webdav/{path}".Replace("\\", "/");

        var xml = $"""
                   <?xml version="1.0" encoding="utf-8"?>
                   <D:multistatus xmlns:D="DAV:">
                     <D:response>
                       <D:href>{href}</D:href>
                       <D:propstat>
                         <D:prop>
                           <D:getcontentlength>{fileInfo.Length}</D:getcontentlength>
                           <D:getlastmodified>{fileInfo.LastWriteTimeUtc.ToString("R", CultureInfo.InvariantCulture)}</D:getlastmodified>
                         </D:prop>
                         <D:status>HTTP/1.1 200 OK</D:status>
                       </D:propstat>
                     </D:response>
                   </D:multistatus>
                   """;

        return Results.Content(xml, "text/xml", Encoding.UTF8, StatusCodes.Status207MultiStatus);
    }

    private async Task<IResult> HandlePutAsync(HttpContext context, string path)
    {
        var filePath = GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await using var target = File.Create(filePath);
        await context.Request.Body.CopyToAsync(target);

        return Results.Text("File uploaded successfully", "text/plain", Encoding.UTF8, StatusCodes.Status201Created);
    }

    private async Task<IResult> HandleGetAsync(string path)
    {
        var filePath = GetFullPath(path);

        if (!File.Exists(filePath))
        {
            return Results.NotFound();
        }

        var bytes = await File.ReadAllBytesAsync(filePath);
        return Results.File(bytes, "application/octet-stream", enableRangeProcessing: true);
    }

    private IResult HandleOptions()
    {
        return Results.Ok().WithHeaders(
            new Dictionary<string, string>
            {
                ["Allow"] = "OPTIONS, GET, PUT, PROPFIND",
                ["DAV"] = "1"
            });
    }

    private string GetFullPath(string relativePath)
    {
        var safePath = relativePath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        return Path.Combine(rootPath, safePath);
    }
}
