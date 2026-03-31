using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Web.Application.Commands.Ingredients;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Ingredients;

public record BindIngredientSupplierRequest(SupplierId? SupplierId);

[Tags("Ingredients")]
[HttpPut("/api/ingredients/{id}/supplier")]
[AllowAnonymous]
public class BindIngredientSupplierEndpoint(IMediator mediator)
    : Endpoint<BindIngredientSupplierRequest>
{
    public override async Task HandleAsync(BindIngredientSupplierRequest req, CancellationToken ct)
    {
        var id = new IngredientId(Route<Guid>("id"));
        await mediator.Send(new BindIngredientSupplierCommand(id, req.SupplierId), ct);
        await Send.NoContentAsync(ct);
    }
}
