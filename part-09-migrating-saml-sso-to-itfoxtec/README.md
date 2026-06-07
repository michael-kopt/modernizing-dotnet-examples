# Part 09: Migrating SAML SSO to ITfoxtec

Article: [Modernizing .NET - Part 9: Migrating SAML SSO to ITfoxtec](https://medium.com/@michael.kopt/modernizing-net-part-9-migrating-saml-sso-to-itfoxtec-44133b003702)

This sample shows the core migration pattern from a legacy SAML integration to `ITfoxtec.Identity.Saml2` in ASP.NET Core.

## Sample Focus

The article centers on three moving parts:

- a central SAML configuration provider
- a response handler that reads `SAMLResponse` from the posted form
- an ACS endpoint that validates the response, extracts claims, logs what matters, and redirects using `RelayState`

## Structure

- `Configuration/SamlOptions.cs` contains app-level SAML settings
- `Services/SamlConfigurationProvider.cs` builds `Saml2Configuration`
- `Services/SamlResponseHandler.cs` reads and validates the posted SAML response
- `Controllers/SamlController.cs` exposes the ACS endpoint

## Implementation Notes

- `POST /saml/acs`
- `GET /saml/config-summary`

`/saml/config-summary` is included only as a migration aid so you can confirm which safety switches and certificate settings are currently active.

## Notes

- The sample compiles and demonstrates the migration pattern, but it is not fully runnable without customer-specific IdP configuration and certificates.
- The defaults intentionally mirror the article's migration posture: certificate validation disabled, revocation checking disabled, and audience restriction disabled.
- In production, those switches should usually be tightened.
