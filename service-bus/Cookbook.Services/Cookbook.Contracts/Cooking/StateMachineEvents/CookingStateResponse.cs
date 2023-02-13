using Cookbook.Contracts.Inventory;

namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record CookingStateResponse
{
    public Guid CookingRequestId { get; set; }
    public string CurrentState { get; set; }
    public string RecipeId { get; set; }
    public int CookTime { get; set; }
    public int PrepTime { get; set; }
    public List<RecipeIngredientModel> Ingredients { get; set; }
    public List<IngredientModel> Inventory { get; set; }
}