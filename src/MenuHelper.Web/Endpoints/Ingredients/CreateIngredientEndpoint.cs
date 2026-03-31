using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Web.Application.Commands.Ingredients;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Ingredients;

public record CreateIngredientRequest(
    string Name,
    string Unit,
    IngredientCategory Category,
    ConsumptionType ConsumptionType,
    decimal DefaultUnitPrice,
    SupplierId? SupplierId = null,
    decimal? SafetyStockLevel = null,
    int? RestockCycleDays = null,
    int? MaxShelfDays = null);

public record CreateIngredientResponse(IngredientId IngredientId);

[Tags("Ingredients")]
[HttpPost("/api/ingredients")]
[AllowAnonymous]
public class CreateIngredientEndpoint(IMediator mediator)
    : Endpoint<CreateIngredientRequest, ResponseData<CreateIngredientResponse>>
{
    public override async Task HandleAsync(CreateIngredientRequest req, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateIngredientCommand(
            req.Name, req.Unit, req.Category, req.ConsumptionType, req.DefaultUnitPrice,
            req.SupplierId, req.SafetyStockLevel, req.RestockCycleDays, req.MaxShelfDays), ct);
        await Send.OkAsync(new CreateIngredientResponse(id).AsResponseData(), ct);
    }
}
