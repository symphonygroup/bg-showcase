using Cookbook.Contracts.Cooking.StateMachineEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cookbook.Cooking.Components.Cooking;

public class CookingStartConsumer : IConsumer<CookingStartRequested>
{
    private readonly ILogger<CookingStartConsumer> _logger;
    
    public CookingStartConsumer(ILogger<CookingStartConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CookingStartRequested> context)
    {
        try
        {
            _logger.LogInformation($"Cooking recipe {context.Message.CookingRequestId}, cook time {context.Message.CookTime}");
            await context.Publish<RecipeCookingStarted>(new
            {
                context.Message.CookingRequestId,
                context.Message.RecipeId,
                context.Message.CookTime
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while processing cooking recipe");
            await context.Publish<RecipeCookingFailed>(new
            {
                context.Message.CookingRequestId
            });
        }
    }
}