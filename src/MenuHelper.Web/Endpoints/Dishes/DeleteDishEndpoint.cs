using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Web.Application.Commands.Dishes;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Dishes;

[Tags("Dishes")]
[HttpDelete("/api/dishes/{id}")]
[AllowAnonymous]
public class DeleteDishEndpoint(IMediator mediator)
    : EndpointWithoutRequest
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = new DishId(Route<Guid>("id"));
        await mediator.Send(new DeleteDishCommand(id), ct);
        await Send.NoContentAsync(ct);
    }
}
