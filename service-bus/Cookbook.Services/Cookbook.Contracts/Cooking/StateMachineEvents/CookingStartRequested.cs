namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record CookingStartRequested
{
    public Guid CookingRequestId { get; set; }
    public string RecipeId { get; set; }
    public int CookTime { get; set; }
}