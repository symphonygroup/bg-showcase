using AutoMapper;
using Cookbook.Contracts.Cooking;
using Cookbook.Cooking.Components.Recipes.Persistence;

namespace Cookbook.Cooking.Application.Profiles;

public class RecipeProfiles : Profile
{
    public RecipeProfiles()
    {
        CreateMap<Recipe, RecipeModel>().ForMember(x => x.Ingredients, opt => opt.Ignore());
    }
}