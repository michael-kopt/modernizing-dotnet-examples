namespace CustomSessionStoreSample.Models;

public sealed class User : Person
{
    public List<Order> Orders { get; init; } = [];
}
