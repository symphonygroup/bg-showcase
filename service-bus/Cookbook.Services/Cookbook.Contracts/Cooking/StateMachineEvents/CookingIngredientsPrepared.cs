namespace Cookbook.Contracts.Cooking.StateMachineEvents;

public record CookingIngredientsPrepared
{
    public Guid CookingRequestId { get; init; }
    public string RecipeId { get; init; }
}