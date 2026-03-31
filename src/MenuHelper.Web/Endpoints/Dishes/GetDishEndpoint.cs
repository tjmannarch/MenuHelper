using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Web.Application.Queries.Dishes;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Dishes;

[Tags("Dishes")]
[HttpGet("/api/dishes/{id}")]
[AllowAnonymous]
public class GetDishEndpoint(IMediator mediator)
    : EndpointWithoutRequest<ResponseData<DishDetailDto>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = new DishId(Route<Guid>("id"));
        var result = await mediator.Send(new GetDishQuery(id), ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
