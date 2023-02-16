using Cookbook.Contracts.Inventory;
using Newtonsoft.Json;

namespace Cookbook.Web.Blazor.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly HttpClient _httpClient;

        public InventoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<IngredientModel> Ingredients { get; set; } = new List<IngredientModel>();

        public async Task FetchIngredients()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/inventory/ingredients");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            var convertedResponse = JsonConvert.DeserializeObject<IngredientsListResponse>(responseJson);

            if (convertedResponse is not null)
            {
                Ingredients = convertedResponse.Ingredients.ToList();
            }
        }

        public string ShowShortUnit(string unitName)
        {
            switch (unitName)
            {
                case "Gram":
                    return "g";
                case "Piece":
                    return "p";
                case "Millilitres":
                    return "ml";
                default:
                    return string.Empty;
            }
        }
    }
}
