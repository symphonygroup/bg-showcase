using Cookbook.Cooking.Components.Cooking.Persistence;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Cookbook.Cooking.Application.Extensions;

using Components;
using Components.Recipes.Persistence;
using Shared.Configuration;
using Shared.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DatabaseConfigurationExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var database = Database.GetMongoDbDatabase<MongoDbDatabaseOptions>(configuration);
        var mongoDbCollections = configuration.GetSection("MongoDbCollections").Get<MongoDbCollectionOptions>();
        
        var recipesClient = database.GetCollection<Recipe>(mongoDbCollections.Recipes);
        var cookingStatesClient = database.GetCollection<CookingState>(mongoDbCollections.CookingStates);
        var mealsClient = database.GetCollection<Meal>(mongoDbCollections.Meals);

        services.AddSingleton(recipesClient);
        services.AddSingleton(cookingStatesClient);
        services.AddSingleton(mealsClient);

        return services;
    }
}