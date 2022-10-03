namespace Cookbook.Contracts.Cooking;

public record RecipeRemovalFailed
{
    public string RecipeId { get; init; }
    public string Reason { get; init; }
}