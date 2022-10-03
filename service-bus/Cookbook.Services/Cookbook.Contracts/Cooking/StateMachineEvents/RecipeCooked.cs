namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public class RecipeCooked
{
    public Guid CookingRequestId { get; set; }
}