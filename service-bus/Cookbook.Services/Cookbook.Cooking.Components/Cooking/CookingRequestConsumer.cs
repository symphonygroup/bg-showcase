using Cookbook.Contracts.Cooking;
using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Contracts.Inventory;
using Cookbook.Cooking.Components.Recipes.Persistence;
using MassTransit;
using MongoDB.Driver;

namespace Cookbook.Cooking.Components.Cooking;

public class CookingRequestConsumer : IConsumer<CookingRequest>
{
    private readonly IRequestClient<IngredientsListRequest> _ingredientsListRequestClient;
    private readonly IMongoCollection<Recipe> _recipes;

    public CookingRequestConsumer(IRequestClient<IngredientsListRequest> ingredientsListRequestClient,
        IMongoCollection<Recipe> recipes)
    {
        _ingredientsListRequestClient = ingredientsListRequestClient;
        _recipes = recipes;
    }

    public async Task Consume(ConsumeContext<CookingRequest> context)
    {
        var cookingRequestId = NewId.NextGuid();

        await context.RespondAsync<CookingResponse>(new
        {
            CookingRequestId = cookingRequestId,
            context.Message.RecipeId
        });

        var filter = Builders<Recipe>.Filter.Eq(r => r.Id, context.Message.RecipeId);
        var recipe = await _recipes.Find(filter).FirstOrDefaultAsync();
        var ingredientList = await _ingredientsListRequestClient.GetResponse<IngredientsListResponse>(new
        {
            IngredientIds = recipe.Ingredients.Select(i => i.IngredientId).ToArray(),
            RequestedAt = DateTime.UtcNow
        });

        await context.Publish<CookingRequestSubmitted>(new
        {
            CookingRequestId = cookingRequestId,
            context.Message.RecipeId,
            recipe.PrepTime,
            recipe.CookTime,
            recipe.Ingredients,
            Inventory = ingredientList.Message.Ingredients
        });
    }
}