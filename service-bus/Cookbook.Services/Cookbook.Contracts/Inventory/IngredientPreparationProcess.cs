namespace Cookbook.Contracts.Inventory;

public record IngredientPreparationProcess
{
    public Guid CookingRecipeId { get; init; }
    public int PrepTime { get; init; }
}