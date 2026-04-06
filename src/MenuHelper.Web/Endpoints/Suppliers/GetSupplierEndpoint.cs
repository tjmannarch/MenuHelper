using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Web.Application.Queries.Suppliers;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Suppliers;

[Tags("Suppliers")]
[HttpGet("/api/suppliers/{id}")]
[AllowAnonymous]
public class GetSupplierEndpoint(IMediator mediator) : EndpointWithoutRequest<ResponseData<SupplierDto>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = new SupplierId(Route<Guid>("id"));
        var result = await mediator.Send(new GetSupplierQuery(id), ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
