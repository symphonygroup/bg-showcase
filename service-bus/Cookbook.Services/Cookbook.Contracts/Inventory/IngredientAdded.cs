namespace Cookbook.Contracts.Inventory;

public record IngredientAdded
{
    public string IngredientId { get; init; }
    public string Name { get; init; }
    public int Quantity { get; init; }
    public string Unit { get; init; }
}