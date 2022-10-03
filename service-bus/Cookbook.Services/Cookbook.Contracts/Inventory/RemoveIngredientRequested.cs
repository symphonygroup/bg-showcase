namespace Cookbook.Contracts.Inventory;

public record RemoveIngredientRequested
{
    public string IngredientId { get; init; }
}