using Cookbook.Contracts.Cooking;
using Cookbook.Contracts.Inventory;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cookbook.Cooking.Components.Cooking.Persistence;

public class CookingState
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    [BsonRepresentation(BsonType.Binary)]
    public Guid Id { get; set; }
    
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    [BsonRepresentation(BsonType.Binary)]
    public Guid CookingRequestId { get; set; }
    public string CurrentState { get; set; }
    public string RecipeId { get; set; }
    public int CookTime { get; set; }
    public int PrepTime { get; set; }
    public List<RecipeIngredientModel> Ingredients { get; set; }
    public List<IngredientModel> Inventory { get; set; }
    public int Version { get; set; }
}