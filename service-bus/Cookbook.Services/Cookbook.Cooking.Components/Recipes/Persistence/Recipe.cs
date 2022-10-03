using Cookbook.Contracts.Cooking;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cookbook.Cooking.Components.Recipes.Persistence;

public class Recipe
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string Instructions { get; set; }
    public string ImageUrl { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public int Servings { get; set; }
    public List<RecipeIngredientModel> Ingredients { get; set; }
}