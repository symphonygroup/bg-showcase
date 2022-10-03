namespace Cookbook.Contracts.Cooking;

public record RecipesListRequest
{
    public string[]? RecipeIds { get; init; }
    public DateTime RequestedAt { get; init; }
}