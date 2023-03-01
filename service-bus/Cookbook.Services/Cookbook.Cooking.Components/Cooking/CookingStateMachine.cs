using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Cooking.Components.Cooking.StateMachineActivities;
using MassTransit;
using MongoDB.Driver.Core.Events;

namespace Cookbook.Cooking.Components.Cooking;

public class CookingStateMachine : MassTransitStateMachine<CookingState>
{
    public CookingStateMachine()
    {
        Event(() => Submitted, x => x.CorrelateById(m => m.Message.CookingRequestId));
        Event(() => InventoryAllocationStarted, x => x.CorrelateById(m => m.Message.CookingRequestId));
        Event(() => InventoryAllocated, x => x.CorrelateById(m => m.Message.CookingRequestId));
        Event(() => InventoryAllocationFailed, x => x.CorrelateById(m => m.Message.CookingRequestId));
        Event(() => IngredientsPreparationStarted, x => x.CorrelateById(m => m.Message.CookingRequestId));
        Event(() => IngredientsPrepared, x => x.CorrelateById(m => m.Message.CookingRequestId));
        Event(() => IngredientsPreparationFailed, x => x.CorrelateById(m => m.Message.CookingRequestId));
        Event(() => CookingStarted, x => x.CorrelateById(m => m.Message.CookingRequestId));
        Event(() => Cooked, x => x.CorrelateById(m => m.Message.CookingRequestId));
        Event(() => CookingFailed, x => x.CorrelateById(m => m.Message.CookingRequestId));
        
        Event(() => CreateStateRequested, x =>
        {
            x.CorrelateById(m => m.Message.CookingRequestId);
            x.OnMissingInstance(y => y.ExecuteAsync(async context =>
            {
                if (context.RequestId.HasValue)
                {
                    await context.RespondAsync<CookingStateNotFound>(new { context.Message.CookingRequestId });
                }
            }));
        });

        InstanceState(x => x.CurrentState);

        Initially(
            When(Submitted)
                .Then(context =>
                {
                    context.Saga.RecipeId = context.Message.RecipeId;
                    context.Saga.CookingRequestId = context.Message.CookingRequestId;
                    context.Saga.CookTime = context.Message.CookTime;
                    context.Saga.PrepTime = context.Message.PrepTime;
                    context.Saga.Ingredients = context.Message.Ingredients;
                    context.Saga.Inventory = context.Message.Inventory;
                })
                .PublishAsync(context => context.Init<CookingInventoryAllocationStarted>(new
                {
                    context.Message.CookingRequestId,
                    context.Message.Ingredients,
                    context.Message.Inventory
                }))
                .TransitionTo(InventoryAllocation));

        During(InventoryAllocation,
            Ignore(Submitted),
            When(InventoryAllocationStarted)
                .Activity(x => x.OfType<AllocateInventoryActivity>()),
            When(InventoryAllocated)
                .PublishAsync(context => context.Init<CookingIngredientsPreparationStarted>(new
                {
                    context.Saga.CookingRequestId,
                    context.Saga.RecipeId,
                    context.Saga.PrepTime
                }))
                .TransitionTo(Preparation),
            When(InventoryAllocationFailed)
                .TransitionTo(IngredientsNotAllocated));

        During(Preparation,
            Ignore(InventoryAllocationStarted),
            When(IngredientsPreparationStarted)
                .Activity(x => x.OfType<PrepareIngredientsActivity>()),
            When(IngredientsPrepared)
                .PublishAsync(context => context.Init<CookingStartRequested>(new
                {
                    context.Saga.CookingRequestId,
                    context.Saga.RecipeId,
                    context.Saga.CookTime
                }))
                .TransitionTo(Cooking),
            When(IngredientsPreparationFailed)
                .TransitionTo(IngredientsNotPrepared));
        
        During(Cooking,
            When(CookingStarted)
                .Activity(x => x.OfType<CookRecipeActivity>()),
            When(Cooked)
                .PublishAsync(context => context.Init<MealStoringRequested>(new
                {
                    context.Saga.CookingRequestId,
                    context.Saga.RecipeId
                }))
                .TransitionTo(Success),
            When(CookingFailed)
                .TransitionTo(Failed));
        
        DuringAny(
            When(CreateStateRequested)
                .RespondAsync(x => x.Init<CookingStateResponse>(new
                    {
                        x.Saga.CookingRequestId,
                        x.Saga.RecipeId,
                        x.Saga.CookTime,
                        x.Saga.PrepTime,
                        x.Saga.Ingredients,
                        x.Saga.Inventory,
                        x.Saga.CurrentState
                    })
                ));
    }

    public State InventoryAllocation { get; private set; }
    public State Preparation { get; private set; }
    public State Cooking { get; private set; }
    public State IngredientsNotAllocated { get; private set; }
    public State IngredientsNotPrepared { get; private set; }
    public State Failed { get; private set; }
    public State Success { get; private set; }

    public Event<CookingRequestSubmitted> Submitted { get; private set; }
    public Event<CookingInventoryAllocationStarted> InventoryAllocationStarted { get; private set; }
    public Event<CookingInventoryAllocated> InventoryAllocated { get; private set; }
    public Event<CookingInventoryAllocationFailed> InventoryAllocationFailed { get; private set; }
    public Event<CookingIngredientsPreparationStarted> IngredientsPreparationStarted { get; private set; }
    public Event<CookingIngredientsPrepared> IngredientsPrepared { get; private set; }
    public Event<CookingIngredientsPreparationFailed> IngredientsPreparationFailed { get; private set; }
    public Event<RecipeCookingStarted> CookingStarted { get; private set; }
    public Event<RecipeCooked> Cooked { get; private set; }
    public Event<RecipeCookingFailed> CookingFailed { get; private set; }
    
    public Event<CookingStateRequested> CreateStateRequested { get; private set; }
}