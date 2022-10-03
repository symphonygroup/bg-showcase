using Cookbook.Shared.Configuration;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Cookbook.Inventory.Components.Ingredients;

public class IngredientsListConsumerDefinition : ConsumerDefinition<IngredientsListConsumer>
{
    private readonly IngredientsListOptions _listOptions;

    public IngredientsListConsumerDefinition(IOptions<IngredientsListOptions> options)
    {
        _listOptions = options.Value;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<IngredientsListConsumer> consumerConfigurator)
    {
        _listOptions.Configure(endpointConfigurator);

        endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 500));
        endpointConfigurator.UseInMemoryOutbox();
    }
}