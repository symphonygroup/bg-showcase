using Cookbook.Inventory.Components;
using Cookbook.Inventory.Components.Ingredients.Persistence;
using Cookbook.Shared.Configuration;
using Cookbook.Shared.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Inventory.Application.Extensions;

public static class DatabaseConfigurationExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var database = Database.GetMongoDbDatabase<MongoDbDatabaseOptions>(configuration);
        var mongoDbCollections = configuration.GetSection("MongoDbCollections").Get<MongoDbCollectionsOptions>();

        var ingredientsClient = database.GetCollection<Ingredient>(mongoDbCollections.Ingredients);
        
        services.AddSingleton(ingredientsClient);

        return services;
    }
}