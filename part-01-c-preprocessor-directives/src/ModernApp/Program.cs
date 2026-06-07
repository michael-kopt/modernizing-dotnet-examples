using Part01.Shared;

Console.WriteLine("ModernApp (.NET 8)");
Console.WriteLine();

foreach (var line in PlatformMessage.Describe())
{
    Console.WriteLine($"- {line}");
}
