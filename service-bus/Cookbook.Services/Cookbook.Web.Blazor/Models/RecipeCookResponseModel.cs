namespace Cookbook.Web.Blazor.Models
{
    public record RecipeCookResponseModel
    {
        public string RecipeId { get; set; }
        public Guid CookingRequestId { get; set; }
    }
}
