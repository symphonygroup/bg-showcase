using Cookbook.Contracts.Cooking.StateMachineEvents;
using MassTransit;

namespace Cookbook.Cooking.Components.Cooking.StateMachineActivities;

public class CookRecipeActivity : IStateMachineActivity<CookingState, RecipeCookingStarted>
{
    public void Probe(ProbeContext context)
    {
        context.CreateScope("cook-recipe");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<CookingState, RecipeCookingStarted> context,
        IBehavior<CookingState, RecipeCookingStarted> next)
    {
        var sendEndpoint = await context.GetSendEndpoint(new Uri("exchange:process-cooking-recipe"));
        
        
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<CookingState, RecipeCookingStarted, TException> context,
        IBehavior<CookingState, RecipeCookingStarted> next) where TException : Exception
    {
        return next.Faulted(context);
    }
}