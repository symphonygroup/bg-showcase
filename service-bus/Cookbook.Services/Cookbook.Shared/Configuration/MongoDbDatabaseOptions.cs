namespace Cookbook.Shared.Configuration;

public class MongoDbDatabaseOptions
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string SagaDatabaseName { get; set; }
}