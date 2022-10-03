using Cookbook.Contracts.Cooking;
using Cookbook.Contracts.Inventory;
using MassTransit;

namespace Cookbook.Cooking.Components.Cooking;

public class CookingState : SagaStateMachineInstance, ISagaVersion
{
    public string CurrentState { get; set; }
    
    public Guid CookingRequestId { get; set; }
    public string RecipeId { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public List<RecipeIngredientModel> Ingredients { get; set; }
    public List<IngredientModel> Inventory { get; set; }

    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
}