namespace Cookbook.Contracts.Cooking;

public record AddRecipeStored
{
    public string RecipeId { get; init; }
    public string Name { get; init; }
    public DateTime Timestamp { get; init; }
}