using Cookbook.Contracts.Cooking.StateMachineEvents;

namespace Cookbook.Contracts.Cooking;

public record RecipeCookingStatesResponse
{
    public List<CookingStateResponse> States { get; set; }
}