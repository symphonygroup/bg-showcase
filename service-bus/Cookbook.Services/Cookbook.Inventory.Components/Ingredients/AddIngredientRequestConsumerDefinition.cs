using Cookbook.Shared.Configuration;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Cookbook.Inventory.Components.Ingredients;

public class AddIngredientRequestConsumerDefinition :
    ConsumerDefinition<AddIngredientRequestConsumer>
{
    private readonly AddIngredientOptions _options;

    public AddIngredientRequestConsumerDefinition(IOptions<AddIngredientOptions> options)
    {
        _options = options.Value;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<AddIngredientRequestConsumer> consumerConfigurator)
    {
        _options.Configure(endpointConfigurator);

        endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 500));
        endpointConfigurator.UseInMemoryOutbox();
    }
}