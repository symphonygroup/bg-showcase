using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Contracts.Inventory;
using MassTransit;

namespace Cookbook.Cooking.Components.Cooking.StateMachineActivities;

public class AllocateInventoryActivity : IStateMachineActivity<CookingState, CookingInventoryAllocationStarted>
{
    public void Probe(ProbeContext context)
    {
        context.CreateScope("inventory-allocation-failure");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<CookingState, CookingInventoryAllocationStarted> context,
        IBehavior<CookingState, CookingInventoryAllocationStarted> next)
    {
        var sendEndpoint = await context.GetSendEndpoint(new Uri("exchange:process-inventory-allocation"));

        await sendEndpoint.Send<InventoryAllocationProcessing>(new
        {
            context.Saga.CookingRequestId,
            context.Saga.Ingredients
        });

        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<CookingState, CookingInventoryAllocationStarted, TException> context,
        IBehavior<CookingState, CookingInventoryAllocationStarted> next) where TException : Exception
    {
        return next.Faulted(context);
    }
}