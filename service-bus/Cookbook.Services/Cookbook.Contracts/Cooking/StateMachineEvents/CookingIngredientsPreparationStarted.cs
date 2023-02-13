namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record CookingIngredientsPreparationStarted
{
    public Guid CookingRequestId { get; set; }
    public string RecipeId { get; set; }
    public int PrepTime { get; set; }
}