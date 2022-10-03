namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public class RecipeCookingFailed
{
    public Guid CookingRequestId { get; set; }
}