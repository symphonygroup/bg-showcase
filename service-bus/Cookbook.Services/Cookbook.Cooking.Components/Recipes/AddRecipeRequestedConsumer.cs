using Cookbook.Contracts.Cooking;
using Cookbook.Cooking.Components.Recipes.Persistence;
using MassTransit;
using MongoDB.Driver;

namespace Cookbook.Cooking.Components.Recipes;

public class AddRecipeRequestedConsumer : IConsumer<AddRecipeRequested>
{
    private readonly IMongoCollection<Recipe> _recipes;

    public AddRecipeRequestedConsumer(IMongoCollection<Recipe> recipes)
    {
        _recipes = recipes;
    }
    
    public async Task Consume(ConsumeContext<AddRecipeRequested> context)
    {
        var recipe = new Recipe
        {
            Name = context.Message.Name,
            Description = context.Message.Description,
            Ingredients = context.Message.Ingredients,
            Instructions = context.Message.Instructions,
            Servings = context.Message.Servings,
            PrepTime = context.Message.PrepTime,
            CookTime = context.Message.CookTime,
            ImageUrl = context.Message.ImageUrl
        };
        
        try
        {
            await _recipes.InsertOneAsync(recipe);
            await context.RespondAsync<AddRecipeStored>(new 
            {
                RecipeId = recipe.Id,
                Name = recipe.Name,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception e)
        {
            await context.RespondAsync<AddRecipeStoreFailed>(new
            {
                Name = recipe.Name,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}