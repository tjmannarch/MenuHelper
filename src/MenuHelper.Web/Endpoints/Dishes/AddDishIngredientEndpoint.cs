using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Web.Application.Commands.Dishes;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Dishes;

public record AddDishIngredientRequest(IngredientId IngredientId, QuantityType QuantityType, decimal? FixedQuantity = null);

[Tags("Dishes")]
[HttpPost("/api/dishes/{id}/ingredients")]
[AllowAnonymous]
public class AddDishIngredientEndpoint(IMediator mediator)
    : Endpoint<AddDishIngredientRequest>
{
    public override async Task HandleAsync(AddDishIngredientRequest req, CancellationToken ct)
    {
        var dishId = new DishId(Route<Guid>("id"));
        await mediator.Send(new AddDishIngredientCommand(dishId, req.IngredientId, req.QuantityType, req.FixedQuantity), ct);
        await Send.NoContentAsync(ct);
    }
}
