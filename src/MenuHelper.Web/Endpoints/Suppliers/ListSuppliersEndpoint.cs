using FastEndpoints;
using MenuHelper.Web.Application.Queries.Suppliers;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Suppliers;

[Tags("Suppliers")]
[HttpGet("/api/suppliers")]
[AllowAnonymous]
public class ListSuppliersEndpoint(IMediator mediator)
    : EndpointWithoutRequest<ResponseData<PagedData<SupplierListItemDto>>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var keyword = Query<string?>("keyword", isRequired: false);
        var pageIndex = Query<int>("pageIndex", isRequired: false);
        var pageSize = Query<int>("pageSize", isRequired: false);
        if (pageIndex <= 0) pageIndex = 1;
        if (pageSize <= 0) pageSize = 20;
        var result = await mediator.Send(new ListSuppliersQuery(keyword, pageIndex, pageSize), ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
