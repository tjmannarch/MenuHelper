using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Web.Application.Commands.Ingredients;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Ingredients;

public record UpdateIngredientRequest(
    string Name,
    string Unit,
    IngredientCategory Category,
    ConsumptionType ConsumptionType,
    decimal? DefaultUnitPrice,
    decimal? SafetyStockLevel = null,
    int? RestockCycleDays = null,
    int? MaxShelfDays = null);

[Tags("Ingredients")]
[HttpPut("/api/ingredients/{id}")]
[AllowAnonymous]
public class UpdateIngredientEndpoint(IMediator mediator)
    : Endpoint<UpdateIngredientRequest>
{
    public override async Task HandleAsync(UpdateIngredientRequest req, CancellationToken ct)
    {
        var id = new IngredientId(Route<Guid>("id"));
        await mediator.Send(new UpdateIngredientCommand(
            id, req.Name, req.Unit, req.Category, req.ConsumptionType, req.DefaultUnitPrice,
            req.SafetyStockLevel, req.RestockCycleDays, req.MaxShelfDays), ct);
        await Send.NoContentAsync(ct);
    }
}
