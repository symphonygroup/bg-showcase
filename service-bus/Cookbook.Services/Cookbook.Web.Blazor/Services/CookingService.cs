using Cookbook.Contracts.Cooking;
using Cookbook.Contracts.Cooking.StateMachineEvents;
using Cookbook.Web.Blazor.Models;
using Newtonsoft.Json;

namespace Cookbook.Web.Blazor.Services
{
    public class CookingService : ICookingService
    {
        private readonly HttpClient _httpClient;

        public CookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Dictionary<string, List<CookingStateResponse>> CookingStates { get; set; } = new Dictionary<string, List<CookingStateResponse>>();

        public async Task<RecipeCookResponseModel> Cook(string recipeId)
        {
            var cookRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"/cooking/{recipeId}/cook");

            var cookResponse = await _httpClient.SendAsync(cookRequestMessage);

            var responseJson = await cookResponse.Content.ReadAsStringAsync();

            var convertedResponse = JsonConvert.DeserializeObject<RecipeCookResponseModel>(responseJson);

            return convertedResponse;
        }

        public async Task FetchRecipeCookingStates(string recipeId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/cooking/{recipeId}/states");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            var convertedResponse = JsonConvert.DeserializeObject<RecipeCookingStatesResponse>(responseJson);

            if (convertedResponse is not null)
            {
                CookingStates[recipeId] = convertedResponse.States;
            }
        }

        public async Task<CookingStateResponse> RefreshCookingState(Guid cookingRequestId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/cooking/{cookingRequestId}/state");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            var convertedResponse = JsonConvert.DeserializeObject<CookingStateResponse>(responseJson);

            return convertedResponse;
        }
    }
}
