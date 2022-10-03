namespace Cookbook.Contracts.Inventory;

public record IngredientModel
{
    public string IngredientId { get; init; }
    public string Name { get; init; }
    public int Quantity { get; init; }
    public string Unit { get; init; }
}