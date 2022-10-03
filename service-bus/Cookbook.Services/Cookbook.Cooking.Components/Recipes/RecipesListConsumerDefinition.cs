using Cookbook.Shared.Configuration;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Cookbook.Cooking.Components.Recipes;

public class RecipesListConsumerDefinition : ConsumerDefinition<RecipesListConsumer>
{
    private readonly RecipesListOptions _options;

    public RecipesListConsumerDefinition(IOptions<RecipesListOptions> options)
    {
        _options = options.Value;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<RecipesListConsumer> consumerConfigurator)
    {
        _options.Configure(endpointConfigurator);

        endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 500));
        endpointConfigurator.UseInMemoryOutbox();
    }
}