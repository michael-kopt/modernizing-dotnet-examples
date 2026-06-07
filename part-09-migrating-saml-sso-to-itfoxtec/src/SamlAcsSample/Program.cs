using Part09.SamlAcsSample.Configuration;
using Part09.SamlAcsSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SamlOptions>(builder.Configuration.GetSection(SamlOptions.SectionName));
builder.Services.AddSingleton<SamlConfigurationProvider>();
builder.Services.AddSingleton<SamlResponseHandler>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/saml/config-summary"));

app.Run("http://localhost:5680");
