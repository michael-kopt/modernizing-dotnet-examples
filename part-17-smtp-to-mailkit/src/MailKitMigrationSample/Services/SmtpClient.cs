using System.Net;
using MailKit.Security;
using MailKitMigrationSample.Models;
using MimeKit;

namespace MailKitMigrationSample.Services;

public sealed class SmtpClient
{
    public SmtpClient()
    {
        Host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "localhost";
        Port = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 25;
        EnableSsl = bool.TryParse(Environment.GetEnvironmentVariable("SMTP_SSL"), out var enableSsl) && enableSsl;

        var username = Environment.GetEnvironmentVariable("SMTP_USER") ?? string.Empty;
        var password = Environment.GetEnvironmentVariable("SMTP_PASS") ?? string.Empty;
        Credentials = new NetworkCredential(username, password);
    }

    public bool EnableSsl { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public NetworkCredential Credentials { get; set; }

    public async Task SendAsync(MailMessage message, CancellationToken cancellationToken = default)
    {
        var mimeMessage = MapToMimeMessage(message);

        using var client = new MailKit.Net.Smtp.SmtpClient();
        await client.ConnectAsync(Host, Port, GetSocketOptions(), cancellationToken);

        if (!string.IsNullOrWhiteSpace(Credentials.UserName))
        {
            await client.AuthenticateAsync(Credentials.UserName, Credentials.Password, cancellationToken);
        }

        await client.SendAsync(mimeMessage, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    public MimeMessage MapToMimeMessage(MailMessage message)
    {
        var mimeMessage = new MimeMessage();

        mimeMessage.From.Add(new MailboxAddress(message.From.Name, message.From.Address));

        foreach (var recipient in message.To)
        {
            mimeMessage.To.Add(new MailboxAddress(recipient.Name, recipient.Address));
        }

        foreach (var recipient in message.Cc)
        {
            mimeMessage.Cc.Add(new MailboxAddress(recipient.Name, recipient.Address));
        }

        foreach (var recipient in message.Bcc)
        {
            mimeMessage.Bcc.Add(new MailboxAddress(recipient.Name, recipient.Address));
        }

        mimeMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder();

        if (message.IsBodyHtml)
        {
            bodyBuilder.HtmlBody = message.Body;
        }
        else
        {
            bodyBuilder.TextBody = message.Body;
        }

        mimeMessage.Body = bodyBuilder.ToMessageBody();
        return mimeMessage;
    }

    private SecureSocketOptions GetSocketOptions()
    {
        if (!EnableSsl)
        {
            return SecureSocketOptions.None;
        }

        return Port == 465
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;
    }
}
