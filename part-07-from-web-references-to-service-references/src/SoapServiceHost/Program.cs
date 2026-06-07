using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Part07.SoapServiceHost.Contracts;
using Part07.SoapServiceHost.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<RequestRecorder>();
builder.Services.AddSingleton<LegacyLoginService>();
builder.Services.AddSingleton<SoapServiceController>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path.Equals("/SoapService.asmx", StringComparison.OrdinalIgnoreCase) &&
        HttpMethods.IsPost(context.Request.Method))
    {
        var recorder = context.RequestServices.GetRequiredService<RequestRecorder>();
        recorder.Record(
            context.Request.Headers["X-Client-Name"].ToString(),
            context.Request.Cookies["session-id"]);
    }

    await next();
});

app.UseServiceModel(serviceBuilder =>
{
    var httpBinding = new BasicHttpBinding
    {
        MaxBufferSize = 65536,
        MaxReceivedMessageSize = 65536,
        ReceiveTimeout = TimeSpan.FromMinutes(2),
        SendTimeout = TimeSpan.FromMinutes(2)
    };

    serviceBuilder.AddService<SoapServiceController>();
    serviceBuilder.AddServiceEndpoint<SoapServiceController, ISoapService>(httpBinding, "/SoapService.asmx");
});

var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
serviceMetadataBehavior.HttpGetEnabled = true;

app.MapGet("/debug/last-request", (RequestRecorder recorder) => Results.Json(recorder.GetSnapshot()));
app.MapPost("/debug/reset", (RequestRecorder recorder) =>
{
    recorder.Reset();
    return Results.NoContent();
});

app.MapGet("/", () => Results.Redirect("/SoapService.asmx"));

app.Run("http://localhost:5580");
