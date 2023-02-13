namespace Cookbook.Contracts.Inventory;

public record IngredientPreparationProcess
{
    public string RecipeId { get; init; }
    public Guid CookingRequestId { get; init; }
    public int PrepTime { get; init; }
}