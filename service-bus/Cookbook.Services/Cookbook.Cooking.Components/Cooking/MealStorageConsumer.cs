using Cookbook.Contracts.Cooking;
using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Cooking.Components.Cooking.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Cookbook.Cooking.Components.Cooking;

public class MealStorageConsumer : IConsumer<MealStoringRequested>
{
    private readonly ILogger<MealStorageConsumer> _logger;
    private readonly IMongoCollection<Meal> _mealCollection;
    private readonly IRequestClient<RecipesListRequest> _recipesListRequestClient;

    public MealStorageConsumer(ILogger<MealStorageConsumer> logger, IMongoCollection<Meal> mealCollection, IRequestClient<RecipesListRequest> recipesListRequestClient)
    {
        _logger = logger;
        _mealCollection = mealCollection;
        _recipesListRequestClient = recipesListRequestClient;
    }

    public async Task Consume(ConsumeContext<MealStoringRequested> context)
    {
        var response = await _recipesListRequestClient.GetResponse<RecipesListResponse>(new
        {
            RecipeIds = new string[]
            {
                context.Message.RecipeId
            }
        });
        
        var recipe = response.Message.Recipes.First();
        
        // check if meal already exists
        var filter = Builders<Meal>.Filter.Eq(x => x.Name, recipe.Name);
        var existingMeal = await _mealCollection.Find(filter).FirstOrDefaultAsync();
        
        // if meal already exists, update the servings available
        if (existingMeal is not null)
        {
            var update = Builders<Meal>.Update.Inc(x => x.ServingsAvailable, recipe.Servings).Inc(x => x.TotalServingsPrepared, recipe.Servings);
            await _mealCollection.UpdateOneAsync(filter, update);
            
            return;
        }

        var meal = new Meal
        {
            Name = recipe.Name,
            Description = recipe.Description,
            ImageUrl = recipe.ImageUrl,
            ServingsAvailable = recipe.Servings,
            TotalServingsPrepared = recipe.Servings
        };
        
        await _mealCollection.InsertOneAsync(meal);
    }
}