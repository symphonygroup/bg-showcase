using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Web.Blazor.Models;

namespace Cookbook.Web.Blazor.Services
{
    public interface ICookingService
    {
        Dictionary<string, List<CookingStateResponse>> CookingStates { get; }
        Task<RecipeCookResponseModel> Cook(string recipeId);
        Task FetchRecipeCookingStates(string recipeId);
        Task<CookingStateResponse> RefreshCookingState(Guid cookingRequestId);
    }
}
