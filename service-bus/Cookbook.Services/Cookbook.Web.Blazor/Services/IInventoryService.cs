using Cookbook.Contracts.Inventory;

namespace Cookbook.Web.Blazor.Services
{
    public interface IInventoryService
    {
        List<IngredientModel> Ingredients { get; }
        Task FetchIngredients();
        string ShowShortUnit(string unitName);
    }
}
