using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Web.Application.Commands.Suppliers;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Suppliers;

[Tags("Suppliers")]
[HttpDelete("/api/suppliers/{id}")]
[AllowAnonymous]
public class DeleteSupplierEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = new SupplierId(Route<Guid>("id"));
        await mediator.Send(new DeleteSupplierCommand(id), ct);
        await Send.NoContentAsync(ct);
    }
}
