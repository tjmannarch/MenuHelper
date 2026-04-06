using FastEndpoints;
using MenuHelper.Web.Application.Queries.InventoryChecks;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.InventoryChecks;

[Tags("InventoryChecks")]
[HttpGet("/api/inventory-checks/{date}")]
[AllowAnonymous]
public class GetInventoryCheckEndpoint(IMediator mediator)
    : EndpointWithoutRequest<ResponseData<InventoryCheckDto>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var dateStr = Route<string>("date")!;
        var checkDate = DateOnly.Parse(dateStr, System.Globalization.CultureInfo.InvariantCulture);

        var result = await mediator.Send(new GetInventoryCheckQuery(checkDate), ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
