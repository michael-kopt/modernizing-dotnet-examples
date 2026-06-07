using CustomSessionStoreSample.Models;

namespace CustomSessionStoreSample.Services;

public static class WorkflowFactory
{
    public static WorkflowState Create()
    {
        var owner = new User
        {
            Name = "Ada"
        };

        var reviewer = new Reviewer
        {
            Name = "Grace",
            Role = "Approver"
        };

        var orders = new List<Order>
        {
            new()
            {
                Id = 101,
                Description = "Quarterly report export",
                User = owner
            },
            new()
            {
                Id = 102,
                Description = "Reconciliation attachment bundle",
                User = owner
            }
        };

        owner.Orders.AddRange(orders);

        return new WorkflowState
        {
            Name = "FinanceCloseWorkflow",
            CreatedBy = "legacy-module",
            Owner = owner,
            Reviewer = reviewer,
            Orders = orders
        };
    }
}
