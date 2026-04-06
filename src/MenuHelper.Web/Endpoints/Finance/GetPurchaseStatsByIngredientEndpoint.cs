using FastEndpoints;
using MenuHelper.Web.Application.Queries.Finance;
using Microsoft.AspNetCore.Authorization;
namespace MenuHelper.Web.Endpoints.Finance;

[Tags("Finance")]
[HttpGet("/api/finance/purchase-by-ingredient")]
[AllowAnonymous]
public class GetPurchaseStatsByIngredientEndpoint(IMediator mediator) : Endpoint<FinanceRangeRequest, ResponseData<List<IngredientStatDto>>>
{
    public override async Task HandleAsync(FinanceRangeRequest req, CancellationToken ct)
    {
        var from = DateOnly.Parse(req.From);
        var to = DateOnly.Parse(req.To);
        var result = await mediator.Send(new GetPurchaseStatsByIngredientQuery(from, to), ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}
