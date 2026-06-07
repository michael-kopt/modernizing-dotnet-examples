using ITfoxtec.Identity.Saml2;

namespace Part09.SamlAcsSample.Models;

public sealed record SamlResponseData(
    Saml2AuthnResponse Response,
    string? RelayState);
