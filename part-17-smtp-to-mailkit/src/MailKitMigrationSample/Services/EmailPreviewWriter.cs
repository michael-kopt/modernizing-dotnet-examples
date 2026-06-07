using MailKitMigrationSample.Models;

namespace MailKitMigrationSample.Services;

public static class EmailPreviewWriter
{
    public static async Task<EmailPreviewResult> WriteAsync(SmtpClient client, MailMessage message, string filePath)
    {
        var mimeMessage = client.MapToMimeMessage(message);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await using var stream = File.Create(filePath);
        await mimeMessage.WriteToAsync(stream);

        return new EmailPreviewResult(
            filePath,
            mimeMessage.Subject ?? string.Empty,
            mimeMessage.To.Mailboxes.Select(mailbox => mailbox.Address).ToArray());
    }
}

public sealed record EmailPreviewResult(string FilePath, string Subject, IReadOnlyList<string> Recipients);
