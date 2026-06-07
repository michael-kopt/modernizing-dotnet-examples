using MailKitMigrationSample.Services;

var command = args.FirstOrDefault()?.ToLowerInvariant() ?? "preview";

var smtpClient = new SmtpClient();
var sampleMessage = SampleMessageFactory.Create();

switch (command)
{
    case "preview":
    {
        var outputPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "tests", "preview-message.eml"));
        var result = await EmailPreviewWriter.WriteAsync(smtpClient, sampleMessage, outputPath);

        Console.WriteLine($"[preview] file={result.FilePath}");
        Console.WriteLine($"[preview] subject={result.Subject}");
        Console.WriteLine($"[preview] to={string.Join(",", result.Recipients)}");
        break;
    }

    case "config":
    {
        Console.WriteLine($"[config] host={smtpClient.Host}");
        Console.WriteLine($"[config] port={smtpClient.Port}");
        Console.WriteLine($"[config] ssl={smtpClient.EnableSsl}");
        Console.WriteLine($"[config] user={(string.IsNullOrWhiteSpace(smtpClient.Credentials?.UserName) ? "<empty>" : smtpClient.Credentials.UserName)}");
        break;
    }

    case "send":
    {
        await smtpClient.SendAsync(sampleMessage);
        Console.WriteLine("[send] message sent successfully");
        break;
    }

    default:
        Console.WriteLine("Supported commands: preview, config, send");
        break;
}
