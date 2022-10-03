namespace Cookbook.Contracts.Cooking;

public record CookingRequest
{
    public string RecipeId { get; init; }
}