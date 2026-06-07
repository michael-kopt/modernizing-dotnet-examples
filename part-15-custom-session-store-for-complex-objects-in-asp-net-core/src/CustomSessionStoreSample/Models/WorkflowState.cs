namespace CustomSessionStoreSample.Models;

public sealed class WorkflowState
{
    public string Name { get; init; } = string.Empty;

    public string CreatedBy { get; init; } = string.Empty;

    public User Owner { get; init; } = null!;

    public Person Reviewer { get; init; } = null!;

    public List<Order> Orders { get; init; } = [];
}
