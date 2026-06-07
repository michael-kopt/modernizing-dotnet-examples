using Microsoft.AspNetCore.Mvc;
using AspxMigrationSample.Models;
using AspxMigrationSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PageService>();

var app = builder.Build();

app.MapGet("/Page.aspx", async context =>
{
    var htmlPath = Path.Combine(app.Environment.ContentRootPath, "Static", "Page.html");
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(htmlPath);
});

app.MapPost("/Page.aspx", async (HttpContext context, PageService pageService) =>
{
    var form = await context.Request.ReadFormAsync();
    var pageRequest = new PageRequest(
        context.Request.Query.ContainsKey("lite"),
        form["__EVENTTARGET"].ToString(),
        form["__EVENTARGUMENT"].ToString(),
        form);

    return pageRequest.EventTarget switch
    {
        "GetData" => Results.Json(pageService.GetPageData(pageRequest)),
        "GetPage" => Results.File(
            pageService.BuildPageExport(),
            "application/zip",
            "PageExport.zip"),
        "UploadButton" => Results.Text(string.Empty),
        _ => Results.BadRequest(new { message = "Invalid Form" })
    };
});

app.Run("http://localhost:5886");
