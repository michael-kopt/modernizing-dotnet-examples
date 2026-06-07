namespace DictionaryToRedisSample.Models;

public sealed class User
{
    public int Id { get; init; }

    public string Username { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public List<string> Roles { get; init; } = [];
}
