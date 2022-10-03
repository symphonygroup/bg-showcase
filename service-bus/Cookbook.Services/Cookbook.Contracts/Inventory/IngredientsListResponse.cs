namespace Cookbook.Contracts.Inventory;

public record IngredientsListResponse
{
    public ICollection<IngredientModel> Ingredients { get; set; } = new List<IngredientModel>();
}