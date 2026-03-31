using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Web.Application.Commands.Dishes;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Dishes;

public record CreateDishRequest(string Name);
public record CreateDishResponse(DishId DishId);

[Tags("Dishes")]
[HttpPost("/api/dishes")]
[AllowAnonymous]
public class CreateDishEndpoint(IMediator mediator)
    : Endpoint<CreateDishRequest, ResponseData<CreateDishResponse>>
{
    public override async Task HandleAsync(CreateDishRequest req, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateDishCommand(req.Name), ct);
        await Send.OkAsync(new CreateDishResponse(id).AsResponseData(), ct);
    }
}
