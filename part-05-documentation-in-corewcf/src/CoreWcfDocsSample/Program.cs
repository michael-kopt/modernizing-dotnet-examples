using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Part05.CoreWcfDocsSample.Contracts;
using Part05.CoreWcfDocsSample.Middleware;
using Part05.CoreWcfDocsSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<SoapDocumentationService>();
builder.Services.AddSingleton<LegacyLoginService>();
builder.Services.AddSingleton<SoapServiceController>();

var app = builder.Build();

app.UseWhen(
    context =>
        HttpMethods.IsGet(context.Request.Method) &&
        context.Request.Path.Equals("/SoapService.asmx", StringComparison.OrdinalIgnoreCase) &&
        !context.Request.Query.ContainsKey("wsdl") &&
        !context.Request.Query.ContainsKey("singleWsdl"),
    appBuilder =>
    {
        appBuilder.UseMiddleware<SoapDocumentationMiddleware>();
    });

app.UseWhen(
    context =>
        HttpMethods.IsGet(context.Request.Method) &&
        context.Request.Path.Equals("/SoapService.asmx", StringComparison.OrdinalIgnoreCase) &&
        context.Request.QueryString.HasValue,
    appBuilder =>
    {
        appBuilder.UseMiddleware<WsdlCustomizerMiddleware>();
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

app.MapGet("/", () => Results.Redirect("/SoapService.asmx"));

app.Run("http://localhost:5380");
