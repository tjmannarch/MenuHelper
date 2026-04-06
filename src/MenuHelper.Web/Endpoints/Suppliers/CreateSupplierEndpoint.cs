using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Web.Application.Commands.Suppliers;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Suppliers;

public record CreateSupplierRequest(string Name, string? Phone, string? Remark);
public record CreateSupplierResponse(SupplierId SupplierId);

[Tags("Suppliers")]
[HttpPost("/api/suppliers")]
[AllowAnonymous]
public class CreateSupplierEndpoint(IMediator mediator)
    : Endpoint<CreateSupplierRequest, ResponseData<CreateSupplierResponse>>
{
    public override async Task HandleAsync(CreateSupplierRequest req, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateSupplierCommand(req.Name, req.Phone, req.Remark), ct);
        await Send.OkAsync(new CreateSupplierResponse(id).AsResponseData(), ct);
    }
}
