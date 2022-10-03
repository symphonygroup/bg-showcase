namespace Cookbook.Contracts.Inventory;

public record IngredientsListRequest
{
    public string[]? IngredientIds { get; init; }
    public DateTime RequestedAt { get; init; }
}