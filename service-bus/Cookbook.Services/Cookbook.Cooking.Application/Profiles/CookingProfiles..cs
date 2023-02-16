using AutoMapper;
using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Cooking.Components.Cooking.Persistence;

namespace Cookbook.Cooking.Application.Profiles;

public class CookingProfiles : Profile
{
    public CookingProfiles()
    {
        CreateMap<CookingState, CookingStateResponse>()
            .ForMember(x => x.Inventory, opt => opt.MapFrom(y => y.Inventory))
            .ForMember(x => x.Ingredients, opt => opt.MapFrom(y => y.Ingredients))
            .ForMember(x => x.CurrentState, opt => opt.MapFrom(y => y.CurrentState))
            .ForMember(x => x.CookTime, opt => opt.MapFrom(y => y.CookTime))
            .ForMember(x => x.PrepTime, opt => opt.MapFrom(y => y.PrepTime))
            .ForMember(x => x.RecipeId, opt => opt.MapFrom(y => y.RecipeId))
            .ForMember(x => x.CookingRequestId, opt => opt.MapFrom(y => y.CookingRequestId))
            .ForSourceMember(x => x.Id, opt => opt.DoNotValidate())
            .ForAllMembers(x => x.AllowNull());
    }
}