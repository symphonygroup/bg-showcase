namespace Cookbook.Contracts.Cooking;

public record AddRecipeResponse
{
    public string? RecipeId { get; set; }
    public string Name { get; set; }
    public DateTime ProcessedAt { get; set; }
    public bool Success { get; set; }
}