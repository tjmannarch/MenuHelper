using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Web.Application.Queries.Ingredients;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Ingredients;

[Tags("Ingredients")]
[HttpGet("/api/ingredients/{id}")]
[AllowAnonymous]
public class GetIngredientEndpoint(IMediator mediator)
    : EndpointWithoutRequest<ResponseData<IngredientDetailDto>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = new IngredientId(Route<Guid>("id"));
        var result = await mediator.Send(new GetIngredientQuery(id), ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
