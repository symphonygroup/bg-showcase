using Cookbook.Shared.Configuration;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace Cookbook.Shared.Database;

public static class Database
{
    public static IMongoDatabase GetMongoDbDatabase<T>(IConfiguration configuration, string sectionName = "MongoDbDatabase") where T : MongoDbDatabaseOptions
    {
        var options = configuration.GetSection(sectionName).Get<T>();
        var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(options.ConnectionString));
        clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
        var client = new MongoClient(clientSettings);

        var database = client.GetDatabase(options.DatabaseName);
        return database;
    }
}