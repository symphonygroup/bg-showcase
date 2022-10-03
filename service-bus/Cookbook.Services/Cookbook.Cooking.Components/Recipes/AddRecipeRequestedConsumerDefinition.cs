using Cookbook.Shared.Configuration;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Cookbook.Cooking.Components.Recipes;

public class AddRecipeRequestedConsumerDefinition : ConsumerDefinition<AddRecipeRequestedConsumer>
{
    private readonly AddRecipeOptions _options;

    public AddRecipeRequestedConsumerDefinition(IOptions<AddRecipeOptions> options)
    {
        _options = options.Value;
    }
    
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AddRecipeRequestedConsumer> consumerConfigurator)
    {
        _options.Configure(endpointConfigurator);
        
        endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 500));
        endpointConfigurator.UseInMemoryOutbox();
    }
}