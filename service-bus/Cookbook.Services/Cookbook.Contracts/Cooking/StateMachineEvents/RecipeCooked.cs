namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record RecipeCooked
{
    public Guid CookingRequestId { get; set; }
    public string RecipeId { get; set; }
}