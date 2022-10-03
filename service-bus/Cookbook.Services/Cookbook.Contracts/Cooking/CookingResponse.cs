namespace Cookbook.Contracts.Cooking;

public record CookingResponse
{
    public string RecipeId { get; init; }
    public Guid CookingRequestId { get; init; }
}