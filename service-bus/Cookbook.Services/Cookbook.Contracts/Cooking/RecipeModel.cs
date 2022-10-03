using Cookbook.Contracts.Inventory;

namespace Cookbook.Contracts.Cooking;

public class RecipeModel
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Instructions { get; init; }
    public string ImageUrl { get; init; }
    public int PrepTime { get; init; }
    public int CookTime { get; init; }
    public int Servings { get; init; }
    public List<IngredientModel> Ingredients { get; set; }
}