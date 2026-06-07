using System.Collections.Generic;

namespace Part01.Shared;

public static class PlatformMessage
{
    public static IReadOnlyList<string> Describe()
    {
        var lines = new List<string>();

        lines.Add("Shared code: this line is compiled for both targets.");

#if PROJECT_DOTNET_CORE
        lines.Add("Platform-specific code: compiled only for .NET 8.");
#endif

#if PROJECT_DOTNET_FRAMEWORK
        lines.Add("Platform-specific code: compiled only for .NET Framework.");
#endif

        lines.Add("Migration guidance: define your own symbols instead of relying on framework defaults.");

        return lines;
    }
}
