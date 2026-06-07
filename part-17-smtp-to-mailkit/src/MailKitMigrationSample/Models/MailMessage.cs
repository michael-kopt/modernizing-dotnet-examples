namespace MailKitMigrationSample.Models;

public sealed class MailMessage
{
    public MailMessage()
    {
        To = [];
        Cc = [];
        Bcc = [];
    }

    public MailMessage(string from, string to) : this()
    {
        From = new MailAddress(from);
        To.Add(new MailAddress(to));
    }

    public MailAddress From { get; set; } = null!;

    public List<MailAddress> To { get; }

    public List<MailAddress> Cc { get; }

    public List<MailAddress> Bcc { get; }

    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public bool IsBodyHtml { get; set; } = true;
}
