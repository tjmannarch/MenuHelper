using FastEndpoints;
using MenuHelper.Web.Application.Queries.DailyRevenues;
using Microsoft.AspNetCore.Authorization;
namespace MenuHelper.Web.Endpoints.DailyRevenues;

public record GetDailyRevenueRequest(string Date);

[Tags("DailyRevenues")]
[HttpGet("/api/daily-revenues/{Date}")]
[AllowAnonymous]
public class GetDailyRevenueEndpoint(IMediator mediator) : Endpoint<GetDailyRevenueRequest, ResponseData<DailyRevenueDto>>
{
    public override async Task HandleAsync(GetDailyRevenueRequest req, CancellationToken ct)
    {
        var date = DateOnly.Parse(req.Date);
        var result = await mediator.Send(new GetDailyRevenueByDateQuery(date), ct);
        await Send.OkAsync((result ?? new DailyRevenueDto(req.Date, 0, null)).AsResponseData(), cancellation: ct);
    }
}
