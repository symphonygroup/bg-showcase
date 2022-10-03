namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record CookingInventoryAllocated
{
    public Guid CookingRequestId { get; init; }
}