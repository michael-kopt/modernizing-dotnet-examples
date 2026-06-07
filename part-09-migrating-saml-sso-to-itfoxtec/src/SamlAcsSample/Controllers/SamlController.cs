using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Part09.SamlAcsSample.Configuration;
using Part09.SamlAcsSample.Services;

namespace Part09.SamlAcsSample.Controllers;

[ApiController]
[Route("saml")]
public sealed class SamlController(
    SamlResponseHandler responseHandler,
    SamlConfigurationProvider configurationProvider,
    IOptions<SamlOptions> options,
    ILogger<SamlController> logger) : ControllerBase
{
    [HttpGet("config-summary")]
    public IActionResult ConfigSummary()
    {
        var settings = options.Value;
        var config = configurationProvider.GetConfiguration();

        return Ok(new
        {
            settings.Issuer,
            settings.IdpEntityId,
            config.CertificateValidationMode,
            config.RevocationMode,
            config.AudienceRestricted,
            DecryptionCertificates = config.DecryptionCertificates.Count,
            SignatureValidationCertificates = config.SignatureValidationCertificates.Count
        });
    }

    [HttpPost("acs")]
    public async Task<IActionResult> AssertionConsumerService()
    {
        if (!await responseHandler.HasSamlResponseAsync(Request))
        {
            logger.LogWarning("ACS called without SAMLResponse form value.");
            return BadRequest("No SAML response found");
        }

        try
        {
            var samlData = responseHandler.ExtractResponse(Request);
            var claims = samlData.Response.ClaimsIdentity?.Claims?.ToList() ?? [];
            var nameId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var issuer = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Issuer;
            var redirectUrl = string.IsNullOrWhiteSpace(samlData.RelayState) ? "/default" : samlData.RelayState;

            logger.LogInformation(
                "SAML ACS success. NameId: {NameId}, Issuer: {Issuer}, RelayState: {RelayState}, Claims: {Claims}",
                nameId,
                issuer,
                samlData.RelayState,
                claims.Select(c => new { c.Type, c.Value, c.Issuer }));

            return Ok(new
            {
                NameId = nameId,
                Issuer = issuer,
                RedirectUrl = redirectUrl,
                Claims = claims.Select(c => new { c.Type, c.Value, c.Issuer })
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process SAML response.");
            return BadRequest(new
            {
                Message = "Failed to process SAML response",
                Error = ex.Message
            });
        }
    }
}
