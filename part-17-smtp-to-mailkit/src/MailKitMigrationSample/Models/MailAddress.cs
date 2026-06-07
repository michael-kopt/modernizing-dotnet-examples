namespace MailKitMigrationSample.Models;

public sealed class MailAddress
{
    public MailAddress(string address)
    {
        Address = address;
    }

    public MailAddress(string address, string name)
    {
        Address = address;
        Name = name;
    }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; }
}
