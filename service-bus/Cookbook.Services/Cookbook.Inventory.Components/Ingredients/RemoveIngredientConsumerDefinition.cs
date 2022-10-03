using Cookbook.Shared.Configuration;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Cookbook.Inventory.Components.Ingredients;

public class RemoveIngredientConsumerDefinition : ConsumerDefinition<RemoveIngredientConsumer>
{
    private readonly RemoveIngredientOptions _listOptions;

    public RemoveIngredientConsumerDefinition(IOptions<RemoveIngredientOptions> options)
    {
        _listOptions = options.Value;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<RemoveIngredientConsumer> consumerConfigurator)
    {
        _listOptions.Configure(endpointConfigurator);

        endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 500));
        endpointConfigurator.UseInMemoryOutbox();
    }
}