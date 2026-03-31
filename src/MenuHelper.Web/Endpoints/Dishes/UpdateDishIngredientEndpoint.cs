using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Web.Application.Commands.Dishes;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Dishes;

public record UpdateDishIngredientRequest(QuantityType QuantityType, decimal? FixedQuantity = null);

[Tags("Dishes")]
[HttpPut("/api/dishes/{id}/ingredients/{ingredientId}")]
[AllowAnonymous]
public class UpdateDishIngredientEndpoint(IMediator mediator)
    : Endpoint<UpdateDishIngredientRequest>
{
    public override async Task HandleAsync(UpdateDishIngredientRequest req, CancellationToken ct)
    {
        var dishId = new DishId(Route<Guid>("id"));
        var ingredientId = new IngredientId(Route<Guid>("ingredientId"));
        await mediator.Send(new UpdateDishIngredientCommand(dishId, ingredientId, req.QuantityType, req.FixedQuantity), ct);
        await Send.NoContentAsync(ct);
    }
}
