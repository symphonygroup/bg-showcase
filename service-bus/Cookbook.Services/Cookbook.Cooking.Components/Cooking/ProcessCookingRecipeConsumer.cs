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
            var deliveryTime = DateTime.UtcNow.AddMinutes(context.Message.CookTime);
            await context.SchedulePublish<RecipeCooked>(deliveryTime, new
            {
                context.Message.CookingRecipeId
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while processing cooking recipe");
            await context.Publish<RecipeCookingFailed>(new
            {
                context.Message.CookingRecipeId
            });
        }
    }
}