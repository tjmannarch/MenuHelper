using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Web.Application.Queries.PurchaseOrders;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.PurchaseOrders;

public record GeneratePurchaseOrderRequestItem(IngredientId IngredientId, decimal Quantity);

public record GeneratePurchaseOrderRequest(
    List<GeneratePurchaseOrderRequestItem> Items,
    DateOnly? OrderDate = null);

[Tags("PurchaseOrders")]
[HttpPost("/api/purchase-orders/generate")]
[AllowAnonymous]
public class GeneratePurchaseOrderEndpoint(IMediator mediator)
    : Endpoint<GeneratePurchaseOrderRequest, ResponseData<PurchaseOrderDto>>
{
    public override async Task HandleAsync(GeneratePurchaseOrderRequest req, CancellationToken ct)
    {
        var items = req.Items
            .Select(i => new PurchaseOrderItemRequest(i.IngredientId, i.Quantity))
            .ToList();
        var result = await mediator.Send(new GeneratePurchaseOrderQuery(items, req.OrderDate), ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
