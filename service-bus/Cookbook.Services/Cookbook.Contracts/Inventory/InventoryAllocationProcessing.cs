using Cookbook.Contracts.Cooking;

namespace Cookbook.Contracts.Inventory;

public record InventoryAllocationProcessing
{
    public Guid CookingRequestId { get; init; }
    public List<RecipeIngredientModel> Ingredients { get; init; }
}