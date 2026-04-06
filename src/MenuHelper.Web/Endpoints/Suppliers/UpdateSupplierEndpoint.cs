using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Web.Application.Commands.Suppliers;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Suppliers;

public record UpdateSupplierRequest(string Name, string? Phone, string? Remark);

[Tags("Suppliers")]
[HttpPut("/api/suppliers/{id}")]
[AllowAnonymous]
public class UpdateSupplierEndpoint(IMediator mediator)
    : Endpoint<UpdateSupplierRequest, EmptyResponse>
{
    public override async Task HandleAsync(UpdateSupplierRequest req, CancellationToken ct)
    {
        var id = new SupplierId(Route<Guid>("id"));
        await mediator.Send(new UpdateSupplierCommand(id, req.Name, req.Phone, req.Remark), ct);
        await Send.NoContentAsync(ct);
    }
}
