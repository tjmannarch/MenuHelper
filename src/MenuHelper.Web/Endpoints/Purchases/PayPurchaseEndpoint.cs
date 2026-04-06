using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;
using MenuHelper.Web.Application.Commands.Purchases;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Purchases;

[Tags("Purchases")]
[HttpPost("/api/purchases/{id}/pay")]
[AllowAnonymous]
public class PayPurchaseEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = new PurchaseId(Route<Guid>("id"));
        await mediator.Send(new PayPurchaseCommand(id), ct);
        await Send.NoContentAsync(ct);
    }
}
