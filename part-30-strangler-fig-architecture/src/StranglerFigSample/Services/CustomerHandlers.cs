namespace StranglerFigSample.Services;

public sealed class LegacyCustomerPageHandler
{
    public Task<CustomerPageResult> HandleAsync(int customerId, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CustomerPageResult(
            Source: "legacy",
            CustomerId: customerId,
            DisplayName: $"Legacy Customer {customerId}",
            Notes: "Handled by legacy ASP.NET Framework-style implementation."));
    }

    public Task<CustomerPostResult> HandlePostAsync(CustomerPagePostRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CustomerPostResult(
            Source: "legacy",
            CustomerId: request.CustomerId,
            Message: $"Legacy save completed for customer {request.CustomerId}."));
    }
}

public sealed class ModernCustomerPageHandler
{
    public Task<CustomerPageResult> HandleAsync(int customerId, bool failModern, CancellationToken cancellationToken)
    {
        if (failModern)
        {
            throw new InvalidOperationException("Modern customer page failed validation during rollout.");
        }

        return Task.FromResult(new CustomerPageResult(
            Source: "modern",
            CustomerId: customerId,
            DisplayName: $"Modern Customer {customerId}",
            Notes: "Handled by ASP.NET Core while preserving the old URL."));
    }

    public Task<CustomerPostResult> HandlePostAsync(CustomerPagePostRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CustomerPostResult(
            Source: "modern",
            CustomerId: request.CustomerId,
            Message: $"Modern save completed for customer {request.CustomerId}."));
    }
}

public sealed record CustomerPageResult(
    string Source,
    int CustomerId,
    string DisplayName,
    string Notes);

public sealed record CustomerPagePostRequest(
    int CustomerId,
    string Name);

public sealed record CustomerPostResult(
    string Source,
    int CustomerId,
    string Message);
