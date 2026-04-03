using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Web.Application.Commands.Dishes;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Dishes;

[Tags("Dishes")]
[HttpDelete("/api/dishes/{id}/ingredients/{dishIngredientId}")]
[AllowAnonymous]
public class RemoveDishIngredientEndpoint(IMediator mediator)
    : EndpointWithoutRequest
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var dishId = new DishId(Route<Guid>("id"));
        var dishIngredientId = new DishIngredientId(Route<Guid>("dishIngredientId"));
        await mediator.Send(new RemoveDishIngredientCommand(dishId, dishIngredientId), ct);
        await Send.NoContentAsync(ct);
    }
}
