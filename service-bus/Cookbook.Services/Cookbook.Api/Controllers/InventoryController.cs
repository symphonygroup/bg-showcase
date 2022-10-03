using Cookbook.Api.Models.Inventory;
using Cookbook.Contracts.Inventory;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Cookbook.Api.Controllers;

[ApiController]
[Route("inventory")]
public class InventoryController : ControllerBase
{
    private readonly IRequestClient<AddIngredientRequest> _addIngredientRequestClient;
    private readonly IRequestClient<IngredientsListRequest> _ingredientsListRequestClient;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public InventoryController(IRequestClient<AddIngredientRequest> addIngredientRequestClient,
        IRequestClient<IngredientsListRequest> ingredientsListRequestClient, ISendEndpointProvider sendEndpointProvider)
    {
        _addIngredientRequestClient = addIngredientRequestClient;
        _ingredientsListRequestClient = ingredientsListRequestClient;
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpPost("ingredients")]
    public async Task<IActionResult> AddIngredient([FromBody] NewIngredientModel newIngredient)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Response<AddIngredientResponse> response = await _addIngredientRequestClient.GetResponse<AddIngredientResponse>(
            new
            {
                newIngredient.Name,
                newIngredient.Unit,
                newIngredient.Quantity
            });

        return Ok(response.Message);
    }

    [HttpGet("ingredients")]
    public async Task<IActionResult> GetIngredients([FromQuery] string[]? ingredientIds)
    {
        Response<IngredientsListResponse> response =
            await _ingredientsListRequestClient.GetResponse<IngredientsListResponse>(new
            {
                IngredientIds = ingredientIds,
                RequestedAt = DateTime.UtcNow
            });

        return Ok(response.Message);
    }

    [HttpDelete("ingredients/{ingredientId}")]
    public async Task<IActionResult> DeleteIngredient([FromRoute] string ingredientId)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:remove-ingredient"));
        await endpoint.Send<RemoveIngredientRequested>(new
        {
            IngredientId = ingredientId
        });
        return NoContent();
    }
}