using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cookbook.Cooking.Components.Cooking.Persistence;

public class Meal
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public int TotalServingsPrepared { get; set; }
    public int ServingsAvailable { get; set; }
}