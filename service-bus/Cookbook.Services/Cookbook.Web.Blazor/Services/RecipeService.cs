using Cookbook.Contracts.Cooking;
using Newtonsoft.Json;

namespace Cookbook.Web.Blazor.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly HttpClient _httpClient;

        public RecipeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<RecipeModel> Recipes { get; set; } = new List<RecipeModel>();

        public async Task FetchRecipes()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/cooking/recipes");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            var convertedResponse = JsonConvert.DeserializeObject<RecipesListResponse>(responseJson);

            if (convertedResponse is not null)
            {
                Recipes = convertedResponse.Recipes.ToList();
            }
        }
    }
}
