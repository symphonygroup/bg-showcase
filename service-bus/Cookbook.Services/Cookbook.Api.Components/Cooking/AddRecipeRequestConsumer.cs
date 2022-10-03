using Cookbook.Contracts.Cooking;
using MassTransit;

namespace Cookbook.Api.Components.Cooking;

public class AddRecipeRequestConsumer : IConsumer<AddRecipeRequest>
{
    private readonly IRequestClient<AddRecipeRequested> _requestClient;

    public AddRecipeRequestConsumer(IRequestClient<AddRecipeRequested> requestClient)
    {
        _requestClient = requestClient;
    }

    public async Task Consume(ConsumeContext<AddRecipeRequest> context)
    {
        var (accepted, rejected) = await _requestClient.GetResponse<AddRecipeStored, AddRecipeStoreFailed>(new
        {
            context.Message.Name,
            context.Message.Description,
            context.Message.Ingredients,
            context.Message.Instructions,
            context.Message.ImageUrl,
            context.Message.Servings,
            context.Message.CookTime,
            context.Message.PrepTime
        });

        if (accepted.IsCompletedSuccessfully)
        {
            var response = await accepted;
            var result = new AddRecipeResponse
            {
                Success = true,
                RecipeId = response.Message.RecipeId,
                ProcessedAt = response.Message.Timestamp,
                Name = response.Message.Name
            };

            await context.RespondAsync(result);
        }
        else
        {
            var response = await rejected;
            var result = new AddRecipeResponse
            {
                Success = false,
                Name = response.Message.Name,
                RecipeId = null,
                ProcessedAt = response.Message.Timestamp
            };
            await context.RespondAsync(result);
        }
    }
}