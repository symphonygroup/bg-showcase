using Cookbook.Contracts.Cooking;

namespace Cookbook.Web.Blazor.Services
{
    public interface IRecipeService
    {
        List<RecipeModel> Recipes { get; }
        Task FetchRecipes();
    }
}
