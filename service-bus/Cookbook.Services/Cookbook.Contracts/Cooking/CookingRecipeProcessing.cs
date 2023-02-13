namespace Cookbook.Contracts.Cooking;

public record CookingRecipeProcessing
{
    public Guid CookingRequestId { get; init; }
    public string RecipeId { get; init; }
    public int CookTime { get; init; }
}