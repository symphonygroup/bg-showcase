using Cookbook.Api.Models.Cooking;
using Cookbook.Contracts.Cooking;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Cookbook.Api.Controllers;

[ApiController]
[Route("cooking")]
public class CookingController : ControllerBase
{
    private readonly IRequestClient<AddRecipeRequest> _addRecipeRequestClient;
    private readonly IRequestClient<RecipesListRequest> _recipesListRequestClient;
    private readonly IRequestClient<CookingRequest> _cookingRequestClient;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public CookingController(IRequestClient<AddRecipeRequest> addRecipeRequestClient,
        IRequestClient<RecipesListRequest> recipesListRequestClient, ISendEndpointProvider sendEndpointProvider, IRequestClient<CookingRequest> cookingRequestClient)
    {
        _addRecipeRequestClient = addRecipeRequestClient;
        _recipesListRequestClient = recipesListRequestClient;
        _sendEndpointProvider = sendEndpointProvider;
        _cookingRequestClient = cookingRequestClient;
    }
    
    [HttpPost("recipes")]
    public async Task<IActionResult> AddRecipe([FromBody] RecipeRequestModel recipeRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _addRecipeRequestClient.GetResponse<AddRecipeResponse>(new
        {
            recipeRequest.Name,
            recipeRequest.Description,
            recipeRequest.Instructions,
            recipeRequest.Servings,
            recipeRequest.CookTime,
            recipeRequest.PrepTime,
            recipeRequest.Ingredients,
            recipeRequest.ImageUrl
        });

        return Ok(response.Message);
    }

    [HttpGet("recipes")]
    public async Task<IActionResult> GetRecipes([FromQuery] string[]? recipeIds)
    {
        var response = await _recipesListRequestClient.GetResponse<RecipesListResponse>(new
        {
            RecipeIds = recipeIds
        });

        return Ok(response.Message);
    }

    [HttpDelete("recipes/{recipeId}")]
    public async Task<IActionResult> DeleteRecipe([FromRoute] string recipeId)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:remove-recipe"));
        await endpoint.Send<RemoveRecipeRequested>(new
        {
            RecipeId = recipeId
        });
        return NoContent();
    }
    
    [HttpPost("{recipeId}/cook")]
    public async Task<IActionResult> CookRecipe([FromRoute] string recipeId)
    {
        var response = await _cookingRequestClient.GetResponse<CookingResponse>(new
        {
            RecipeId = recipeId
        });
        
        return Ok(response.Message);
    }
}