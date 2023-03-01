namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record MealStoringRequested
{
    public Guid CookingRequestId { get; set; }
    public string RecipeId { get; set; }
}