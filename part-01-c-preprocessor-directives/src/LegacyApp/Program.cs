using System;
using Part01.Shared;

namespace Part01.LegacyApp;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("LegacyApp (.NET Framework 4.8)");
        Console.WriteLine();

        foreach (var line in PlatformMessage.Describe())
        {
            Console.WriteLine($"- {line}");
        }
    }
}
