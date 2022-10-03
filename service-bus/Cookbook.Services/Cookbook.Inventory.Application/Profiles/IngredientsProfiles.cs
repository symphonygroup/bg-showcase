using AutoMapper;
using Cookbook.Contracts.Inventory;
using Cookbook.Inventory.Components.Ingredients.Persistence;

namespace Cookbook.Inventory.Application.Profiles;

public class IngredientsProfiles : Profile
{
    public IngredientsProfiles()
    {
        CreateMap<Ingredient, IngredientModel>()
            .ForMember(x => x.IngredientId, opt => opt.MapFrom(x => x.Id));
    }
}