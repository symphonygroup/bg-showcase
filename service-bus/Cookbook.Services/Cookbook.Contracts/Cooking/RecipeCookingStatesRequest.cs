namespace Cookbook.Contracts.Cooking;

public record RecipeCookingStatesRequest
{
    public string RecipeId { get; init; }
}