namespace Cookbook.Api.Models.Cooking;

public record RecipeIngredientModel
{
    public string IngredientId { get; init; }
    public int Quantity { get; init; }
}