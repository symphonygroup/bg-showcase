﻿using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Contracts.Inventory;
using MassTransit;

namespace Cookbook.Inventory.Components.Ingredients;

public class PrepareIngredientsConsumer : IConsumer<IngredientPreparationProcess>
{
    public async Task Consume(ConsumeContext<IngredientPreparationProcess> context)
    {
        try
        {
            var deliveryTime = DateTime.Now.AddMinutes(context.Message.PrepTime); // Change to UtcNow for running on servers

            await context.SchedulePublish<CookingIngredientsPrepared>(deliveryTime, new
            {
                context.Message.CookingRecipeId,
            });
        }
        catch (Exception ex)
        {
            await context.Publish<CookingIngredientsPreparationFailed>(new
            {
                context.Message.CookingRecipeId,
                Reason = ex.Message
            });
        }
    }
}