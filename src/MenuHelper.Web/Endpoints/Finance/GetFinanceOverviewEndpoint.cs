using FastEndpoints;
using MenuHelper.Web.Application.Queries.Finance;
using Microsoft.AspNetCore.Authorization;
namespace MenuHelper.Web.Endpoints.Finance;

public record FinanceRangeRequest(string From, string To);

[Tags("Finance")]
[HttpGet("/api/finance/overview")]
[AllowAnonymous]
public class GetFinanceOverviewEndpoint(IMediator mediator) : Endpoint<FinanceRangeRequest, ResponseData<FinanceOverviewDto>>
{
    public override async Task HandleAsync(FinanceRangeRequest req, CancellationToken ct)
    {
        var from = DateOnly.Parse(req.From);
        var to = DateOnly.Parse(req.To);
        var result = await mediator.Send(new GetFinanceOverviewQuery(from, to), ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}
