using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.DailyRevenueAggregate;
using MenuHelper.Web.Application.Commands.DailyRevenues;
using Microsoft.AspNetCore.Authorization;
namespace MenuHelper.Web.Endpoints.DailyRevenues;

public record UpsertDailyRevenueRequest(string Date, decimal Amount, string? Note);
public record UpsertDailyRevenueResponse(DailyRevenueId Id);

[Tags("DailyRevenues")]
[HttpPost("/api/daily-revenues")]
[AllowAnonymous]
public class UpsertDailyRevenueEndpoint(IMediator mediator) : Endpoint<UpsertDailyRevenueRequest, ResponseData<UpsertDailyRevenueResponse>>
{
    public override async Task HandleAsync(UpsertDailyRevenueRequest req, CancellationToken ct)
    {
        var date = DateOnly.Parse(req.Date);
        var id = await mediator.Send(new UpsertDailyRevenueCommand(date, req.Amount, req.Note), ct);
        await Send.OkAsync(new UpsertDailyRevenueResponse(id).AsResponseData(), cancellation: ct);
    }
}
