using System.Security.Cryptography.X509Certificates;
using ITfoxtec.Identity.Saml2;
using Microsoft.Extensions.Options;
using Part09.SamlAcsSample.Configuration;
using X509CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode;

namespace Part09.SamlAcsSample.Services;

public sealed class SamlConfigurationProvider(IOptions<SamlOptions> options)
{
    public Saml2Configuration GetConfiguration()
    {
        var settings = options.Value;

        var config = new Saml2Configuration
        {
            Issuer = settings.Issuer,
            CertificateValidationMode = settings.CertificateValidationEnabled
                ? X509CertificateValidationMode.ChainTrust
                : X509CertificateValidationMode.None,
            RevocationMode = settings.RevocationCheckEnabled
                ? X509RevocationMode.Online
                : X509RevocationMode.NoCheck,
            AudienceRestricted = settings.AudienceRestricted
        };

        if (!string.IsNullOrWhiteSpace(settings.IdpEntityId))
        {
            config.AllowedAudienceUris.Add(settings.IdpEntityId);
        }

        if (!string.IsNullOrWhiteSpace(settings.DecryptionCertificatePath) &&
            File.Exists(settings.DecryptionCertificatePath))
        {
            var certificate = new X509Certificate2(
                settings.DecryptionCertificatePath,
                settings.DecryptionCertificatePassword);
            config.DecryptionCertificates.Add(certificate);
        }

        if (!string.IsNullOrWhiteSpace(settings.SignatureValidationCertificatePath) &&
            File.Exists(settings.SignatureValidationCertificatePath))
        {
            var signingCertificate = new X509Certificate2(settings.SignatureValidationCertificatePath);
            config.SignatureValidationCertificates.Add(signingCertificate);
        }

        return config;
    }
}
