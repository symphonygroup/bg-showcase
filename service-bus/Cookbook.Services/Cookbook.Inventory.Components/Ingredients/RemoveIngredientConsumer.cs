using Cookbook.Contracts.Inventory;
using Cookbook.Inventory.Components.Ingredients.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Cookbook.Inventory.Components.Ingredients;

public class RemoveIngredientConsumer : IConsumer<RemoveIngredientRequested>
{
    private readonly IMongoCollection<Ingredient> _ingredients;
    private readonly ILogger<RemoveIngredientConsumer> _logger;

    public RemoveIngredientConsumer(IMongoCollection<Ingredient> ingredients, ILogger<RemoveIngredientConsumer> logger)
    {
        _ingredients = ingredients;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RemoveIngredientRequested> context)
    {
        try
        {
            var filter = Builders<Ingredient>.Filter.Eq(i => i.Id, context.Message.IngredientId);
            await _ingredients.FindOneAndDeleteAsync(filter);
            await context.Publish<IngredientRemoved>(new { context.Message.IngredientId });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to remove ingredient {IngredientId}", context.Message.IngredientId);
            await context.Publish<IngredientRemovalFailed>(new { context.Message.IngredientId, Reason = e.Message });
        }
    }
}