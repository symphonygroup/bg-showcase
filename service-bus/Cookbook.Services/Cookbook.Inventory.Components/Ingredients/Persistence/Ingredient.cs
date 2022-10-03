using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cookbook.Inventory.Components.Ingredients.Persistence;

public class Ingredient
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; }
    public string Unit { get; set; }
    public int Quantity { get; set; }
}