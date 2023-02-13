using Cookbook.Contracts.Cooking;
using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Contracts.Inventory;
using Cookbook.Inventory.Components.Ingredients.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Cookbook.Inventory.Components.Ingredients;

public class ProcessInventoryAllocationConsumer : IConsumer<InventoryAllocationProcessing>
{
    private readonly IMongoCollection<Ingredient> _ingredients;
    private readonly ILogger<ProcessInventoryAllocationConsumer> _logger;

    public ProcessInventoryAllocationConsumer(IMongoCollection<Ingredient> ingredients,
        ILogger<ProcessInventoryAllocationConsumer> logger)
    {
        _ingredients = ingredients;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryAllocationProcessing> context)
    {
        try
        {
            var invalidIngredients = new List<RecipeIngredientModel>();
            foreach (var ingredient in context.Message.Ingredients)
            {
                // query filter to check if the ingredient exists and has enough quantity
                var checkFilter = Builders<Ingredient>.Filter.And(
                    Builders<Ingredient>.Filter.Eq(x => x.Id, ingredient.IngredientId),
                    Builders<Ingredient>.Filter.Gte(x => x.Quantity, ingredient.Quantity));
                
                // run query and throw exception if the ingredient does not exist or does not have enough quantity
                var checkResult = await _ingredients.Find(checkFilter).FirstOrDefaultAsync();
                if (checkResult != null)
                {
                    continue;
                }
                
                invalidIngredients.Add(ingredient);
                _logger.LogError("Ingredient {IngredientId} does not exist or does not have enough quantity",
                    ingredient.IngredientId);
            }
            
            // if there are invalid ingredients, publish the failure event
            if (invalidIngredients.Any())
            {
                await context.Publish<CookingInventoryAllocationFailed>(new
                {
                    context.Message.CookingRequestId,
                    InvalidIngredients = invalidIngredients
                });
                return;
            }
            
            var filter =
                Builders<Ingredient>.Filter.In(x => x.Id, context.Message.Ingredients.Select(x => x.IngredientId));
            var update = Builders<Ingredient>.Update;

            UpdateDefinition<Ingredient>? updateDefinition = null;
            foreach (var ingredient in context.Message.Ingredients)
            {
                updateDefinition = updateDefinition == null
                    ? update.Inc(x => x.Quantity, -ingredient.Quantity)
                    : updateDefinition.Inc(x => x.Quantity, -ingredient.Quantity);
            }

            var result = await _ingredients.UpdateManyAsync(filter, updateDefinition);
            if (result.ModifiedCount == context.Message.Ingredients.Count)
            {
                await context.Publish<CookingInventoryAllocated>(new
                {
                    context.Message.CookingRequestId
                });
            }
            else
            {
                await context.Publish<CookingInventoryAllocationFailed>(new
                {
                    context.Message.CookingRequestId
                });
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while processing inventory allocation");
            await context.Publish<CookingInventoryAllocationFailed>(new
            {
                context.Message.CookingRequestId
            });
        }
    }
}