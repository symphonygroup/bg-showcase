using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Contracts.Inventory;
using MassTransit;

namespace Cookbook.Cooking.Components.Cooking.StateMachineActivities;

public class PrepareIngredientsActivity : IStateMachineActivity<CookingState, CookingIngredientsPreparationStarted>
{
    public void Probe(ProbeContext context)
    {
        context.CreateScope("prepare-ingredients");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<CookingState, CookingIngredientsPreparationStarted> context,
        IBehavior<CookingState, CookingIngredientsPreparationStarted> next)
    {
        var sendEndpoint = await context.GetSendEndpoint(new Uri("exchange:prepare-ingredients"));
        await sendEndpoint.Send<IngredientPreparationProcess>(new
        {
            context.Saga.CookingRequestId,
            context.Saga.PrepTime
        });
        
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<CookingState, CookingIngredientsPreparationStarted, TException> context,
        IBehavior<CookingState, CookingIngredientsPreparationStarted> next) where TException : Exception
    {
        return next.Faulted(context);
    }
}