using Cookbook.Contracts.Inventory;

namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record CookingRequestSubmitted
{
    public Guid CookingRequestId { get; init; }
    public string RecipeId { get; init; }
    public int PrepTime { get; init; }
    public int CookTime { get; init; }
    public List<RecipeIngredientModel> Ingredients { get; set; }
    public List<IngredientModel> Inventory { get; set; }
}