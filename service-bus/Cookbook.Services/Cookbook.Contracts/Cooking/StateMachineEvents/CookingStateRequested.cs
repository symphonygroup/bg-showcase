namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record CookingStateRequested
{
    public Guid CookingRequestId { get; set; }
}