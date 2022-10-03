namespace Cookbook.Contracts.Cooking;

public class RecipesListResponse
{
    public ICollection<RecipeModel> Recipes { get; set; } = new List<RecipeModel>();
}