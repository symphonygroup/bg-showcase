using System.ComponentModel.DataAnnotations;

namespace Cookbook.Api.Models.Cooking;

public record RecipeRequestModel
{
    [Required]
    public string Name { get; init; }
    [Required]
    public string Description { get; init; }
    [Required]
    public string Instructions { get; init; }
    [Required]
    public string ImageUrl { get; init; }
    public int PrepTime { get; init; }
    public int CookTime { get; init; }
    public int Servings { get; init; }

    public List<RecipeIngredientModel> Ingredients { get; init; }
}