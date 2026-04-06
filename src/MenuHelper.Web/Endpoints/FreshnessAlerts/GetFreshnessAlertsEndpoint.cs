using FastEndpoints;
using MenuHelper.Web.Application.Queries.FreshnessAlerts;
using Microsoft.AspNetCore.Authorization;
namespace MenuHelper.Web.Endpoints.FreshnessAlerts;

[Tags("FreshnessAlerts")]
[HttpGet("/api/freshness-alerts")]
[AllowAnonymous]
public class GetFreshnessAlertsEndpoint(IMediator mediator) : EndpointWithoutRequest<ResponseData<List<FreshnessAlertDto>>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new GetFreshnessAlertsQuery(), ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}
