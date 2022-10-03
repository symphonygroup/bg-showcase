namespace Cookbook.Contracts.Cooking;

public record CookingRecipeProcessing
{
    public Guid CookingRecipeId { get; init; }
    public int CookTime { get; init; }
}