namespace Cookbook.Contracts.Cooking;

public record AddRecipeStoreFailed
{
    public string Name { get; init; }
    public DateTime Timestamp { get; init; }
}