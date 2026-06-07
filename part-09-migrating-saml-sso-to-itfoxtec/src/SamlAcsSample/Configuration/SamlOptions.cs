namespace Part09.SamlAcsSample.Configuration;

public sealed class SamlOptions
{
    public const string SectionName = "Saml";

    public string Issuer { get; set; } = "https://sp.example.local";
    public string? IdpEntityId { get; set; }
    public bool CertificateValidationEnabled { get; set; }
    public bool RevocationCheckEnabled { get; set; }
    public bool AudienceRestricted { get; set; }
    public string? DecryptionCertificatePath { get; set; }
    public string? DecryptionCertificatePassword { get; set; }
    public string? SignatureValidationCertificatePath { get; set; }
}
