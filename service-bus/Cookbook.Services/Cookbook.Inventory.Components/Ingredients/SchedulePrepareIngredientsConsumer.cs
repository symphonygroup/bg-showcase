using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Contracts.Inventory;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cookbook.Inventory.Components.Ingredients;

public class SchedulePrepareIngredientsConsumer : IConsumer<IngredientPreparationProcess>
{
    private readonly ILogger<SchedulePrepareIngredientsConsumer> _logger;

    public SchedulePrepareIngredientsConsumer(ILogger<SchedulePrepareIngredientsConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IngredientPreparationProcess> context)
    {
        try
        {
            _logger.LogInformation("Consumer: Preparing ingredients for cooking recipe {CookingRecipeId}",
                context.Message.RecipeId);
            var deliveryTime =
                DateTime.Now.AddMinutes(context.Message.PrepTime); // Change to UtcNow for running on servers

            await context.SchedulePublish<CookingIngredientsPrepared>(deliveryTime, new
            {
                context.Message.RecipeId,
                context.Message.CookingRequestId
            });
        }
        catch (Exception ex)
        {
            await context.Publish<CookingIngredientsPreparationFailed>(new
            {
                CookingRecipeId = context.Message.RecipeId,
                Reason = ex.Message
            });
        }
    }
}