namespace Cookbook.Contracts.Inventory;

public record AddIngredientRequest
{
    public string Name { get; init; }
    public string Unit { get; init; }
    public int Quantity { get; init; }
    public DateTime Timestamp { get; init; }
}