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

        services.AddSingleton(recipesClient);

        return services;
    }
}