namespace Cookbook.Contracts.Inventory;

public record AddIngredientResponse
{
    public string IngredientId { get; init; }
    public string Name { get; init; }
    public string Unit { get; init; }
    public int Quantity { get; init; }
    public DateTime RequestedTimestamp { get; init; }
}