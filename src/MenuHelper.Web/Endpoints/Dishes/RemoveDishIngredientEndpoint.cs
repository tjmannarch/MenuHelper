using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Web.Application.Commands.Dishes;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Dishes;

[Tags("Dishes")]
[HttpDelete("/api/dishes/{id}/ingredients/{ingredientId}")]
[AllowAnonymous]
public class RemoveDishIngredientEndpoint(IMediator mediator)
    : EndpointWithoutRequest
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var dishId = new DishId(Route<Guid>("id"));
        var ingredientId = new IngredientId(Route<Guid>("ingredientId"));
        await mediator.Send(new RemoveDishIngredientCommand(dishId, ingredientId), ct);
        await Send.NoContentAsync(ct);
    }
}
