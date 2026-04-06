using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Web.Application.Commands.Purchases;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Purchases;

public record CreatePurchaseRequestItemDto(IngredientId IngredientId, decimal Quantity, decimal UnitPrice);

public record CreatePurchaseRequest(
    SupplierId? SupplierId,
    DateOnly PurchaseDate,
    List<CreatePurchaseRequestItemDto> Items,
    string? Remark);

public record CreatePurchaseResponse(PurchaseId PurchaseId);

[Tags("Purchases")]
[HttpPost("/api/purchases")]
[AllowAnonymous]
public class CreatePurchaseEndpoint(IMediator mediator)
    : Endpoint<CreatePurchaseRequest, ResponseData<CreatePurchaseResponse>>
{
    public override async Task HandleAsync(CreatePurchaseRequest req, CancellationToken ct)
    {
        var items = req.Items
            .Select(i => new CreatePurchaseItemDto(i.IngredientId, i.Quantity, i.UnitPrice))
            .ToList();
        var id = await mediator.Send(new CreatePurchaseCommand(req.SupplierId, req.PurchaseDate, items, req.Remark), ct);
        await Send.OkAsync(new CreatePurchaseResponse(id).AsResponseData(), ct);
    }
}
