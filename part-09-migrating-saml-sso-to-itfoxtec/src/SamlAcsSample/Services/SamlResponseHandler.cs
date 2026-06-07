using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Part09.SamlAcsSample.Models;

namespace Part09.SamlAcsSample.Services;

public sealed class SamlResponseHandler(
    SamlConfigurationProvider configurationProvider,
    ILogger<SamlResponseHandler> logger)
{
    public async Task<bool> HasSamlResponseAsync(HttpRequest request)
    {
        if (!request.HasFormContentType)
        {
            return false;
        }

        var form = await request.ReadFormAsync();
        return !string.IsNullOrWhiteSpace(form["SAMLResponse"]);
    }

    public SamlResponseData ExtractResponse(HttpRequest request)
    {
        var config = configurationProvider.GetConfiguration();
        var binding = new Saml2PostBinding();
        var saml2AuthnResponse = new Saml2AuthnResponse(config);

        binding.ReadSamlResponse(request.ToGenericHttpRequest(validate: true), saml2AuthnResponse);

        logger.LogInformation(
            "SAML response parsed. RelayState present: {HasRelayState}, Claims count: {ClaimsCount}",
            !string.IsNullOrWhiteSpace(binding.RelayState),
            saml2AuthnResponse.ClaimsIdentity?.Claims?.Count() ?? 0);

        return new SamlResponseData(saml2AuthnResponse, binding.RelayState);
    }
}
