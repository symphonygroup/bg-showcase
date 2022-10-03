namespace Cookbook.Cooking.Components.Recipes;

using AutoMapper;
using Cookbook.Contracts.Cooking;
using Contracts.Inventory;
using Persistence;
using MassTransit;
using MongoDB.Driver;

public class RecipesListConsumer : IConsumer<RecipesListRequest>
{
    private readonly IRequestClient<IngredientsListRequest> _ingredientsListRequestClient;
    private readonly IMongoCollection<Recipe> _recipesCollection;
    private readonly IMapper _mapper;

    public RecipesListConsumer(IRequestClient<IngredientsListRequest> ingredientsListRequestClient,
        IMongoCollection<Recipe> recipesCollection, IMapper mapper)
    {
        _ingredientsListRequestClient = ingredientsListRequestClient;
        _recipesCollection = recipesCollection;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<RecipesListRequest> context)
    {
        var recipes = new List<Recipe>();

        if (context.Message.RecipeIds == null  || context.Message.RecipeIds.Length == 0)
        {
            recipes = await _recipesCollection.Find(_ => true).ToListAsync();
        }
        else
        {
            var filter = Builders<Recipe>.Filter.In(r => r.Id, context.Message.RecipeIds);
            recipes = await _recipesCollection.Find(filter).ToListAsync();
        }

        var ingredientsListResponse = await _ingredientsListRequestClient.GetResponse<IngredientsListResponse>(new
        {
            RequestedAt = DateTime.UtcNow,
            IngredientIds = recipes.SelectMany(x => x.Ingredients).Select(x => x.IngredientId).Distinct().ToArray()
        });

        var recipeIngredients = recipes.GroupBy(x => x.Id).Select(x => new
        {
            RecipeId = x.Key,
            Ingredients = x.SelectMany(y => y.Ingredients).Distinct().ToArray(),
        }).ToList();

        var response = new RecipesListResponse
        {
            Recipes = new List<RecipeModel>()
        };
        
        foreach (var recipe in recipes)
        {
            var recipeModel = _mapper.Map<RecipeModel>(recipe);
            recipeModel.Ingredients = recipeIngredients.First(x => x.RecipeId == recipe.Id).Ingredients
                .Select(x => new IngredientModel
                {
                    IngredientId = x.IngredientId,
                    Name = ingredientsListResponse.Message.Ingredients.First(y => y.IngredientId == x.IngredientId).Name,
                    Unit = ingredientsListResponse.Message.Ingredients.First(y => y.IngredientId == x.IngredientId).Unit,
                    Quantity = x.Quantity
                }).ToList();
            
            response.Recipes.Add(recipeModel);
        }

        await context.RespondAsync(response);
    }
}