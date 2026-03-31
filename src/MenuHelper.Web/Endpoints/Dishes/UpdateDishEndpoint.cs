using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Web.Application.Commands.Dishes;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Dishes;

public record UpdateDishRequest(string Name);

[Tags("Dishes")]
[HttpPut("/api/dishes/{id}")]
[AllowAnonymous]
public class UpdateDishEndpoint(IMediator mediator)
    : Endpoint<UpdateDishRequest>
{
    public override async Task HandleAsync(UpdateDishRequest req, CancellationToken ct)
    {
        var id = new DishId(Route<Guid>("id"));
        await mediator.Send(new UpdateDishCommand(id, req.Name), ct);
        await Send.NoContentAsync(ct);
    }
}
