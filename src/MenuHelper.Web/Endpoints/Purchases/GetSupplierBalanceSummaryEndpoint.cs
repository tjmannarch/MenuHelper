using FastEndpoints;
using MenuHelper.Web.Application.Queries.Purchases;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Purchases;

[Tags("Purchases")]
[HttpGet("/api/supplier-balances")]
[AllowAnonymous]
public class GetSupplierBalanceSummaryEndpoint(IMediator mediator)
    : EndpointWithoutRequest<ResponseData<List<SupplierBalanceDto>>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new GetSupplierBalanceSummaryQuery(), ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
