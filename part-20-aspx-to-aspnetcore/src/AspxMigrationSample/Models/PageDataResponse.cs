namespace AspxMigrationSample.Models;

public sealed class PageDataResponse
{
    public string SpaceLabel { get; init; } = string.Empty;

    public string SpaceID { get; init; } = string.Empty;

    public string Schema { get; init; } = string.Empty;

    public string DirectoryID { get; init; } = string.Empty;

    public string CurrentUser { get; init; } = string.Empty;

    public string Version { get; init; } = string.Empty;

    public string DateLabel { get; init; } = string.Empty;

    public bool PageDetails { get; init; }

    public bool IsAccountAdmin { get; init; }

    public List<ProductRow> Products { get; init; } = [];
}
