using Cookbook.Contracts.Inventory;
using Cookbook.Inventory.Components.Ingredients.Persistence;
using MassTransit;
using MongoDB.Driver;

namespace Cookbook.Inventory.Components.Ingredients;

public class AddIngredientRequestConsumer : IConsumer<AddIngredientRequest>
{
    private readonly IMongoCollection<Ingredient> _ingredients;

    public AddIngredientRequestConsumer(IMongoCollection<Ingredient> ingredients)
    {
        _ingredients = ingredients;
    }

    public async Task Consume(ConsumeContext<AddIngredientRequest> context)
    {
        var newIngredient = new Ingredient
        {
            Name = context.Message.Name,
            Unit = context.Message.Unit,
            Quantity = context.Message.Quantity,
        };

        await _ingredients.InsertOneAsync(newIngredient);


        await context.RespondAsync<AddIngredientResponse>(new
        {
            IngredientId = newIngredient.Id,
            Name = context.Message.Name,
            Unit = context.Message.Unit,
            Quantity = context.Message.Quantity,
            RequestedTimestamp = DateTime.UtcNow
        });

        await context.Publish<IngredientAdded>(new
        {
            IngredientId = newIngredient.Id,
            context.Message.Name,
            context.Message.Quantity,
            context.Message.Unit
        });
    }
}