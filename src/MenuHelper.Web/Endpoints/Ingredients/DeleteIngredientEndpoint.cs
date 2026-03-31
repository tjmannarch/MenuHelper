using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Web.Application.Commands.Ingredients;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Ingredients;

[Tags("Ingredients")]
[HttpDelete("/api/ingredients/{id}")]
[AllowAnonymous]
public class DeleteIngredientEndpoint(IMediator mediator)
    : EndpointWithoutRequest
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = new IngredientId(Route<Guid>("id"));
        await mediator.Send(new DeleteIngredientCommand(id), ct);
        await Send.NoContentAsync(ct);
    }
}
