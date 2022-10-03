namespace Cookbook.Contracts.Cooking;

public record RemoveRecipeRequested
{
    public string RecipeId { get; init; }
}