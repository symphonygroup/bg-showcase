using Cookbook.Contracts.Cooking;
using Cookbook.Cooking.Components.Recipes.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Cookbook.Cooking.Components.Recipes;

public class RemoveRecipeConsumer : IConsumer<RemoveRecipeRequested>
{
    private readonly IMongoCollection<Recipe> _recipes;
    private readonly ILogger<RemoveRecipeConsumer> _logger;

    public RemoveRecipeConsumer(IMongoCollection<Recipe> recipes, ILogger<RemoveRecipeConsumer> logger)
    {
        _recipes = recipes;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RemoveRecipeRequested> context)
    {
        try
        {
            var filter = Builders<Recipe>.Filter.Eq(r => r.Id, context.Message.RecipeId);
            await _recipes.DeleteOneAsync(filter);
            await context.Publish<RecipeRemoved>(new { context.Message.RecipeId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove recipe {RecipeId}", context.Message.RecipeId);
            await context.Publish<RecipeRemovalFailed>(new { context.Message.RecipeId, Reason = ex.Message });
        }
    }
}