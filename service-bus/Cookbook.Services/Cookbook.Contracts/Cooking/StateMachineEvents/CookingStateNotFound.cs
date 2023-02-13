namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record CookingStateNotFound
{
    public Guid CookingRequestId { get; set; }
}