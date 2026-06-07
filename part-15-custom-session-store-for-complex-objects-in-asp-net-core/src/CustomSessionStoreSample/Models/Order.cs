namespace CustomSessionStoreSample.Models;

public sealed class Order
{
    public int Id { get; init; }

    public User User { get; set; } = null!;

    public string Description { get; init; } = string.Empty;
}
