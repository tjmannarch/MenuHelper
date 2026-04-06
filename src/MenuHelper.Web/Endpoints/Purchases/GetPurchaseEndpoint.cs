using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;
using MenuHelper.Web.Application.Queries.Purchases;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Purchases;

[Tags("Purchases")]
[HttpGet("/api/purchases/{id}")]
[AllowAnonymous]
public class GetPurchaseEndpoint(IMediator mediator) : EndpointWithoutRequest<ResponseData<PurchaseDetailDto>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = new PurchaseId(Route<Guid>("id"));
        var result = await mediator.Send(new GetPurchaseQuery(id), ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
