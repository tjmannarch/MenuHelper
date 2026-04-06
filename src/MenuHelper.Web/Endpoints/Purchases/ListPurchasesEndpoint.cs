using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Web.Application.Queries.Purchases;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Purchases;

[Tags("Purchases")]
[HttpGet("/api/purchases")]
[AllowAnonymous]
public class ListPurchasesEndpoint(IMediator mediator)
    : EndpointWithoutRequest<ResponseData<PagedData<PurchaseListItemDto>>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var supplierIdRaw = Query<string?>("supplierId", isRequired: false);
        SupplierId? supplierId = supplierIdRaw != null && Guid.TryParse(supplierIdRaw, out var g)
            ? new SupplierId(g) : null;

        var fromStr = Query<string?>("from", isRequired: false);
        var toStr = Query<string?>("to", isRequired: false);
        DateOnly? from = fromStr != null && DateOnly.TryParse(fromStr, out var fd) ? fd : null;
        DateOnly? to = toStr != null && DateOnly.TryParse(toStr, out var td) ? td : null;

        var isPaidStr = Query<string?>("isPaid", isRequired: false);
        bool? isPaid = isPaidStr != null && bool.TryParse(isPaidStr, out var b) ? b : null;

        var pageIndex = Query<int>("pageIndex", isRequired: false);
        var pageSize = Query<int>("pageSize", isRequired: false);
        if (pageIndex <= 0) pageIndex = 1;
        if (pageSize <= 0) pageSize = 20;

        var result = await mediator.Send(new ListPurchasesQuery(supplierId, from, to, isPaid, pageIndex, pageSize), ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
