namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record CookingInventoryAllocationFailed
{
    public Guid CookingRequestId { get; init; }
}