namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record CookingIngredientsPreparationFailed
{
    public Guid CookingRequestId { get; init; }
    public string Reason { get; init; }
}