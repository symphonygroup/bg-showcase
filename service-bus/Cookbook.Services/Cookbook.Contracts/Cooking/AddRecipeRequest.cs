namespace Cookbook.Contracts.Cooking;

public record AddRecipeRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Instructions { get; set; }
    public string ImageUrl { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public int Servings { get; set; }
    public List<RecipeIngredientModel> Ingredients { get; set; }
}