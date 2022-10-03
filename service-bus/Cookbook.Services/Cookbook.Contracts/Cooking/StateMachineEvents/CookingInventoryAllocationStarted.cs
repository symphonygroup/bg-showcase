using Cookbook.Contracts.Inventory;

namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record CookingInventoryAllocationStarted
{
    public Guid CookingRequestId { get; set; }
    public List<RecipeIngredientModel> Ingredients { get; set; }
    public List<IngredientModel> Inventory { get; set; }
};