using AutoMapper;
using Cookbook.Contracts.Cooking;
using Cookbook.Contracts.Cooking.StateMachineEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Cookbook.Cooking.Components.Cooking;

public class CookingStatesConsumer : IConsumer<RecipeCookingStatesRequest>
{
    private readonly IMongoCollection<Cookbook.Cooking.Components.Cooking.Persistence.CookingState> _cookingStateCollection;
    private readonly IMapper _mapper;
    private readonly ILogger<CookingStatesConsumer> _logger;

    public CookingStatesConsumer(IMongoCollection<Cookbook.Cooking.Components.Cooking.Persistence.CookingState> cookingStateCollection, IMapper mapper,
        ILogger<CookingStatesConsumer> logger)
    {
        _cookingStateCollection = cookingStateCollection;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RecipeCookingStatesRequest> context)
    {
        // get all cooking states for the recipe
        var filter = Builders<Cookbook.Cooking.Components.Cooking.Persistence.CookingState>.Filter.Eq(x => x.RecipeId, context.Message.RecipeId);

        var cookingStates = await _cookingStateCollection.Find(filter).ToListAsync();

        if (cookingStates is null)
        {
            _logger.LogWarning("No cooking states found for recipe {RecipeId}", context.Message.RecipeId);
            await context.RespondAsync(new RecipeCookingStatesResponse
            {
                States = new List<CookingStateResponse>()
            });
            return;
        }

        var cookingStatesResponse =
            _mapper.Map<List<Cookbook.Cooking.Components.Cooking.Persistence.CookingState>, List<CookingStateResponse>>(cookingStates);
        
        await context.RespondAsync(new RecipeCookingStatesResponse
        {
            States = cookingStatesResponse
        });
    }
}