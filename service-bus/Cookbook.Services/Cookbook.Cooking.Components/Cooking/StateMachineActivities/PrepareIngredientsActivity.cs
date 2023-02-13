using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Contracts.Inventory;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cookbook.Cooking.Components.Cooking.StateMachineActivities;

public class PrepareIngredientsActivity : IStateMachineActivity<CookingState, CookingIngredientsPreparationStarted>
{
    private readonly ILogger<PrepareIngredientsActivity> _logger;

    public PrepareIngredientsActivity(ILogger<PrepareIngredientsActivity> logger)
    {
        _logger = logger;
    }

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
        _logger.LogInformation($"Activity: Cooking recipe {context.Message.RecipeId}, RequestId: {context.Message.CookingRequestId}");

        var sendEndpoint = await context.GetSendEndpoint(new Uri("queue:schedule-prepare-ingredients"));

        await sendEndpoint.Send<IngredientPreparationProcess>(new
        {
            context.Message.CookingRequestId,
            context.Message.RecipeId,
            context.Message.PrepTime
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