using Cookbook.Contracts.Cooking.StateMachineEvents;
using MassTransit;

namespace Cookbook.Cooking.Components.Cooking;

public class CookingStateMachineDefinition : SagaDefinition<CookingState>
{
    public CookingStateMachineDefinition()
    {
        ConcurrentMessageLimit = 16;
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<CookingState> sagaConfigurator)
    {
        // use endpointConfigurator to define middleware for all endpoints
        // use sagaConfigurator to define middleware for either specific messages, consumers, and/or events

        // partition incoming events by user profile id
        // this will attempt to group messages per user if possible.
        // partitioner is handling this per instance of a saga,
        // with conductor this is doable client side using consistent hash rate
        var partition = endpointConfigurator.CreatePartitioner(8);
        sagaConfigurator.Message<CookingRequestSubmitted>(x => x.UsePartitioner(partition, y => y.Message.RecipeId));

        // this configures retry on all saga endpoints
        // to specify different retries on different endpoints switch from
        // using ConfigureEndpoints extension to declaring specific
        // endpoints by using ReceiveEndpoint extension and define each of them
        var intervals = new[] { 500, 1000, 10000 };
        endpointConfigurator.UseMessageRetry(x => x.Intervals(intervals));

        // prevents messages sent or published by FSM from going out,
        // reason: if event was published and FSM could not be persisted it
        // would prevent the event from being dispatched since FSM is not in
        // correct state
        endpointConfigurator.UseInMemoryOutbox();
    }
}