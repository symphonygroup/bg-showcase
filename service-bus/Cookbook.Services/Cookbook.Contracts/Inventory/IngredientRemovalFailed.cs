namespace Cookbook.Contracts.Inventory;

public record IngredientRemovalFailed
{
    public string IngredientId { get; init; }
    public string Reason { get; init; }
}