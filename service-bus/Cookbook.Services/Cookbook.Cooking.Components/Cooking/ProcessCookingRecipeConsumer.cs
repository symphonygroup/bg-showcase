using Cookbook.Contracts.Cooking;
using Cookbook.Contracts.Cooking.StateMachineEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cookbook.Cooking.Components.Cooking;

public class ProcessCookingRecipeConsumer : IConsumer<CookingRecipeProcessing>
{
    private readonly ILogger<ProcessCookingRecipeConsumer> _logger;

    public ProcessCookingRecipeConsumer(ILogger<ProcessCookingRecipeConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CookingRecipeProcessing> context)
    {
        try
        {
            _logger.LogInformation("Consumer: Cooking recipe {CookingRecipeId}", context.Message.RecipeId);
            var deliveryTime = DateTime.Now.AddMinutes(context.Message.CookTime); // Change to UtcNow for running on servers
            await context.SchedulePublish<RecipeCooked>(deliveryTime, new
            {
                context.Message.CookingRequestId,
                context.Message.RecipeId
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while processing cooking recipe");
            await context.Publish<RecipeCookingFailed>(new
            {
                CookingRecipeId = context.Message.CookingRequestId
            });
        }
    }
}