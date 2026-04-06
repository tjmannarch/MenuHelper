using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Web.Application.Commands.Purchases;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Purchases;

[Tags("Purchases")]
[HttpPost("/api/suppliers/{id}/settle")]
[AllowAnonymous]
public class SettleSupplierEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = new SupplierId(Route<Guid>("id"));
        await mediator.Send(new SettleSupplierCommand(id), ct);
        await Send.NoContentAsync(ct);
    }
}
