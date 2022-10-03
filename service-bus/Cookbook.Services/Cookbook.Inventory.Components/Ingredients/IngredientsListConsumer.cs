using AutoMapper;
using Cookbook.Contracts.Inventory;
using Cookbook.Inventory.Components.Ingredients.Persistence;
using MassTransit;
using MongoDB.Driver;

namespace Cookbook.Inventory.Components.Ingredients;

public class IngredientsListConsumer : IConsumer<IngredientsListRequest>
{
    private readonly IMongoCollection<Ingredient> _ingredients;
    private readonly IMapper _mapper;

    public IngredientsListConsumer(IMongoCollection<Ingredient> ingredients, IMapper mapper)
    {
        _ingredients = ingredients;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<IngredientsListRequest> context)
    {
        var ingredients = new List<Ingredient>();

        if (context.Message.IngredientIds == null || context.Message.IngredientIds.Length == 0)
        {
            ingredients = await _ingredients.Find(_ => true).ToListAsync();
        }
        else
        {
            var filter = Builders<Ingredient>.Filter.In(x => x.Id, context.Message.IngredientIds);
            ingredients = await _ingredients.Find(filter).ToListAsync();
        }

        var response = new IngredientsListResponse
        {
            Ingredients = _mapper.Map<List<Ingredient>, List<IngredientModel>>(ingredients)
        };
        
        await context.RespondAsync(response);
    }
}