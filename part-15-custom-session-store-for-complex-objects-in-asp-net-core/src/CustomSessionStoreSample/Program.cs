using System.Text.Json;
using CustomSessionStoreSample.Models;
using CustomSessionStoreSample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<SessionStore>();
builder.Services.AddSingleton<ISessionStore>(serviceProvider => serviceProvider.GetRequiredService<SessionStore>());
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});

var app = builder.Build();

app.UseSession();

app.MapGet("/session/json-attempt", () =>
{
    var workflow = WorkflowFactory.Create();

    try
    {
        JsonSerializer.Serialize(workflow);
        return Results.Ok(new
        {
            success = true,
            message = "JSON serialization unexpectedly succeeded."
        });
    }
    catch (Exception ex)
    {
        return Results.Ok(new
        {
            success = false,
            error = ex.GetType().Name,
            message = ex.Message
        });
    }
});

app.MapPost("/session/workflow/store", (HttpContext context, SessionStore sessionStore) =>
{
    var workflow = WorkflowFactory.Create();
    var sessionState = new HttpSessionState(context.Session, sessionStore);

    sessionState["CurrentWorkflow"] = workflow;
    context.Session.SetString("workflow-name", workflow.Name);

    return Results.Ok(new
    {
        sessionId = context.Session.Id,
        workflow.Name,
        ownerType = workflow.Owner.GetType().Name,
        reviewerType = workflow.Reviewer.GetType().Name,
        firstOrderBackReference = ReferenceEquals(workflow.Owner, workflow.Orders[0].User)
    });
});

app.MapGet("/session/workflow/current", (HttpContext context, SessionStore sessionStore) =>
{
    var sessionState = new HttpSessionState(context.Session, sessionStore);
    var workflow = sessionState["CurrentWorkflow"] as WorkflowState;

    if (workflow is null)
    {
        return Results.NotFound(new
        {
            message = "No workflow is stored for the current session."
        });
    }

    var simpleName = context.Session.GetString("workflow-name");

    return Results.Ok(new
    {
        sessionId = context.Session.Id,
        workflow.Name,
        workflow.CreatedBy,
        ownerType = workflow.Owner.GetType().Name,
        reviewerType = workflow.Reviewer.GetType().Name,
        orderCount = workflow.Orders.Count,
        firstOrderBackReference = ReferenceEquals(workflow.Owner, workflow.Orders[0].User),
        simpleName,
        isNewSession = sessionStore.GetIsSessionNew(context.Session),
        objectKeys = sessionStore.GetAllObjectKeys(context.Session)
    });
});

app.MapPost("/session/workflow/clear", (HttpContext context, SessionStore sessionStore) =>
{
    var sessionState = new HttpSessionState(context.Session, sessionStore);
    sessionState["CurrentWorkflow"] = null;
    context.Session.Remove("workflow-name");

    return Results.NoContent();
});

app.Run("http://localhost:5883");
