using System.ComponentModel.DataAnnotations;

namespace Cookbook.Api.Models.Inventory;

public record NewIngredientModel
{
    [Required] public string Name { get; init; }
    [Required] public string Unit { get; init; }
    [Required] public int Quantity { get; init; }
}